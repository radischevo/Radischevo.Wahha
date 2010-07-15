using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.SessionState;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Routing;
using Radischevo.Wahha.Web.Scripting.Serialization;
using Radischevo.Wahha.Web.Mvc.Configurations;

namespace Radischevo.Wahha.Web.Mvc
{
    public class Controller : ControllerBase, IActionFilter, IResultFilter, 
        IExceptionFilter, IAuthorizationFilter
    {
        #region Instance Fields
        private IActionExecutor _actionExecutor;
        private ITempDataProvider _tempDataProvider;
        #endregion

        #region Constructors
        public Controller()
        {
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets the <see cref="Radischevo.Wahha.Web.Mvc.IActionExecutor"/> 
        /// for the controller.
        /// </summary>
        protected IActionExecutor ActionExecutor
        {
            get
            {
				if (_actionExecutor == null)
					_actionExecutor = CreateActionExecutor();

                return _actionExecutor;
            }
            set
            {
                Precondition.Require(value, () => Error.ArgumentNull("value"));
                _actionExecutor = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="Radischevo.Wahha.Web.Mvc.ITempDataProvider"/> 
        /// object used to store data for the next request.
        /// </summary>
        protected ITempDataProvider TempDataProvider
        {
            get
            {
                if (_tempDataProvider == null)
                    _tempDataProvider = new SessionStateTempDataProvider();

                return _tempDataProvider;
            }
            set
            {
                _tempDataProvider = value;
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Method called before the action method is invoked.
        /// </summary>
        /// <param name="context">Contains information about the current request and action</param>
        protected virtual void OnPreAction(ActionExecutionContext context)
        {
        }

        /// <summary>
        /// Method called after the action method is invoked.
        /// </summary>
        /// <param name="context">Contains information about the current request and action</param>
        protected virtual void OnPostAction(ActionExecutedContext context)
        {
        }

        /// <summary>
        /// Method called before the action result returned by an action method is executed.
        /// </summary>
        /// <param name="context">Contains information about the current request and action result</param>
        protected virtual void OnPreResult(ResultExecutionContext context)
        {
        }

        /// <summary>
        /// Method called after the action result returned by an action method is executed.
        /// </summary>
        /// <param name="context">Contains information about the current request and action result</param>
        protected virtual void OnPostResult(ResultExecutedContext context)
        {
        }

        /// <summary>
        /// Method called when authorization occurs.
        /// </summary>
        /// <param name="context">Contains information about the current request and action</param>
        protected virtual void OnAuthorization(AuthorizationContext context)
        {
        }

        /// <summary>
        /// Method called when an unhandled exception occurs in the action.
        /// </summary>
        /// <param name="context">Contains information about the current request and action</param>
        protected virtual void OnException(ExceptionContext context)
        {
        }
        #endregion

        #region Instance Methods
        protected override void ProcessRequest()
        {
			LoadTempData();
            
            try
            {
                string actionName = RouteData.GetRequiredValue<string>("action");
                if (!ActionExecutor.InvokeAction(Context, actionName, null))
                    HandleUnknownAction(actionName);
            }
            finally
            {
				SaveTempData();
            }
        }

		protected virtual void LoadTempData()
		{
			if (!Context.IsChild)
				TempData.Load(Context, TempDataProvider);
		}

		protected virtual void SaveTempData()
		{
			if (!Context.IsChild)
				TempData.Save(Context, TempDataProvider);
		}

		protected virtual IActionExecutor CreateActionExecutor()
		{
			return new ActionExecutor(Context);
		}

        /// <summary>
        /// Method called whenever a request matches this controller, 
        /// but not an action of this controller.
        /// </summary>
        /// <param name="actionName">The name of the attempted action</param>
        protected virtual void HandleUnknownAction(string actionName)
        {
            throw Error.InvalidMvcAction(this.GetType(), actionName);
        }

        protected void Include(string controllerName, string actionName)
        {
            Include(controllerName, actionName, new ValueDictionary());
        }

        protected void Include(string controllerName,
            string actionName, object arguments)
        {
            Include(controllerName, actionName, new ValueDictionary(arguments));
        }

        protected virtual void Include<TController>(Expression<Action<TController>> action)
            where TController : Controller
        {
            MethodCallExpression mce = (action.Body as MethodCallExpression);
            Precondition.Require(mce, () => Error.ExpressionMustBeAMethodCall("action"));
            Precondition.Require(mce.Object == action.Parameters[0],
				() => Error.MethodCallMustTargetLambdaArgument("action"));

            Include(typeof(TController).Name,
                ActionMethodSelector.GetNameOrAlias(mce.Method),
                LinqHelper.ExtractArgumentsToDictionary(mce));
        }

        protected virtual void Include(string controllerName,
            string actionName, ValueDictionary arguments)
        {
            IControllerFactory factory =
                ControllerBuilder.Instance.GetControllerFactory();
            IController controller = null;

			try
			{
				controller = factory.CreateController(Context, controllerName);
				Controller c = (controller as Controller);
				Precondition.Require(c, () => Error.TargetMustSubclassController(controllerName));

				using (ChildContextOperator child = c.InitializeChildRequest(Context))
				{
					if (!c.ActionExecutor.InvokeAction(c.Context, actionName, arguments))
						c.HandleUnknownAction(actionName);
				}
			}
			catch (HttpException ex)
			{
				if (ex.GetHttpCode() == 500)
					throw;

				throw Error.ChildRequestExecutionError(ex);
			}
            finally
            {
                factory.ReleaseController(controller);
            }
        }

        protected string Resource(string expression, params object[] args)
        {
            return ResourceHelper.GetResourceString(expression, "~/", args);
        }
        #endregion

        #region Result executing methods
        /// <summary>
        /// Returns a <see cref="T:Radischevo.Wahha.Web.Mvc.ContentResult"/> 
        /// which renders the supplied content to the response.
        /// </summary>
        /// <param name="content">The content to write to the response</param>
        protected ContentResult Content(string content)
        {
            return Content(content, null);
        }

        /// <summary>
        /// Returns a <see cref="T:Radischevo.Wahha.Web.Mvc.ContentResult"/> 
        /// which renders the supplied content to the response.
        /// </summary>
        /// <param name="content">The content to write to the response</param>
        /// <param name="contentType">The content MIME-type</param>
        protected ContentResult Content(string content, string contentType)
        {
            return Content(content, contentType, null);
        }

        /// <summary>
        /// Returns a <see cref="T:Radischevo.Wahha.Web.Mvc.ContentResult"/> 
        /// which renders the supplied content to the response.
        /// </summary>
        /// <param name="content">The content to write to the response</param>
        /// <param name="contentType">The content MIME-type</param>
        /// <param name="contentEncoding">The content encoding</param>
        protected virtual ContentResult Content(string content, 
            string contentType, Encoding contentEncoding)
        {
            return new ContentResult()
            {
                Content = content,
                ContentType = contentType,
                ContentEncoding = contentEncoding
            };
        }

        /// <summary>
        /// Returns a <see cref="Radischevo.Wahha.Web.Mvc.JsonResult"/> which serializes the 
        /// specified object to JSON and writes the JSON to the response.
        /// </summary>
        /// <param name="data">The object to serialize</param>
        protected JsonResult Json(object data)
        {
            return Json(data, null, null, null);
        }

        /// <summary>
        /// Returns a <see cref="Radischevo.Wahha.Web.Mvc.JsonResult"/> which serializes the 
        /// specified object to JSON and writes the JSON to the response.
        /// </summary>
        /// <param name="data">The object to serialize</param>
        /// <param name="converters">A collection of custom object-to-JSON converters.</param>
        protected JsonResult Json(object data, IEnumerable<JavaScriptConverter> converters)
        {
            return Json(data, null, null, converters);
        }

        /// <summary>
        /// Returns a <see cref="Radischevo.Wahha.Web.Mvc.JsonResult"/> which serializes the 
        /// specified object to JSON and writes the JSON to the response.
        /// </summary>
        /// <param name="data">The object to serialize</param>
        /// <param name="contentType">The content type</param>
        protected JsonResult Json(object data, string contentType)
        {
            return Json(data, contentType, null, null);
        }

        /// <summary>
        /// Returns a <see cref="Radischevo.Wahha.Web.Mvc.JsonResult"/> which serializes the 
        /// specified object to JSON and writes the JSON to the response.
        /// </summary>
        /// <param name="data">The object to serialize</param>
        /// <param name="contentType">The content type</param>
        /// <param name="converters">A collection of custom object-to-JSON converters.</param>
        protected JsonResult Json(object data, string contentType, 
            IEnumerable<JavaScriptConverter> converters)
        {
            return Json(data, contentType, null, converters);
        }

        /// <summary>
        /// Returns a <see cref="Radischevo.Wahha.Web.Mvc.JsonResult"/> which serializes the 
        /// specified object to JSON and writes the JSON to the response.
        /// </summary>
        /// <param name="data">The object to serialize</param>
        /// <param name="contentType">The content type</param>
        /// <param name="contentEncoding">The content encoding</param>
        protected JsonResult Json(object data,
            string contentType, Encoding contentEncoding)
        {
            return Json(data, contentType, contentEncoding, null);
        }

        /// <summary>
        /// Returns a <see cref="Radischevo.Wahha.Web.Mvc.JsonResult"/> which serializes the 
        /// specified object to JSON and writes the JSON to the response.
        /// </summary>
        /// <param name="data">The object to serialize</param>
        /// <param name="contentType">The content type</param>
        /// <param name="contentEncoding">The content encoding</param>
        /// <param name="converters">A collection of custom object-to-JSON converters.</param>
        protected virtual JsonResult Json(object data, 
            string contentType, Encoding contentEncoding, 
            IEnumerable<JavaScriptConverter> converters)
        {
            return new JsonResult() {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                Converters = converters
            };
        }

        /// <summary>
        /// Returns a <see cref="Radischevo.Wahha.Web.Mvc.RedirectResult"/> 
        /// which redirects to the specified URL.
        /// </summary>
        /// <param name="url">The URL to redirect to.</param>
        protected virtual ActionResult Redirect(string url)
        {
            Precondition.Defined(url, () => Error.ArgumentNull("url"));
            return new RedirectResult(url);
        }

		/// <summary>
		/// Returns a <see cref="Radischevo.Wahha.Web.Mvc.PermanentRedirectResult"/> 
		/// which permanently redirects to the specified URL.
		/// </summary>
		/// <param name="url">The URL to redirect to.</param>
		protected virtual ActionResult RedirectPermanent(string url)
		{
			Precondition.Defined(url, () => Error.ArgumentNull("url"));
			return new PermanentRedirectResult(url);
		}

		/// <summary>
		/// Returns a <see cref="Radischevo.Wahha.Web.Mvc.RedirectResult"/> 
		/// which redirects to the specified route.
		/// </summary>
		/// <param name="routeName">The name of the route.</param>
		protected ActionResult Route(string routeName)
		{
			return Route(routeName, null, null);
		}

        /// <summary>
        /// Returns a <see cref="Radischevo.Wahha.Web.Mvc.RedirectResult"/> 
        /// which redirects to the specified route.
        /// </summary>
        /// <param name="values">An object containing the parameters for a route.</param>
        protected ActionResult Route(object values)
        {
            ValueDictionary rvd = new ValueDictionary(values);
            return Route(null, rvd, null);
        }

        /// <summary>
        /// Returns a <see cref="Radischevo.Wahha.Web.Mvc.RedirectResult"/> 
        /// which redirects to the specified route.
        /// </summary>
        /// <param name="values">An object containing the parameters for a route.</param>
        protected ActionResult Route(ValueDictionary values)
        {
            return Route(null, values, null);
        }

        /// <summary>
        /// Returns a <see cref="Radischevo.Wahha.Web.Mvc.RedirectResult"/> 
        /// which redirects to the specified route.
        /// </summary>
        /// <param name="values">An object containing the parameters for a route.</param>
        /// <param name="routeName">The name of the route</param>
        protected ActionResult Route(string routeName, object values)
        {
            ValueDictionary rvd = new ValueDictionary(values);
            return Route(routeName, rvd, null);
        }

        /// <summary>
        /// Returns a <see cref="Radischevo.Wahha.Web.Mvc.RedirectResult"/> 
        /// which redirects to the specified route.
        /// </summary>
        /// <param name="values">An object containing the parameters for a route.</param>
        /// <param name="routeName">The name of the route</param>
        protected ActionResult Route(string routeName, ValueDictionary values)
        {
            return Route(routeName, values, null);
        }

        /// <summary>
        /// Returns a <see cref="Radischevo.Wahha.Web.Mvc.RedirectResult"/> 
        /// which redirects to the specified route.
        /// </summary>
        /// <param name="values">An object containing the parameters for a route.</param>
        /// <param name="routeName">The name of the route</param>
        /// <param name="suffix">An optional suffix, which will be added to the resulting URL.</param>
        protected virtual ActionResult Route(string routeName, 
            ValueDictionary values, string suffix)
        {
			values = values ?? new ValueDictionary();

            VirtualPathData vp = RouteTable.Routes.GetVirtualPath(Context, routeName, values);
            if (vp != null)
                return new RedirectResult(String.Concat(vp.VirtualPath, suffix));

            return EmptyResult.Instance;
        }

		protected ActionResult Route<TController>(
			Expression<Action<TController>> action)
			where TController : IController
		{
			return Route<TController>(null, action, null);
		}

		protected ActionResult Route<TController>(string routeName,
			Expression<Action<TController>> action)
			where TController : IController
		{
			return Route<TController>(routeName, action, null);
		}

		protected ActionResult Route<TController>(string routeName, 
			Expression<Action<TController>> action, string suffix)
			where TController : IController
		{
			Precondition.Require(action, () => Error.ArgumentNull("action"));

			MethodCallExpression mexp = (action.Body as MethodCallExpression);
			if (mexp == null)
				throw Error.ExpressionMustBeAMethodCall("action");

			if (mexp.Object != action.Parameters[0])
				throw Error.MethodCallMustTargetLambdaArgument("action");

			ValueDictionary values = LinqHelper.ExtractArgumentsToDictionary(mexp);
			values = (values != null) ? values : new ValueDictionary();

			values["controller"] = typeof(TController).Name;
			values["action"] = ActionMethodSelector.GetNameOrAlias(mexp.Method);

			return Route(routeName, values, suffix);
		}

        /// <summary>
        /// Returns a <see cref="Radischevo.Wahha.Web.Mvc.ViewResult"/> 
        /// which renders a view to the response.
        /// </summary>
        /// <param name="viewName">The name of the view</param>
        protected ViewResult View(string viewName)
        {
            return View(viewName, null, ViewData);
        }

        /// <summary>
        /// Returns a <see cref="Radischevo.Wahha.Web.Mvc.ViewResult"/> 
        /// which renders a view to the response.
        /// </summary>
        /// <param name="viewName">The name of the view</param>
        /// <param name="viewData">The view data supplied to the view</param>
        protected ViewResult View(string viewName, ViewDataDictionary viewData)
        {
            return View(viewName, null, viewData);
        }

        /// <summary>
        /// Returns a <see cref="Radischevo.Wahha.Web.Mvc.ViewResult"/> 
        /// which renders a view to the response.
        /// </summary>
        /// <param name="viewName">The name of the view</param>
        /// <param name="model">The model rendered by the view</param>
        protected ViewResult View(string viewName, object model)
        {
            return View(viewName, model, ViewData);
        }

        /// <summary>
        /// Returns a <see cref="Radischevo.Wahha.Web.Mvc.ViewResult"/> 
        /// which renders a view to the response.
        /// </summary>
        /// <param name="viewName">The name of the view</param>
        /// <param name="model">The model rendered by the view</param>
        /// <param name="viewData">The view data supplied to the view</param>
        protected virtual ViewResult View(string viewName, 
            object model, ViewDataDictionary viewData)
        {
            ViewDataDictionary data = (viewData == null)
                ? new ViewDataDictionary() : viewData;

            if (model != null)
                data.Model = model;

            return new ViewResult() {
                ViewName = viewName,
                ViewData = data,
                TempData = TempData,
                Errors = Errors
            };
        }

        /// <summary>
        /// Returns a <see cref="Radischevo.Wahha.Web.Mvc.ViewResult"/> 
        /// which renders the specified <see cref="Radischevo.Wahha.Web.Mvc.IView"/> 
        /// to the response.
        /// </summary>
        /// <param name="view">The view rendered to the response</param>
        protected ViewResult View(IView view)
        {
            return View(view, null, ViewData);
        }

        /// <summary>
        /// Returns a <see cref="Radischevo.Wahha.Web.Mvc.ViewResult"/> 
        /// which renders the specified <see cref="Radischevo.Wahha.Web.Mvc.IView"/> 
        /// to the response.
        /// </summary>
        /// <param name="view">The view rendered to the response</param>
        /// <param name="model">The model rendered by the view</param>
        protected ViewResult View(IView view, object model)
        {
            return View(view, model, ViewData);
        }

        /// <summary>
        /// Returns a <see cref="Radischevo.Wahha.Web.Mvc.ViewResult"/> 
        /// which renders the specified <see cref="Radischevo.Wahha.Web.Mvc.IView"/> 
        /// to the response.
        /// </summary>
        /// <param name="view">The view rendered to the response</param>
        /// <param name="model">The model rendered by the view</param>
        /// <param name="viewData">The view data supplied to the view</param>
        protected virtual ViewResult View(IView view, object model, ViewDataDictionary viewData)
        {
            ViewDataDictionary data = (viewData == null)
                ? new ViewDataDictionary() : viewData;

            if (model != null)
                data.Model = model;

            return new ViewResult()
            {
                View = view,
                ViewData = data,
                TempData = TempData,
                Errors = Errors
            };
        }
        #endregion

        #region Update Model with Binder Methods
        protected TModel BindModel<TModel, TBinder>(TModel model)
            where TBinder : IModelBinder, new()
        {
            return BindModel<TModel, TBinder>(model, String.Empty,
                new string[0], new string[0], (IDictionary<string, object>)null);
        }

        protected TModel BindModel<TModel, TBinder>(TModel model, string[] include)
            where TBinder : IModelBinder, new()
        {
            return BindModel<TModel, TBinder>(model, String.Empty, include,
                new string[0], (IDictionary<string, object>)null);
        }

        protected TModel BindModel<TModel, TBinder>(TModel model, IValueSet values)
            where TBinder : IModelBinder, new()
        {
            return BindModel<TModel, TBinder>(model, String.Empty,
                new string[0], new string[0], values);
        }

        protected TModel BindModel<TModel, TBinder>(TModel model,
            IDictionary<string, object> values)
            where TBinder : IModelBinder, new()
        {
            return BindModel<TModel, TBinder>(model, String.Empty,
                new string[0], new string[0], values);
        }

        protected TModel BindModel<TModel, TBinder>(TModel model, string name)
            where TBinder : IModelBinder, new()
        {
            return BindModel<TModel, TBinder>(model, name,
                new string[0], new string[0], (IDictionary<string, object>)null);
        }

        protected TModel BindModel<TModel, TBinder>(TModel model, string name,
            IValueSet values)
            where TBinder : IModelBinder, new()
        {
            return BindModel<TModel, TBinder>(model, name,
                new string[0], new string[0], values);
        }

        protected TModel BindModel<TModel, TBinder>(TModel model, string name,
            IDictionary<string, object> values)
            where TBinder : IModelBinder, new()
        {
            return BindModel<TModel, TBinder>(model, name, 
                new string[0], new string[0], values);
        }

        protected TModel BindModel<TModel, TBinder>(TModel model, 
            string name, string[] include)
            where TBinder : IModelBinder, new()
        {
            return BindModel<TModel, TBinder>(model, name,
                include, new string[0], (IDictionary<string, object>)null);
        }

        protected TModel BindModel<TModel, TBinder>(TModel model, string name,
            string[] include, IValueSet values)
            where TBinder : IModelBinder, new()
        {
            return BindModel<TModel, TBinder>(model, name, include, new string[0], values);
        }

        protected TModel BindModel<TModel, TBinder>(TModel model, string name,
            string[] include, IDictionary<string, object> values)
            where TBinder : IModelBinder, new()
        {
            return BindModel<TModel, TBinder>(model, name, include, new string[0], values);
        }

        protected TModel BindModel<TModel, TBinder>(TModel model, string name,
            string[] include, string[] exclude)
            where TBinder : IModelBinder, new()
        {
            return BindModel<TModel, TBinder>(model, name, 
                include, exclude, (IDictionary<string, object>)null);
        }

        protected TModel BindModel<TModel, TBinder>(TModel model, string name,
            string[] include, string[] exclude, IValueSet values)
            where TBinder : IModelBinder, new()
        {
            IDictionary<string, object> data = (values == null) ? 
                null : new ValueDictionary().Merge(values);
            return BindModel<TModel, TBinder>(model, name, include, exclude, data);
        }

        protected TModel BindModel<TModel, TBinder>(TModel model, string name,
            string[] include, string[] exclude, 
            IDictionary<string, object> values)
            where TBinder : IModelBinder, new()
        {
            IModelBinder binder = Activator.CreateInstance<TBinder>();
            ValueDictionary data = (values == null) ? null : new ValueDictionary(values);

            BindingContext bc = new BindingContext(Context, typeof(TModel),
                name, ParameterSource.Default, data, 
                s => BindAttribute.IsUpdateAllowed(s, include, exclude),
                Errors);

            bc.Model = model;
            return (TModel)binder.Bind(bc);
        }
        #endregion

        #region Update Model Methods
        protected TModel BindModel<TModel>(TModel model)
        {
            return BindModel<TModel>(model, String.Empty,
                new string[0], new string[0], (IDictionary<string, object>)null);
        }

        protected TModel BindModel<TModel>(TModel model, string[] include) 
        {
            return BindModel<TModel>(model, String.Empty, include,
                new string[0], (IDictionary<string, object>)null);
        }

        protected TModel BindModel<TModel>(TModel model, IValueSet values)
        {
            return BindModel<TModel>(model, String.Empty,
                new string[0], new string[0], values);
        }

        protected TModel BindModel<TModel>(TModel model, 
            IDictionary<string, object> values) 
        {
            return BindModel<TModel>(model, String.Empty, 
                new string[0], new string[0], values);
        }

        protected TModel BindModel<TModel>(TModel model, string name) 
        {
            return BindModel<TModel>(model, name,
                new string[0], new string[0], (IDictionary<string, object>)null);
        }

        protected TModel BindModel<TModel>(TModel model, string name, IValueSet values)
        {
            return BindModel<TModel>(model, name,
                new string[0], new string[0], values);
        }

        protected TModel BindModel<TModel>(TModel model, string name, 
            IDictionary<string, object> values) 
        {
            return BindModel<TModel>(model, name, 
                new string[0], new string[0], values);
        }

        protected TModel BindModel<TModel>(TModel model, string name, string[] include) 
        {
            return BindModel<TModel>(model, name, include, 
                new string[0], (IDictionary<string, object>)null);
        }

        protected TModel BindModel<TModel>(TModel model, string name,
            string[] include, IValueSet values)
        {
            return BindModel<TModel>(model, name, include, new string[0], values);
        }

        protected TModel BindModel<TModel>(TModel model, string name, 
            string[] include, IDictionary<string, object> values)
        {
            return BindModel<TModel>(model, name, include, new string[0], values);
        }

        protected TModel BindModel<TModel>(TModel model, string name,
            string[] include, string[] exclude)
        {
            return BindModel<TModel>(model, name, include, 
                exclude, (IDictionary<string, object>)null);
        }

        protected TModel BindModel<TModel>(TModel model, string name,
            string[] include, string[] exclude, IValueSet values)
        {
            IDictionary<string, object> data = (values == null) ? 
                null : new ValueDictionary().Merge(values);
            return BindModel<TModel>(model, name, include, exclude, data);
        }

        protected TModel BindModel<TModel>(TModel model, string name, 
            string[] include, string[] exclude, IDictionary<string, object> values) 
        {
            ValueDictionary data = (values == null) ? null : new ValueDictionary(values);

            IModelBinder binder = Configuration.Instance
                .Models.Binders.GetBinder(typeof(TModel));

            BindingContext bc = new BindingContext(Context, typeof(TModel),
                name, ParameterSource.Default, data, 
                s => BindAttribute.IsUpdateAllowed(s, include, exclude), 
                Errors);
            
            bc.Model = model;
            return (TModel)binder.Bind(bc);
        }
        #endregion

        #region Interface Implementations
        void IActionFilter.OnExecuting(ActionExecutionContext context)
        {
            OnPreAction(context);
        }

        void IActionFilter.OnExecuted(ActionExecutedContext context)
        {
            OnPostAction(context);
        }

        void IResultFilter.OnResultExecuting(ResultExecutionContext context)
        {
            OnPreResult(context);
        }

        void IResultFilter.OnResultExecuted(ResultExecutedContext context)
        {
            OnPostResult(context);
        }

        void IExceptionFilter.OnException(ExceptionContext context)
        {
            OnException(context);
        }

        void IAuthorizationFilter.OnAuthorization(AuthorizationContext context)
        {
            OnAuthorization(context);
        }
        #endregion
    }
}

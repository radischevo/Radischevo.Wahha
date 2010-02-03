using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;
using Radischevo.Wahha.Web.Routing;

namespace Radischevo.Wahha.Web.Mvc
{
    public class ActionExecutor : IActionExecutor
    {
        #region Static Fields
        private static ControllerDescriptorCache _staticCache = new ControllerDescriptorCache();
        #endregion

        #region Instance Fields
        private ControllerDescriptorCache _instanceCache;
        private ControllerContext _context;
        private ModelBinderCollection _binders;
        #endregion

        #region Constructors
        public ActionExecutor(ControllerContext context)
        {
            Precondition.Require(context, Error.ArgumentNull("context"));
            Precondition.Require(context.Controller, Error.ArgumentNull("controller"));

            _context = context;
        }
        #endregion

        #region Instance Properties
        public ControllerContext Context
        {
            get
            {
                return _context;
            }
        }

        protected ControllerDescriptorCache Descriptors
        {
            get
            {
                if (_instanceCache == null)
                    _instanceCache = _staticCache;

                return _instanceCache;
            }
        }

        protected ModelBinderCollection Binders
        {
            get
            {
                if (_binders == null)
                    _binders = Configuration.Configuration.Instance.Models.Binders;

                return _binders;
            }
        }
        #endregion

        #region Static Methods
        private static ActionExecutedContext InvokeActionFilter(IActionFilter filter, 
            ActionExecutionContext context, Func<ActionExecutedContext> continuation)
        {
            filter.OnExecuting(context);
            if (context.Cancel)
                return new ActionExecutedContext(context, null) { Result = context.Result };

            bool wasError = false;
            ActionExecutedContext postContext = null;
            try
            {
                postContext = continuation();
            }
            catch (Exception ex)
            {
                wasError = true;
                postContext = new ActionExecutedContext(context, ex);
                filter.OnExecuted(postContext);

                if (!postContext.ExceptionHandled)
                    throw;
            }
            if (!wasError)
                filter.OnExecuted(postContext);
            
            return postContext;
        }

        private static ResultExecutedContext InvokeActionResultFilter(IResultFilter filter, 
            ResultExecutionContext context, Func<ResultExecutedContext> continuation)
        {
            filter.OnResultExecuting(context);

            if (context.Cancel)
                return new ResultExecutedContext(context, context.Result, null);

            bool wasError = false;
            ResultExecutedContext postContext = null;
            try
            {
                postContext = continuation();
            }
            catch (ThreadAbortException)
            {
                // This type of exception occurs as a result of Response.Redirect(), but we special-case so that
                // the filters don't see this as an error.
                postContext = new ResultExecutedContext(context, context.Result, null);
                filter.OnResultExecuted(postContext);

                throw;
            }
            catch (Exception ex)
            {
                wasError = true;
                postContext = new ResultExecutedContext(context, context.Result, ex);
                filter.OnResultExecuted(postContext);
                
                if (!postContext.ExceptionHandled)
                    throw;
            }
            if (!wasError)
                filter.OnResultExecuted(postContext);
            
            return postContext;
        }

        protected static void ValidateRequest(HttpRequestBase request)
        {
            request.ValidateInput();
        }

        private static void ControllerAsFilter<TFilter>(ControllerBase controller, IList<TFilter> filters) 
            where TFilter : class
        {
            TFilter item = controller as TFilter;
            if (item != null)
                filters.Insert(0, item);
        }
        #endregion

        #region Instance Methods
        protected ControllerBase GetController()
        {
            Precondition.Require(_context, Error.ArgumentNull("context"));
            Precondition.Require(_context.Controller, Error.ArgumentNull("controller"));

            return _context.Controller;
        }

        protected virtual IModelBinder GetBinder(ParameterDescriptor parameter)
        {
            return parameter.Binding.Binder ?? Binders.GetBinder(parameter.Type);
        }

        protected virtual ControllerDescriptor GetControllerDescriptor(ControllerContext context)
        {
            Type type = GetController().GetType();
            return Descriptors.GetDescriptor(type);
        }

        protected virtual ActionDescriptor FindAction(ControllerContext context, 
            ControllerDescriptor descriptor, string actionName)
        {
            return descriptor.FindAction(context, actionName);
        }

        protected virtual ActionFilterInfo GetFilters(ControllerContext context, 
            ActionDescriptor descriptor)
        {
            ActionFilterInfo filters = descriptor.GetFilters();
            ControllerBase controller = GetController();

            ControllerAsFilter<IActionFilter>(controller, filters.ActionFilters);
            ControllerAsFilter<IResultFilter>(controller, filters.ResultFilters);
            ControllerAsFilter<IAuthorizationFilter>(controller, filters.AuthorizationFilters);
            ControllerAsFilter<IExceptionFilter>(controller, filters.ExceptionFilters);

            return filters;
        }

        protected virtual ActionResult CreateActionResult(ControllerContext context,
            ActionDescriptor descriptor, object returnValue)
        {
            if (returnValue == null)
                return new EmptyResult();

            if (returnValue is ActionResult)
                return (ActionResult)returnValue;

            return new ContentResult() {
                Content = Convert.ToString(returnValue, CultureInfo.InvariantCulture)
            };
        }

        protected virtual IDictionary<string, object> GetParameterValues(
            ControllerContext context, ActionDescriptor descriptor)
        {
            Precondition.Require(context, Error.ArgumentNull("context"));
            Precondition.Require(descriptor, Error.ArgumentNull("descriptor"));

            return descriptor.GetParameters().ToDictionary(
                p => p.Name, p => GetParameterValue(context, p), 
                StringComparer.OrdinalIgnoreCase);
        }

        protected virtual object GetParameterValue(ControllerContext context, 
            ParameterDescriptor parameter)
        {
            Precondition.Require(context, Error.ArgumentNull("context"));
            Precondition.Require(parameter, Error.ArgumentNull("parameter"));

            IModelBinder binder = GetBinder(parameter);
            BindingContext bc = new BindingContext(context, parameter.Type, 
                parameter.Binding.Name, parameter.Binding.Source, null, 
                parameter.Binding.GetMemberFilter(), GetController().Errors);
            bc.FallbackToEmptyPrefix = (String.Equals(parameter.Binding.Name, parameter.Name));

            return binder.Bind(bc) ?? parameter.Binding.DefaultValue;
        }

        protected virtual ActionExecutedContext InvokeActionFilters(
            ControllerContext context, ActionDescriptor action, 
            IList<IActionFilter> filters, IDictionary<string, object> parameters)
        {
            Precondition.Require(context, Error.ArgumentNull("context"));
            Precondition.Require(action, Error.ArgumentNull("action"));
            Precondition.Require(filters, Error.ArgumentNull("filters"));
            Precondition.Require(parameters, Error.ArgumentNull("parameters"));
            
            ActionExecutionContext exc = new ActionExecutionContext(context, action);
            Func<ActionExecutedContext> continuation = () =>
                new ActionExecutedContext(exc, null) { Result = InvokeActionMethod(context, action, parameters) };

            Func<ActionExecutedContext> thunk = filters.Reverse().Aggregate(continuation,
                (next, filter) => () => InvokeActionFilter(filter, exc, next));
            return thunk();
        }

        protected virtual ResultExecutedContext InvokeActionResultFilters(
            ControllerContext context, IList<IResultFilter> filters, ActionResult result)
        {
            Precondition.Require(context, Error.ArgumentNull("context"));
            Precondition.Require(result, Error.ArgumentNull("result"));
            Precondition.Require(filters, Error.ArgumentNull("filters"));

            ResultExecutionContext rec = new ResultExecutionContext(context, result);
            Func<ResultExecutedContext> continuation = () => {
                InvokeActionResult(context, result);
                return new ResultExecutedContext(context, rec.Result, null);
            };

            Func<ResultExecutedContext> thunk = filters.Reverse().Aggregate(continuation,
                (next, filter) => () => InvokeActionResultFilter(filter, rec, next));
            return thunk();
        }

        protected virtual AuthorizationContext InvokeAuthorizationFilters(
            ControllerContext context, ActionDescriptor descriptor,
            IEnumerable<IAuthorizationFilter> filters)
        {
            Precondition.Require(context, Error.ArgumentNull("context"));
            Precondition.Require(descriptor, Error.ArgumentNull("descriptor"));
            Precondition.Require(filters, Error.ArgumentNull("filters"));

            AuthorizationContext filterContext = new AuthorizationContext(context, descriptor);
            foreach (IAuthorizationFilter filter in filters)
            {
                filter.OnAuthorization(filterContext);
                if (filterContext.Cancel)
                    break;
            }
            return filterContext;
        }

        protected virtual ExceptionContext InvokeExceptionFilters(
            ControllerContext context, Exception exception, IList<IExceptionFilter> filters)
        {
            Precondition.Require(exception, Error.ArgumentNull("exception"));
            Precondition.Require(filters, Error.ArgumentNull("filters"));

            ExceptionContext filterContext = new ExceptionContext(context, exception);
            foreach (IExceptionFilter filter in filters)
                filter.OnException(filterContext);

            return filterContext;
        }

        public bool InvokeAction(ControllerContext context, string actionName)
        {
            return InvokeAction(context, actionName, new ValueDictionary());
        }

        public virtual bool InvokeAction(ControllerContext context, string actionName, 
            IDictionary<string, object> values)
        {
            Precondition.Require(context, Error.ArgumentNull("context"));
            Precondition.Require(!String.IsNullOrEmpty(actionName),
                Error.ArgumentNull("actionName"));

            ControllerDescriptor controller = GetControllerDescriptor(context);
            ActionDescriptor currentAction = FindAction(context, controller, actionName);

            if(currentAction == null)
                return false;

            ActionFilterInfo filters = GetFilters(context, currentAction);
            context.Parameters.Merge(GetParameterValues(context, currentAction))
                .Merge(values);

            try
            {
                AuthorizationContext authContext = InvokeAuthorizationFilters(
                    context, currentAction, filters.AuthorizationFilters);

                if (authContext.Cancel)
                {
                    InvokeActionResult(context, authContext.Result ?? EmptyResult.Instance);
                }
                else
                {
                    if (context.Controller.ValidateRequest)
                        ValidateRequest(context.Context.Request);

                    ActionExecutedContext resultContext = InvokeActionFilters(
                        context, currentAction, filters.ActionFilters, 
                        context.Parameters);

                    InvokeActionResultFilters(context, filters.ResultFilters, resultContext.Result);
                }
            }
            catch(Exception ex)
            {
                ExceptionContext exceptionContext = InvokeExceptionFilters(context, ex, filters.ExceptionFilters);
                if (!exceptionContext.Handled)
                    throw;

                InvokeActionResult(context, exceptionContext.Result);
            }
            return true;
        }

        public virtual ActionResult InvokeActionMethod(ControllerContext context,
            ActionDescriptor action, IDictionary<string, object> parameters)
        {
            Precondition.Require(context, Error.ArgumentNull("context"));
            Precondition.Require(action, Error.ArgumentNull("action"));
            Precondition.Require(parameters, Error.ArgumentNull("parameters"));

            object value = action.Execute(context, parameters);
            return CreateActionResult(context, action, value);
        }

        protected virtual void InvokeActionResult(ControllerContext context, ActionResult result)
        {
            Precondition.Require(context, Error.ArgumentNull("context"));
            Precondition.Require(result, Error.ArgumentNull("result"));

            result.Execute(context);
        }
        #endregion
    }
}

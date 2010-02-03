using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Routing;
using Radischevo.Wahha.Web.Text;

namespace Radischevo.Wahha.Web.Mvc.UI
{
    public class HtmlHelper : IHideObjectMembers
    {
        #region Nested Types
        private sealed class ComponentController : Controller
        {
            #region Constructors
            public ComponentController()
            {   }
            #endregion

            #region Instance Methods
            public void RenderAction(ControllerContext context, 
                string controllerName, string actionName, ValueDictionary arguments)
            {
                Precondition.Require(context, Error.ArgumentNull("context"));
                InitializeChildRequest(context);
                Include(controllerName, actionName, arguments);
            }

            public void RenderAction<TController>(ControllerContext context, 
                Expression<Action<TController>> action)
                where TController : Controller
            {
                Precondition.Require(action, Error.ArgumentNull("action"));
                Precondition.Require(context, Error.ArgumentNull("context"));
                InitializeChildRequest(context);
                Include<TController>(action);
            }            
            #endregion
        }
        #endregion

        #region Instance Fields
        private ViewContext _context;
        private HtmlControlHelper _controls;
        private TemplateHelper _templates;
        #endregion

        #region Constructors
        public HtmlHelper(ViewContext context)
        {
            Precondition.Require(context, Error.ArgumentNull("context"));
            _context = context;
            _controls = new HtmlControlHelper(_context);
            _templates = new TemplateHelper(this);
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets the current 
        /// <see cref="Radischevo.Wahha.Web.Mvc.ViewContext"/>.
        /// </summary>
        public ViewContext Context
        {
            get 
            {
                return _context;
            }
        }

        public TemplateHelper Templates
        {
            get
            {
                return _templates;
            }
        }

        /// <summary>
        /// Gets the <see cref="Radischevo.Wahha.Web.Mvc.HtmlControlHelper"/> instance 
        /// for use in rendering HTML controls within a view.
        /// </summary>
        public HtmlControlHelper Controls
        {
            get
            {
                return _controls;
            }
        }
        #endregion

        #region Default Helper Overrides
        protected void SetTemplateHelper(TemplateHelper helper)
        {
            Precondition.Require(helper, Error.ArgumentNull("helper"));
            _templates = helper;
        }

        protected void SetControlsHelper(HtmlControlHelper helper)
        {
            Precondition.Require(helper, Error.ArgumentNull("helper"));
            _controls = helper;
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Converts an object to a HTML-encoded string.
        /// </summary>
        /// <param name="obj">The object to encode.</param>
        public string Encode(object obj)
        {
            return Encode(Convert.ToString(obj, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Converts a string to a HTML-encoded string.
        /// </summary>
        /// <param name="text">The string to encode.</param>
        public string Encode(string text)
        {
            return HttpUtility.HtmlEncode(text);
        }

        /// <summary>
        /// Executes the specified <paramref name="ifTrue">action</paramref> 
        /// against the current context if the condition succees.
        /// </summary>
        /// <param name="condition">The condition to check.</param>
        /// <param name="ifTrue">The action to execute.</param>
        public void Choice(bool condition, Action ifTrue)
        {
            Choice(condition, ifTrue, null);
        }

        /// <summary>
        /// Executes the <paramref name="ifTrue"/> action 
        /// against the current context if the condition succees. 
        /// Otherwise, the <paramref name="ifFalse"/> action will be executed.
        /// </summary>
        /// <param name="condition">The condition to check.</param>
        /// <param name="ifTrue">The action to execute if the condition succees.</param>
        /// <param name="ifFalse">The action to execute if the condition fails.</param>
        public void Choice(bool condition, Action ifTrue, Action ifFalse)
        {
            if (condition)
            {
                if (ifTrue != null)
                    ifTrue();
            }
            else
            {
                if (ifFalse != null)
                    ifFalse();
            }
        }

        public void Choice<TValue>(Func<TValue> value, Func<TValue, bool> condition,
            Action<TValue> ifTrue)
        {
            Choice<TValue>(value(), condition, ifTrue, null);
        }

        public void Choice<TValue>(Func<TValue> value, Func<TValue, bool> condition,
            Action<TValue> ifTrue, Action<TValue> ifFalse)
        {
            Precondition.Require(value, Error.ArgumentNull("value"));
            Choice<TValue>(value(), condition, ifTrue, ifFalse);
        }

        public void Choice<TValue>(TValue value, Func<TValue, bool> condition,
            Action<TValue> ifTrue)
        {
            Choice<TValue>(value, condition, ifTrue, null);
        }

        public void Choice<TValue>(TValue value, Func<TValue, bool> condition, 
            Action<TValue> ifTrue, Action<TValue> ifFalse)
        {
            Precondition.Require(condition, Error.ArgumentNull("condition"));
            if (condition(value))
            {
                if (ifTrue != null)
                    ifTrue(value);
            }
            else
            {
                if (ifFalse != null)
                    ifFalse(value);
            }
        }
        #endregion

        #region Component Rendering
        /// <summary>
        /// Executes the specified action against 
        /// the current context and captures an output string.
        /// </summary>
        /// <param name="block">The action to execute</param>
        public string Block(Action block)
        {
            BlockRenderer br = 
                new BlockRenderer(_context.Context);

            return br.Capture(block);
        }

        /// <summary>
        /// Executes the specified action against 
        /// the current context and captures an output string.
        /// </summary>
        /// <param name="block">The action to execute</param>
        /// <param name="parameter">The parameter for the action.</param>
        public string Block<T>(Action<T> block, T parameter)
        {
            BlockRenderer br =
                new BlockRenderer(_context.Context);

            return br.Capture(() => block(parameter));
        }

        /// <summary>
        /// Executes the specified action against 
        /// the current context and captures an output string.
        /// </summary>
        /// <param name="block">The action to execute</param>
        /// <param name="arg1">The first argument of the action.</param>
        /// <param name="arg2">The second argument of the action.</param>
        public string Block<T1, T2>(Action<T1, T2> block, T1 arg1, T2 arg2)
        {
            BlockRenderer br =
                new BlockRenderer(_context.Context);

            return br.Capture(() => block(arg1, arg2));
        }

        /// <summary>
        /// Executes the specified action against 
        /// the current context and captures an output string.
        /// </summary>
        /// <param name="block">The action to execute</param>
        /// <param name="arg1">The first argument of the action.</param>
        /// <param name="arg2">The second argument of the action.</param>
        /// <param name="arg3">The third argument of the action.</param>
        public string Block<T1, T2, T3>(Action<T1, T2, T3> block, T1 arg1, T2 arg2, T3 arg3)
        {
            BlockRenderer br =
                new BlockRenderer(_context.Context);

            return br.Capture(() => block(arg1, arg2, arg3));
        }

        /// <summary>
        /// Executes the specified action against 
        /// the current context and captures an output string.
        /// </summary>
        /// <param name="block">The action to execute</param>
        /// <param name="arg1">The first argument of the action.</param>
        /// <param name="arg2">The second argument of the action.</param>
        /// <param name="arg3">The third argument of the action.</param>
        /// <param name="arg4">The fourth argument of the action.</param>
        public string Block<T1, T2, T3, T4>(Action<T1, T2, T3, T4> block, 
            T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            BlockRenderer br =
                new BlockRenderer(_context.Context);

            return br.Capture(() => block(arg1, arg2, arg3, arg4));
        }
        #endregion

        #region Component Rendering Methods
        /// <summary>
        /// Executes the specified action against the current context.
        /// </summary>
        /// <param name="controller">The name of the controller.</param>
        /// <param name="action">The name of the action.</param>
        public void Component(string controller, string action)
        {
            Component(controller, action, null);
        }

        /// <summary>
        /// Executes the specified action against the current context.
        /// </summary>
        /// <param name="controller">The name of the controller.</param>
        /// <param name="action">The name of the action.</param>
        /// <param name="arguments">The arguments of the action.</param>
        public void Component(string controller, string action, object arguments)
        {
            Component(controller, action, new ValueDictionary(arguments));
        }

        /// <summary>
        /// Executes the specified action against the current context.
        /// </summary>
        /// <param name="controller">The name of the controller.</param>
        /// <param name="action">The name of the action.</param>
        /// <param name="arguments">The arguments of the action.</param>
        public void Component(string controller, string action, ValueDictionary arguments)
        {
            Precondition.Require(!String.IsNullOrEmpty(controller), Error.ArgumentNull("controller"));
            Precondition.Require(!String.IsNullOrEmpty(action), Error.ArgumentNull("action"));

            if (arguments == null)
                arguments = new ValueDictionary();

            ComponentController component = new ComponentController();
            component.RenderAction(Context, controller, action, arguments);
        }

        /// <summary>
        /// Executes the specified action against the current context.
        /// </summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        /// <param name="action">The expression containing the action and its parameters.</param>
        public void Component<TController>(Expression<Action<TController>> action)
            where TController : Controller
        {
            ComponentController component = new ComponentController();
            component.RenderAction<TController>(Context, action);
        }

        /// <summary>
        /// Renders the <see cref="Radischevo.Wahha.Web.Mvc.UI.ViewUserControl">ViewUserControl</see> 
        /// of specified type against the current context and captures the output string.
        /// </summary>
        /// <typeparam name="TControl">The type of control to render.</typeparam>
        /// <param name="viewData">The view data supplied to the view control.</param>
        /// <param name="settings">The object, containing values for the control's properties.</param>
        public string Control<TControl>(ViewDataDictionary viewData, object settings)
            where TControl : ViewUserControl
        {
            return ViewHelper.RenderControl<TControl>(_context, viewData, settings);
        }

        /// <summary>
        /// Renders the <see cref="Radischevo.Wahha.Web.Mvc.UI.ViewUserControl">ViewUserControl</see> 
        /// with the specified virtual path against the current context and captures 
        /// the output string.
        /// </summary>
        /// <param name="virtualPath">The virtual path of the view control.</param>
        public string Control(string virtualPath)
        {
            return ViewHelper.RenderControl(virtualPath, _context);
        }

        /// <summary>
        /// Renders the <see cref="Radischevo.Wahha.Web.Mvc.UI.ViewUserControl">ViewUserControl</see> 
        /// with the specified virtual path against the current context and captures 
        /// the output string.
        /// </summary>
        /// <param name="virtualPath">The virtual path of the view control.</param>
        /// <param name="viewData">The view data supplied to the view control.</param>
        public string Control(string virtualPath, ViewDataDictionary viewData)
        {
            return ViewHelper.RenderControl(virtualPath, _context, viewData, null);
        }

        /// <summary>
        /// Renders the <see cref="Radischevo.Wahha.Web.Mvc.UI.ViewUserControl">ViewUserControl</see> 
        /// with the specified virtual path against the current context and captures 
        /// the output string.
        /// </summary>
        /// <param name="virtualPath">The virtual path of the view control.</param>
        /// <param name="viewData">The view data supplied to the view control.</param>
        /// <param name="settings">The object, containing values for the control's properties.</param>
        public string Control(string virtualPath, ViewDataDictionary viewData, object settings)
        {
            return ViewHelper.RenderControl(virtualPath, _context, viewData, settings);
        }

        /// <summary>
        /// Renders the <see cref="Radischevo.Wahha.Web.Mvc.UI.ViewPage">ViewPage</see> 
        /// of specified type against the current context and captures the output string.
        /// </summary>
        /// <typeparam name="TPage">The type of view page to render.</typeparam>
        /// <param name="viewData">The view data supplied to the view page.</param>
        /// <param name="settings">The object, containing values for the page properties.</param>
        public string Page<TPage>(ViewDataDictionary viewData, object settings)
            where TPage : ViewPage
        {
            return ViewHelper.RenderPage<TPage>(_context, viewData, settings);
        }

        /// <summary>
        /// Renders the <see cref="Radischevo.Wahha.Web.Mvc.UI.ViewPage">ViewPage</see> 
        /// with the specified virtual path against the current context and captures 
        /// the output string.
        /// </summary>
        /// <param name="virtualPath">The virtual path of the view page.</param>
        public string Page(string virtualPath)
        {
            return ViewHelper.RenderPage(virtualPath, _context);
        }

        /// <summary>
        /// Renders the <see cref="Radischevo.Wahha.Web.Mvc.UI.ViewPage">ViewPage</see> 
        /// with the specified virtual path against the current context and captures 
        /// the output string.
        /// </summary>
        /// <param name="virtualPath">The virtual path of the view page.</param>
        /// <param name="viewData">The view data supplied to the view page.</param>
        public string Page(string virtualPath, ViewDataDictionary viewData)
        {
            return ViewHelper.RenderPage(virtualPath, _context, viewData, null);
        }

        /// <summary>
        /// Renders the <see cref="Radischevo.Wahha.Web.Mvc.UI.ViewPage">ViewPage</see> 
        /// with the specified virtual path against the current context and captures 
        /// the output string.
        /// </summary>
        /// <param name="virtualPath">The virtual path of the view page.</param>
        /// <param name="viewData">The view data supplied to the view page.</param>
        /// <param name="settings">The object, containing values for the page properties.</param>
        public string Page(string virtualPath, ViewDataDictionary viewData, object settings)
        {
            return ViewHelper.RenderPage(virtualPath, _context, viewData, settings);
        }
        #endregion
    }

    public class HtmlHelper<TModel> : HtmlHelper
        where TModel : class
    {
        #region Instance Fields
        private TemplateHelper<TModel> _templates;
        private HtmlControlHelper<TModel> _controls;
        #endregion

        #region Constructors
        public HtmlHelper(ViewContext context)
            : base(context)
        {
            _controls = new HtmlControlHelper<TModel>(context);
            _templates = new TemplateHelper<TModel>(this);

            SetControlsHelper(_controls);
            SetTemplateHelper(_templates);
        }
        #endregion

        #region Instance Properties
        public new TemplateHelper<TModel> Templates
        {
            get
            {
                return _templates;
            }
        }

        /// <summary>
        /// Gets the <see cref="Radischevo.Wahha.Web.Mvc.HtmlControlHelper"/> instance 
        /// for use in rendering HTML controls within a view.
        /// </summary>
        public new HtmlControlHelper<TModel> Controls
        {
            get
            {
                return _controls;
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;
using Radischevo.Wahha.Web.Routing;

namespace Radischevo.Wahha.Web.Mvc
{
    public abstract class ActionContext
    {
        #region Instance Fields
        private ActionDescriptor _action;
        private ActionResult _result;
		private ControllerContext _context;
        #endregion

        #region Constructors
        protected ActionContext(ControllerContext context, ActionDescriptor action)
        {
			Precondition.Require(context, () => Error.ArgumentNull("context"));
            Precondition.Require(action, () => Error.ArgumentNull("action"));

            _action = action;
			_context = context;
        }
        #endregion

        #region Instance Properties
        public ActionDescriptor Action
        {
            get
            {
                return _action;
            }
            set
            {
                Precondition.Require(value, () => Error.ArgumentNull("value"));
                _action = value;
            }
        }

        public ActionResult Result
        {
            get
            {
                return _result;
            }
            set
            {
                _result = value;
            }
        }

		public ControllerContext Context
		{
			get
			{
				return _context;
			}
		}

		public HttpContextBase HttpContext
		{
			get
			{
				return _context.Context;
			}
		}
        #endregion

        #region Static Methods
        internal static ActionContext GetActionContext(ActionContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            return context;
        }
        #endregion
    }

    public class ActionExecutionContext : ActionContext
    {
        #region Instance Fields
        private bool _cancel;
        #endregion

        #region Constructors
        public ActionExecutionContext(ActionContext context)
            : this(ActionContext.GetActionContext(context).Context,
				ActionContext.GetActionContext(context).Action)
        {
        }

        public ActionExecutionContext(ControllerContext context, ActionDescriptor action)
            : base(context, action)
        {
        }
        #endregion

        #region Instance Properties
        public bool Cancel
        {
            get
            {
                return _cancel;
            }
            set
            {
                _cancel = value;
            }
        }
        #endregion
    }

    public class ActionExecutedContext : ActionContext
    {
        #region Instance Fields
        private Exception _exception;
        private bool _exceptionHandled;
        #endregion

        #region Constructors
        public ActionExecutedContext(ActionContext context, Exception exception)
            : base(ActionContext.GetActionContext(context).Context, 
				ActionContext.GetActionContext(context).Action)
        {
            _exception = exception;
        }

        #endregion

        #region Instance Properties
        public Exception Exception
        {
            get
            {
                return _exception;
            }
        }

        public bool ExceptionHandled
        {
            get
            {
                return _exceptionHandled;
            }
            set
            {
                _exceptionHandled = value;
            }
        }
        #endregion
    }
}

using System;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
    public abstract class ResultContext
    {
        #region Instance Fields
		private ControllerContext _context;
        private ActionResult _result;
        #endregion

        #region Constructors
        protected ResultContext(ControllerContext context, ActionResult result)
        {
			Precondition.Require(context, () => Error.ArgumentNull("context"));
            Precondition.Require(result, () => Error.ArgumentNull("error"));

			_context = context;
            _result = result;
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

		public HttpContextBase HttpContext
		{
			get
			{
				return _context.Context;
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
                Precondition.Require(value, () => Error.ArgumentNull("value"));
                _result = value;
            }
        }
        #endregion       

		#region Static Methods
		internal static ResultContext GetResultContext(ResultContext context)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));
			return context;
		}
		#endregion
    }

    public class ResultExecutionContext : ResultContext
    {
        #region Instance Fields
        private bool _cancel;
        #endregion

        #region Constructors
		public ResultExecutionContext(ResultContext context)
            : this(ResultContext.GetResultContext(context).Context,
				ResultContext.GetResultContext(context).Result)
        {
        }

        public ResultExecutionContext(ControllerContext context, ActionResult result)
            : base(context, result) 
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

    public class ResultExecutedContext : ResultContext
    {
        #region Instance Fields
        private Exception _exception;
        private bool _exceptionHandled;
        #endregion

        #region Constructors
		public ResultExecutedContext(ResultContext context, Exception exception)
            : this(ResultContext.GetResultContext(context).Context,
				ResultContext.GetResultContext(context).Result, exception)
        {
        }

        public ResultExecutedContext(ControllerContext context, 
            ActionResult result, Exception exception)
            : base(context, result)
        {
            _exception = exception;
        }
        #endregion

        #region Instance Properties
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

        public Exception Exception
        {
            get
            {
                return _exception;
            }
        }
        #endregion
    }
}

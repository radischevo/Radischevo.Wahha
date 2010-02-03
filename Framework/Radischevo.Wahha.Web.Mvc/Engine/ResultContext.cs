using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public class ResultContext : ControllerContext
    {
        #region Instance Fields
        private ActionResult _result;
        #endregion

        #region Constructors
        protected ResultContext(ControllerContext context, ActionResult result)
            : base(context) 
        {
            Precondition.Require(result, Error.ArgumentNull("error"));
            _result = result;
        }
        #endregion

        #region Instance Properties
        public ActionResult Result
        {
            get
            {
                return _result;
            }
            set
            {
                Precondition.Require(value, Error.ArgumentNull("value"));
                _result = value;
            }
        }
        #endregion       
    }

    public class ResultExecutionContext : ResultContext
    {
        #region Instance Fields
        private bool _cancel;
        #endregion

        #region Constructors
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

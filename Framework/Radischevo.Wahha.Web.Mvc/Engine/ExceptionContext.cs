using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public class ExceptionContext : ControllerContext
    {
        #region Instance Fields
        private ActionResult _result;
        private Exception _exception;
        private bool _handled;
        #endregion

        #region Constructors
        public ExceptionContext(ControllerContext context, Exception exception)
            : base(context)
        {
            Precondition.Require(exception, Error.ArgumentNull("exception"));
            _exception = exception;
            _result = EmptyResult.Instance;
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

        public bool Handled
        {
            get
            {
                return _handled;
            }
            set
            {
                _handled = value;
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
                if(value != null)
                    _result = value;
            }
        }
        #endregion
    }
}

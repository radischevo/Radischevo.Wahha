using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public class ErrorHandlerInfo
    {
        #region Instance Fields
        private Exception _exception;
        #endregion

        #region Constructors
        internal ErrorHandlerInfo(Exception exception)
        {
            Precondition.Require(exception, () => Error.ArgumentNull("exception"));
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
        #endregion
    }
}

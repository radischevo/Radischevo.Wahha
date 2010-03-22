using System;
using System.Net;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
    public abstract class HttpStatusResult : EmptyResult
    {
        #region Instance Fields
        private HttpStatusCode _statusCode;
        private string _message;
        private bool _throwError;
        #endregion

        #region Constructors
        protected HttpStatusResult(HttpStatusCode statusCode,
            string message, bool throwError)
        {
            _statusCode = statusCode;
            _message = message;
            _throwError = throwError;
        }
        #endregion

        #region Instance Methods
        public override void Execute(ControllerContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            HttpResponseBase response = context.Context.Response;

            response.Clear();

            if (_throwError)
            {
                throw new HttpException((int)_statusCode, _message ?? String.Empty);
            }
            else
            {
                response.StatusCode = (int)_statusCode;
                response.StatusDescription = _message ?? String.Empty;
				response.TrySkipIisCustomErrors = true;
            }
        }
        #endregion
    }
}

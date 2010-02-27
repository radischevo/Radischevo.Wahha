using System;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;
using Radischevo.Wahha.Web.Scripting.Serialization;

namespace Radischevo.Wahha.Web.Mvc.Validation
{
    public class RemoteValidationResult : ActionResult
    {
        #region Static Fields
		private static RemoteValidationResult _success = 
            new RemoteValidationResult("true");
	    #endregion

        #region Instance Fields
        private string _message;
        #endregion

        #region Constructors
        private RemoteValidationResult(string message)
            : base()
        {
            _message = message;
        }
        #endregion

        #region Instance Properties
        public string Message
        {
            get
            {
                return _message;
            }
        }
        #endregion

        #region Static Methods
        public static RemoteValidationResult Success()
        {
            return _success;
        }

        public static RemoteValidationResult Failure()
        {
            return Failure(null);
        }

        public static RemoteValidationResult Failure(string errorMessage)
        {
            if (String.IsNullOrEmpty(errorMessage))
                return new RemoteValidationResult("false");

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return new RemoteValidationResult(serializer.Serialize(errorMessage));
        }
        #endregion

        #region Instance Methods
        public override void Execute(ControllerContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            HttpResponseBase response = context.Context.Response;

            response.ContentType = "application/json";
            response.Write(_message ?? "false");
        }
        #endregion
    }
}

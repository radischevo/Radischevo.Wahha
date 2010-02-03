using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, 
        Inherited = true, AllowMultiple = false)]
    public sealed class HttpContentTypeAttribute : ActionFilterAttribute
    {
        #region Constants
        private const string _storeKey = "Radischevo.Wahha.Web.Mvc.HttpContentTypeAttribute.HeaderAdded";
        #endregion

        #region Instance Fields
        private string _contentType;
        #endregion

        #region Constructors
        public HttpContentTypeAttribute(string contentType)
        {
            Precondition.Require(!String.IsNullOrEmpty(contentType), 
                Error.ArgumentNull("contentType"));
            _contentType = contentType;
        }
        #endregion

        #region Instance Properties
        public string ContentType
        {
            get
            {
                return _contentType;
            }
        }
        #endregion

        #region Instance Methods
        public override void OnExecuting(ActionExecutionContext context)
        {
            Precondition.Require(context, Error.ArgumentNull("context"));
            if (context.Context.Items[_storeKey] == null)
            {
                context.Context.Response.ContentType = _contentType;
                context.Context.Items[_storeKey] = true;
            }
        }
        #endregion
    }
}

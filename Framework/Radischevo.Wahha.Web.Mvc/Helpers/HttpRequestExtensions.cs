using System;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
    public static class HttpRequestExtensions
    {
        #region Constants
        private const string AjaxHeaderName = "X-Requested-With";
        private const string AjaxHeaderValue = "XMLHttpRequest";
	    #endregion

        #region Extension Methods
        /// <summary>
        /// Gets a value indicating whether the 
        /// current resource is requested through 
        /// the XmlHttpRequest.
        /// </summary>
        public static bool IsAjaxRequest(this HttpRequestBase request)
        {
            Precondition.Require(request, Error.ArgumentNull("request"));
            if (request.Headers == null)
                return false;

            return String.Equals(request.Headers.GetValue<string>(AjaxHeaderName),
                AjaxHeaderValue, StringComparison.Ordinal);
        }
        #endregion
    }
}

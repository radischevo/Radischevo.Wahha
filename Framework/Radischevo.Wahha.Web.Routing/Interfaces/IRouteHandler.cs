using System;
using System.Web;

namespace Radischevo.Wahha.Web.Routing
{
    /// <summary>
    /// Defines the contract for 
    /// handler that implements 
    /// the routing of incoming HTTP requests
    /// </summary>
    public interface IRouteHandler
    {
        #region Instance Methods
        /// <summary>
        /// Gets the appropriate HTTP handler 
        /// for the current request
        /// </summary>
        /// <param name="requestContext">The current request context</param>
        IHttpHandler GetHttpHandler(RequestContext requestContext);
        #endregion
    }
}

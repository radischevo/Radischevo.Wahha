using System;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Routing
{
    /// <summary>
    /// Encapsulates the routing information 
    /// about a single HTTP request.
    /// </summary>
    public class RequestContext
    {
        #region Instance Fields
        private HttpContextBase _context;
        private RouteData _routeData;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="RequestContext"/> class
        /// </summary>
        /// <param name="context">An <see cref="HttpContextBase"/> of the current request</param>
        /// <param name="routeData">The route parameter dictionary</param>
        public RequestContext(HttpContextBase context, RouteData routeData)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            Precondition.Require(routeData, () => Error.ArgumentNull("routeData"));

            _context = context;
            _routeData = routeData;
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets the context for the 
        /// current request
        /// </summary>
        public HttpContextBase Context
        {
            get
            {
                return _context;
            }
        }

        /// <summary>
        /// Gets the parameter 
        /// dictionary for the current route
        /// </summary>
        public RouteData RouteData
        {
            get
            {
                return _routeData;
            }
        }
        #endregion
    }
}

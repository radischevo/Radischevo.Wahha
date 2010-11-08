using System;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Routing
{
    /// <summary>
    /// Provides the base class for 
    /// URL routing
    /// </summary>
    public abstract class RouteBase
    {
        #region Abstract Methods
        /// <summary>
        /// Gets the <see cref="RouteData"/> for 
        /// the current <see cref="HttpContextBase"/>.
        /// </summary>
        /// <param name="context">The <see cref="HttpContextBase"/> 
        /// containing the incoming request.</param>
		/// <param name="variables">The <see cref="ValueDictionary"/> 
		/// containing the route variable values.</param>
        public abstract RouteData GetRouteData(HttpContextBase context, ValueDictionary variables);
        
        /// <summary>
        /// Gets the <see cref="VirtualPathData"/> for 
        /// the current instance.
        /// </summary>
        /// <param name="value">The <see cref="ValueDictionary"/> 
        /// containing the route parameter values.</param>
		/// <param name="variables">The <see cref="ValueDictionary"/> 
		/// containing the route variable values.</param>
        public abstract VirtualPathData GetVirtualPath(RequestContext context, 
			ValueDictionary values, ValueDictionary variables);
        #endregion
    }
}
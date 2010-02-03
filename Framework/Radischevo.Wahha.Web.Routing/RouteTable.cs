using System;

using Radischevo.Wahha.Web.Routing.Providers;

namespace Radischevo.Wahha.Web.Routing
{
    /// <summary>
    /// Provides the URL routing table
    /// </summary>
    public static class RouteTable
    {
        #region Static Fields
        private static RouteCollection _routes = new RouteCollection();
        private static readonly IRouteTableProvider _defaultProvider 
            = new DefaultRouteTableProvider();
        #endregion

        #region Static Properties
        /// <summary>
        /// Gets the collection of 
        /// registered routes
        /// </summary>
        public static RouteCollection Routes
        {
            get
            {
                return _routes;
            }
        }

        /// <summary>
        /// Gets the instance of <see cref="IRouteTableProvider"/> 
        /// which is used to persist the routing configuration
        /// </summary>
        public static IRouteTableProvider Provider
        {
            get
            {
                IRouteTableProvider provider = 
                    Configuration.Configuration.Instance.Provider;

                if (provider == null)
                    return _defaultProvider;

                return provider;
            }
        }
        #endregion
    }
}

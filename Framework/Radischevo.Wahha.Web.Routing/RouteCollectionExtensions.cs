using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Routing
{
    public static class RouteCollectionExtensions
    {
        #region Extension Methods
        public static void IgnoreRoute(this RouteCollection routes, string url)
        {
            Precondition.Require(routes, () => Error.ArgumentNull("routes"));
            Precondition.Require(url, () => Error.ArgumentNull("url"));

            IgnoredRoute route = new IgnoredRoute(url);
            routes.Add(route);
        }

        public static void IgnoreRoute(this RouteCollection routes, string url,
            params IRouteConstraint[] constraints)
        {
            Precondition.Require(routes, () => Error.ArgumentNull("routes"));
            Precondition.Require(constraints, () => Error.ArgumentNull("constraints"));
            Precondition.Require(url, () => Error.ArgumentNull("url"));

            IgnoredRoute route = new IgnoredRoute(url);
            route.Constraints.AddRange(constraints);

            routes.Add(route);
        }

        public static Route MapPageRoute(this RouteCollection routes, string key,
            string url, string virtualPath)
        {
            Precondition.Require(routes, () => Error.ArgumentNull("routes"));
            Precondition.Require(url, () => Error.ArgumentNull("url"));

            Route route = new Route(url, new WebFormRoutingHandler(virtualPath));
            routes.Add(key, route);

            return route;
        }

        public static Route MapPageRoute(this RouteCollection routes, string key,
            string url, string virtualPath, object defaults)
        {
            return MapPageRoute(routes, key, url, virtualPath, defaults, null);
        }

        public static Route MapPageRoute(this RouteCollection routes, string key,
            string url, string virtualPath, object defaults, 
            params IRouteConstraint[] constraints)
        {
            Precondition.Require(routes, () => Error.ArgumentNull("routes"));
            Precondition.Require(url, () => Error.ArgumentNull("url"));

            Route route = (constraints == null) ?
                new Route(url, new ValueDictionary(defaults), 
                    constraints, new WebFormRoutingHandler(virtualPath)) :
                new Route(url, new ValueDictionary(defaults), 
                    new WebFormRoutingHandler(virtualPath));

            routes.Add(key, route);
            return route;
        }
        #endregion
    }
}

using System;
using System.Linq.Expressions;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Routing;

namespace Radischevo.Wahha.Web.Mvc
{
    public static class MvcRouteCollectionExtensions
    {
        #region Static Extension Methods
        public static Route MapRoute(this RouteCollection routes, string key,
            string url)
        {
            Precondition.Require(routes, Error.ArgumentNull("routes"));
            Precondition.Require(url, Error.ArgumentNull("url"));

            Route route = new Route(url, new MvcRouteHandler());

            routes.Add(key, route);
            return route;
        }

        public static Route MapRoute(this RouteCollection routes, string key,
            string url, object defaults)
        {
            return MapRoute(routes, key, url, defaults, null, null);
        }

		public static Route MapRoute(this RouteCollection routes, string key,
			string url, object defaults, object tokens)
		{
			return MapRoute(routes, key, url, defaults, tokens, null);
		}

        public static Route MapRoute(this RouteCollection routes, string key, 
            string url, object defaults, object tokens, 
			params IRouteConstraint[] constraints)
        {
            Precondition.Require(routes, Error.ArgumentNull("routes"));
            Precondition.Require(url, Error.ArgumentNull("url"));

			constraints = constraints ?? new IRouteConstraint[0];

            Route route = new Route(url, new ValueDictionary(defaults), constraints, 
				new ValueDictionary(tokens), new MvcRouteHandler());
            
            routes.Add(key, route);
            return route;
        }

        public static Route MapRoute<TController>(this RouteCollection routes,
            string key, string url, Expression<Action<TController>> action)
            where TController : IController
        {
            return MapRoute<TController>(routes, key, url, action, null, null);
        }

        public static Route MapRoute<TController>(this RouteCollection routes, 
            string key, string url, Expression<Action<TController>> action, 
            object tokens)
            where TController : IController
        {
			return MapRoute<TController>(routes, key, url, action, tokens, null);
        }

		public static Route MapRoute<TController>(this RouteCollection routes,
			string key, string url, Expression<Action<TController>> action, 
			object tokens, params IRouteConstraint[] constraints)
			where TController : IController
		{
			Precondition.Require(routes, Error.ArgumentNull("routes"));
			Precondition.Require(url, Error.ArgumentNull("url"));

			MethodCallExpression mexp = (action.Body as MethodCallExpression);
			if (mexp == null)
				throw Error.ExpressionMustBeAMethodCall("action");

			if (mexp.Object != action.Parameters[0])
				throw Error.MethodCallMustTargetLambdaArgument("action");

			string actionName = ActionMethodSelector.GetNameOrAlias(mexp.Method);
			string controllerName = typeof(TController).Name;

			ValueDictionary defaults = LinqHelper.ExtractArgumentsToDictionary(mexp);
			if (defaults == null)
				defaults = new ValueDictionary();

			defaults["controller"] = controllerName;
			defaults["action"] = actionName;

			constraints = constraints ?? new IRouteConstraint[0];

			Route route = new Route(url, defaults, constraints, 
				new ValueDictionary(tokens), new MvcRouteHandler());

			routes.Add(key, route);
			return route;
		}
        #endregion
    }
}

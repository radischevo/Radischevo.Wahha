using System;
using System.Linq.Expressions;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Routing;

namespace Radischevo.Wahha.Web.Mvc.Async
{
	public static class MvcAsyncRouteCollectionExtensions
	{
		#region Extension Methods
		public static Route MapAsyncRoute(this RouteCollection routes, string key,
			string url)
		{
			Precondition.Require(routes, Error.ArgumentNull("routes"));
			Precondition.Require(url, Error.ArgumentNull("url"));

			Route route = new Route(url, new MvcAsyncRouteHandler());

			routes.Add(key, route);
			return route;
		}

		public static Route MapAsyncRoute(this RouteCollection routes, string key,
			string url, object defaults)
		{
			return MapAsyncRoute(routes, key, url, defaults, null);
		}

		public static Route MapAsyncRoute(this RouteCollection routes, string key,
			string url, object defaults, params IRouteConstraint[] constraints)
		{
			Precondition.Require(routes, Error.ArgumentNull("routes"));
			Precondition.Require(url, Error.ArgumentNull("url"));

			Route route = (constraints == null) ?
				new Route(url, new ValueDictionary(defaults), constraints, new MvcAsyncRouteHandler()) :
				new Route(url, new ValueDictionary(defaults), new MvcAsyncRouteHandler());

			routes.Add(key, route);
			return route;
		}

		public static Route MapAsyncRoute<TController>(this RouteCollection routes,
			string key, string url, Expression<Action<TController>> action)
			where TController : IAsyncController
		{
			return MapAsyncRoute<TController>(routes, key, url, action, null);
		}

		public static Route MapAsyncRoute<TController>(this RouteCollection routes,
			string key, string url, Expression<Action<TController>> action,
			params IRouteConstraint[] constraints)
			where TController : IAsyncController
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

			Route route = (constraints == null) ?
				new Route(url, defaults, constraints, new MvcAsyncRouteHandler()) :
				new Route(url, defaults, new MvcAsyncRouteHandler());

			routes.Add(key, route);
			return route;
		}
		#endregion
	}
}

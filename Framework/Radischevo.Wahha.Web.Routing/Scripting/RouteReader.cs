using System;
using System.Collections.Generic;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Routing.Scripting
{
	public class RouteReader
	{
		#region Constants
		private const string _routeNameKey = "ScriptName";
		private const string _ignoreKey = "ScriptIgnore";
		#endregion

		#region Instance Fields
		private List<IRouteInterceptor> _interceptors;
		#endregion

		#region Constructors
		public RouteReader()
			: this(null)
		{
		}

		public RouteReader(IEnumerable<IRouteInterceptor> interceptors)
		{
			_interceptors = new List<IRouteInterceptor>(interceptors ?? 
				Enumerable.Empty<IRouteInterceptor>());
		}
		#endregion

		#region Instance Properties
		public ICollection<IRouteInterceptor> Interceptors
		{
			get
			{
				return _interceptors;
			}
		}
		#endregion

		#region Instance Methods
		protected virtual bool IsIgnored(Route route)
		{
			if (route is IgnoredRoute)
				return true;

			return route.Tokens.GetValue<bool>(_ignoreKey);
		}

		protected virtual string DetermineName(Route route)
		{
			return route.Tokens.GetValue<string>(_routeNameKey);
		}

		/// <summary>
		/// Gets the route definition from the provided route.
		/// </summary>
		/// <param name="name">The name of the route.</param>
		/// <param name="route">The route instance to create definition for.</param>
		public RouteDefinition Read(Route route)
		{
			string name = DetermineName(route);
			if (IsIgnored(route) || String.IsNullOrEmpty(name))
				return null;

			RouteDefinition definition = new RouteDefinition(name, route);
			foreach (IRouteInterceptor interceptor in Interceptors)
			{
				if (!interceptor.Intercept(definition))
					return null;
			}
			return definition;
		}
		#endregion
	}
}

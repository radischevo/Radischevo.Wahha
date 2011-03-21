using System;

namespace Radischevo.Wahha.Web.Routing.Scripting
{
	public interface IRouteInterceptor
	{
		#region Instance Methods
		/// <summary>
		/// Intercepts the route definition.
		/// </summary>
		/// <param name="definition">Route definition to manipulate.</param>
		/// <returns>Value indicating whether to add the 
		/// route after the interception.</returns>
		bool Intercept(RouteDefinition route);
		#endregion
	}
}

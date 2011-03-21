using System;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Routing.Scripting
{
	/// <summary>
	/// Route interceptor that adds regex constraints 
	/// to the definition if present.
	/// </summary>
	public class RegexConstraintReader : IRouteInterceptor
	{
		#region Constructors
		public RegexConstraintReader()
		{
		}
		#endregion

		#region Instance Methods
		/// <summary>
		/// Intercepts the route definition adding regex constraints 
		/// to its parameter values.
		/// </summary>
		/// <param name="definition">Route definition to manipulate.</param>
		public bool Intercept(RouteDefinition route)
		{
			Precondition.Require(route, () => 
				Error.ArgumentNull("route"));

			foreach (RegexConstraint constraint in route.Route
				.Constraints.OfType<RegexConstraint>())
			{
				route.Constraints.Add(new RegexConstraintDefinition(constraint));
			}
			return true;
		}
		#endregion
	}
}

using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Web.Routing.Scripting
{
	/// <summary>
	/// Provides a contract for the 
	/// script route table provider.
	/// </summary>
	public interface IScriptRouteTableProvider
	{
		#region Instance Methods
		IEnumerable<RouteDefinition> GetRouteTable(RouteCollection routes);
		#endregion
	}
}

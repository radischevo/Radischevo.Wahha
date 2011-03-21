using System;
using System.Collections.Generic;
using System.Linq;

namespace Radischevo.Wahha.Web.Routing.Scripting
{
	public class ScriptRouteTableProvider : IScriptRouteTableProvider
	{
		#region Constructors
		public ScriptRouteTableProvider()
		{
		}
		#endregion

		#region Instance Methods
		protected virtual RouteReader CreateReader(IEnumerable<IRouteInterceptor> interceptors)
		{
			return new RouteReader(interceptors);
		}

		public IEnumerable<RouteDefinition> GetRouteTable(RouteCollection routes)
		{
			List<RouteDefinition> collection = new List<RouteDefinition>();
			RouteReader reader = CreateReader(new IRouteInterceptor[] {
				new RegexConstraintReader(), 
				new RouteVariableAssigner(routes.Variables)
			});

			foreach (Route route in routes.OfType<Route>())
			{
				RouteDefinition definition = reader.Read(route);
				if (definition != null)
					collection.Add(definition);
			}
			return collection;
		}
		#endregion
	}
}

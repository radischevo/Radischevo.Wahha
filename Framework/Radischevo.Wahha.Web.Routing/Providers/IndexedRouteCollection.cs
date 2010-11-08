using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Web.Routing
{
	public class IndexedRouteCollection : Dictionary<string, RouteBase>
	{
		#region Constructors
		public IndexedRouteCollection()
			: base(StringComparer.OrdinalIgnoreCase)
		{
		}
		#endregion
	}
}

using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Routing
{
	public class RouteTableProviderResult
	{
		#region Instance Fields
		private ValueDictionary _variables;
		private IndexedRouteCollection _routes;
		#endregion

		#region Constructors
		public RouteTableProviderResult()
		{
			_variables = new ValueDictionary();
			_routes = new IndexedRouteCollection();
		}
		#endregion

		#region Instance Properties
		public ValueDictionary Variables
		{
			get
			{
				return _variables;
			}
		}

		public IndexedRouteCollection Routes
		{
			get
			{
				return _routes;
			}
		}
		#endregion
	}
}
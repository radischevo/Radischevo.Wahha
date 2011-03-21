using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Routing.Scripting
{
	public class RouteDefinition
	{
		#region Instance Fields
		private Route _route;
		private string _name;
		private string _url;
		private ValueDictionary _defaults;
		private List<RouteConstraintDefinition> _constraints;
		#endregion

		#region Constructors
		public RouteDefinition(string name, Route route)
		{
			Precondition.Defined(name, () => Error.ArgumentNull("name"));
			Precondition.Require(route, () => Error.ArgumentNull("route"));

			_name = name;
			_route = route;
			_url = route.Url;

			_defaults = CreateDefaults(route.Defaults);
			_constraints = new List<RouteConstraintDefinition>();
		}
		#endregion

		#region Instance Properties
		public Route Route
		{
			get
			{
				return _route;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public string Url
		{
			get
			{
				return _url;
			}
			set
			{
				// validate newly assigned value
				RouteParser.Parse(value);
				_url = value;
			}
		}

		public ValueDictionary Defaults
		{
			get
			{
				return _defaults;
			}
		}

		public ICollection<RouteConstraintDefinition> Constraints
		{
			get
			{
				return _constraints;
			}
		}
		#endregion

		#region Static Methods
		private static ValueDictionary CreateDefaults(IValueSet defaults)
		{
			ValueDictionary values = new ValueDictionary(defaults);
			values.Remove("controller");
			values.Remove("action");

			return values;
		}
		#endregion
	}
}

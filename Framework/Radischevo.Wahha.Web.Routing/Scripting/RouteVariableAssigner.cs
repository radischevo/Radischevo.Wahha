using System;
using System.Text.RegularExpressions;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Routing.Scripting
{
	public class RouteVariableAssigner : IRouteInterceptor
	{
		#region Static Fields
		private static Regex _variablePattern = new Regex(@"([^\[])?\[(?'name'([^\[\]]+))\]([^\]])?", 
			RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
		#endregion

		#region Instance Fields
		private ValueDictionary _variables;
		#endregion

		#region Constructors
		public RouteVariableAssigner(IValueSet variables)
		{
			Precondition.Require(variables, () => 
				Error.ArgumentNull("variables"));

			_variables = new ValueDictionary(variables);
		}
		#endregion

		#region Instance Methods
		public bool Intercept(RouteDefinition route)
		{
			Precondition.Require(route, () => Error.ArgumentNull("route"));
			MatchCollection matches = _variablePattern.Matches(route.Url);
			string url = route.Url;

			foreach (Match match in matches)
			{
				string name = match.Groups["name"].Value;
				object value;

				if (!_variables.TryGetValue(name, out value))
					throw Error.UndefinedRouteVariable(name);

				url = url.Replace(String.Format("[{0}]", name), Convert.ToString(value));
			}
			route.Url = url;
			return true;
		}
		#endregion
	}
}

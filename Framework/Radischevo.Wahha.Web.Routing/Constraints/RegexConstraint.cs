using System;
using System.Text.RegularExpressions;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Routing
{
    public class RegexConstraint : IRouteConstraint
    {
        #region Constants
		private const RegexOptions DEFAULT_OPTIONS = RegexOptions.Singleline | 
            RegexOptions.Compiled | RegexOptions.CultureInvariant; 
	    #endregion

        #region Instance Fields
        private string _parameterName;
        private Regex _pattern;
        #endregion

        #region Constructors
        public RegexConstraint(string parameterName, string pattern)
            : this(parameterName, pattern, DEFAULT_OPTIONS)
        {
        }

        public RegexConstraint(string parameterName, string pattern, RegexOptions options)
        {
            Precondition.Defined(parameterName, () =>Error.ArgumentNull("parameterName"));

            _parameterName = parameterName;
            _pattern = new Regex(NormalizePattern(pattern), options);
        }
        #endregion

        #region Instance Properties
        public string ParameterName
        {
            get
            {
                return _parameterName;
            }
        }
        #endregion

        #region Static Methods
        private static string NormalizePattern(string pattern)
        {
            if (String.IsNullOrEmpty(pattern))
                return "^(.*)$";

            return String.Concat("^(", pattern.TrimStart('^').TrimEnd('$'), ")$");
        }
        #endregion

        #region Instance Methods
        protected virtual bool Match(HttpContextBase context, Route route,
            ValueDictionary values, RouteDirection direction)
        {
            Precondition.Require(values, () => Error.ArgumentNull("values"));

            string value = values.GetValue<string>(_parameterName) ?? String.Empty;
            return _pattern.IsMatch(value);
        }
        #endregion

        #region IRouteConstraint Members
        bool IRouteConstraint.Match(HttpContextBase context, Route route,
            ValueDictionary values, RouteDirection direction)
        {
            return Match(context, route, values, direction);
        }
        #endregion
    }
}

using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, 
        AllowMultiple = false, Inherited = true)]
    public sealed class ApplyCultureAttribute : ActionFilterAttribute
    {
        #region Instance Fields
        private string _parameterName;
        private ParameterSource _source;
        #endregion

        #region Constructors
        public ApplyCultureAttribute(string parameterName)
        {
			Precondition.Defined(parameterName, 
				() => Error.ArgumentNull("parameterName"));

            _parameterName = parameterName;
            _source = ParameterSource.Header;
        }
        #endregion

        #region Instance Properties
        public string ParameterName
        {
            get
            {
                return _parameterName;
            }
            set
            {
                Precondition.Defined(value, 
					() => Error.ArgumentNull("value"));
                _parameterName = value;
            }
        }

        public ParameterSource Source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
            }
        }
        #endregion

        #region Static Methods
        private string GetCultureCode(ActionContext context)
        {
            HttpParameters parameters = context.Context.Request.Parameters;

            if ((_source & ParameterSource.Url) == ParameterSource.Url &&
                context.RouteData.Values.ContainsKey(_parameterName))
                return context.RouteData.Values.GetValue<string>(_parameterName);

            if ((_source & ParameterSource.Form) == ParameterSource.Form &&
                parameters.Form.Keys.Any(k => k.Equals(_parameterName, StringComparison.OrdinalIgnoreCase)))
                return parameters.Form.GetValue<string>(_parameterName);

            if ((_source & ParameterSource.QueryString) == ParameterSource.QueryString &&
                parameters.QueryString.Keys.Any(k => k.Equals(_parameterName, StringComparison.OrdinalIgnoreCase)))
                return parameters.QueryString.GetValue<string>(_parameterName);

            if ((_source & ParameterSource.Session) == ParameterSource.Session &&
                context.Context.Session != null && context.Context.Session[_parameterName] != null)
                return context.Context.Session[_parameterName].ToString();

            if ((_source & ParameterSource.Cookie) == ParameterSource.Cookie &&
                parameters.Cookies.Keys.Any(k => k.Equals(_parameterName, StringComparison.OrdinalIgnoreCase)))
                return parameters.Cookies.GetValue<string>(_parameterName);

            if ((_source & ParameterSource.Header) == ParameterSource.Header)
                return context.Context.Request.UserLanguages.FirstOrDefault() ?? String.Empty;
            
            return String.Empty;
        }
        #endregion

        #region Instance Methods
        public override void OnExecuting(ActionExecutionContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));

            string cultureCode = GetCultureCode(context);
            if (!String.IsNullOrEmpty(cultureCode))
            {
                // remove the qualifier value
                cultureCode = cultureCode.Split(new char[] { ';' }, 
                    StringSplitOptions.RemoveEmptyEntries)[0];

                try
                {
                    CultureInfo culture; int lcid;

                    culture = (int.TryParse(cultureCode, NumberStyles.Integer,
                        CultureInfo.InvariantCulture, out lcid)) ?
                            CultureInfo.GetCultureInfo(lcid) :
                            CultureInfo.CreateSpecificCulture(cultureCode);

                    Thread.CurrentThread.CurrentCulture = culture;
                    Thread.CurrentThread.CurrentUICulture = culture;
                }
                catch (ArgumentException)
                { }
                catch (NotSupportedException)
                { }
            }
        }
        #endregion
    }
}

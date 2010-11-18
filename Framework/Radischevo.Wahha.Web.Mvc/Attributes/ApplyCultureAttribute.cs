using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Mvc.Configurations;

namespace Radischevo.Wahha.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, 
        AllowMultiple = false, Inherited = true)]
    public sealed class ApplyCultureAttribute : ActionFilterAttribute
    {
        #region Instance Fields
        private string _parameterName;
        private string _source;
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

        public string Source
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

        #region Instance Methods
		private string GetCultureCode(ActionContext context)
		{
			IValueProvider provider = Configuration.Instance.Models
				.ValueProviders.GetProvider(context.Context, 
				ParameterSource.FromString(Source));

			ValueProviderResult result = provider.GetValue(_parameterName);
			return (result == null) ? null : result.GetValue<string>();
		}

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

using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Routing
{
    internal sealed class ParameterSubsegment : PathSubsegment
    {
        #region Instance Fields
        private string _parameterName;
        private bool _isCatchAll;
        #endregion

        #region Constructors
        public ParameterSubsegment(string parameterName)
        {
            Precondition.Require(parameterName, () => Error.ArgumentNull("parameterName"));

            _isCatchAll = (parameterName.StartsWith("*", StringComparison.Ordinal));
            _parameterName = (_isCatchAll) ? parameterName.Substring(1) : parameterName;
        }
        #endregion

        #region Instance Properties
        public bool IsCatchAll
        {
            get 
            {
                return _isCatchAll;
            }
        }

        public string ParameterName
        {
            get
            {
                return _parameterName;
            }
        }
        #endregion

        #region Instance Methods
        public override string ToString()
        {
            return String.Concat(RouteParser.ParameterStart, (_isCatchAll) ? 
                "*" : "", _parameterName, RouteParser.ParameterEnd);
        }
        #endregion
    }
}

using System;
using System.Globalization;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Routing
{
    internal sealed class VariableSubsegment : LiteralSubsegment
    {
        #region Instance Fields
        private string _variableName;
        private object _value;
        #endregion

        #region Constructors
        public VariableSubsegment(string variableName)
			: base("variable")
        {
            Precondition.Require(variableName, () => Error.ArgumentNull("variableName"));
            _variableName = variableName;
        }
        #endregion

        #region Instance Properties
        public object Value
        {
            get 
            {
                return _value;
            }
			set
			{
				_value = value;
			}
        }

        public string VariableName
        {
            get
            {
                return _variableName;
            }
        }

		public override string Literal
		{
			get
			{
				return Convert.ToString(Value, CultureInfo.InvariantCulture);
			}
		}
        #endregion

        #region Instance Methods
        public override string ToString()
        {
            return String.Concat(RouteParser.VariableStart, 
				_variableName, RouteParser.VariableEnd);
        }
        #endregion
    }
}

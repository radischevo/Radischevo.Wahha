using System;

namespace Radischevo.Wahha.Core
{
    internal class FormatExpression : ITextExpression
    {
        #region Instance Fields
        private bool _isInvalid;
        private string _expression;
        private string _format;
        #endregion

        #region Constructors
        public FormatExpression(string expression)
        {
            if (!expression.StartsWith("{") || !expression.EndsWith("}"))
            {
                _isInvalid = true;
                _expression = expression;
                return;
            }

            string expressionWithoutBraces = expression.Substring(1, expression.Length - 2);
            int colonIndex = expressionWithoutBraces.IndexOf(':');
            
            if (colonIndex < 0)
            {
                _expression = expressionWithoutBraces;
            }
            else
            {
                _expression = expressionWithoutBraces.Substring(0, colonIndex);
                _format = expressionWithoutBraces.Substring(colonIndex + 1);
            }
        }
        #endregion

        #region Instance Properties
        public string Expression
        {
            get
            {
                return _expression;
            }
        }

        public string Format
        {
            get
            {
                return _format;
            }
        }
        #endregion

        #region Instance Methods
        public string Eval(object container)
        {
			if (_isInvalid)
				throw Error.InvalidFormatExpression(_expression);

            try
            {
                return ObjectExtensions.Evaluate(container, _expression, _format);
            }
            catch (ArgumentException)
            {
                throw new FormatException();
            }
            catch (InvalidOperationException)
            {
                throw new FormatException();
            }
        }
        #endregion
    }
}

using System;

namespace Radischevo.Wahha.Core
{
    internal class LiteralExpression : ITextExpression
    {
        #region Instance Fields
        private string _literal;
        #endregion

        #region Constructors
        public LiteralExpression(string literal)
        {
            _literal = literal;
        }
        #endregion

        #region Instance Properties
        public string Literal
        {
            get
            {
                return _literal;
            }
        }
        #endregion

        #region Instance Methods
        public string Eval(object o)
        {
            return _literal
                .Replace("{{", "{")
                .Replace("}}", "}");
        }
        #endregion
    }
}

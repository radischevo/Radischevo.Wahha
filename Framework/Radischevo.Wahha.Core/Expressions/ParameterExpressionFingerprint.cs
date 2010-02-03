using System;
using System.Linq.Expressions;

namespace Radischevo.Wahha.Core.Expressions
{
    internal sealed class ParameterExpressionFingerprint : ExpressionFingerprint
    {
        #region Constructors
        private ParameterExpressionFingerprint(ParameterExpression expression)
            : base(expression)
        {
        }
        #endregion

        #region Static Methods
        public static ParameterExpressionFingerprint Create(
            ParameterExpression expression, ParserContext context)
        {
            if (expression == context.Instance)
                return new ParameterExpressionFingerprint(expression);
            else
                return null;
        }
        #endregion

        #region Instance Methods
        public override Expression ToExpression(ParserContext context)
        {
            return context.Instance;
        }
        #endregion
    }
}

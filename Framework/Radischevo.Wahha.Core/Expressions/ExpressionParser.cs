using System;
using System.Linq.Expressions;

namespace Radischevo.Wahha.Core.Expressions
{
    internal static class ExpressionParser
    {
        #region Static Methods
        public static ParserContext Parse<TClass, TMember>(
            Expression<Func<TClass, TMember>> expression)
        {
            ParserContext context = new ParserContext() {
                    Instance = expression.Parameters[0]
                };

            Expression body = expression.Body;
            context.Fingerprint = ExpressionFingerprint.Create(body, context);

            return context;
        }
        #endregion
    }
}

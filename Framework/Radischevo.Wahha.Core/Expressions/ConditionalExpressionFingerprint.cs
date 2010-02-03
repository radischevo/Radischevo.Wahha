#pragma warning disable 659

using System;
using System.Linq.Expressions;

namespace Radischevo.Wahha.Core.Expressions
{
    internal sealed class ConditionalExpressionFingerprint : ExpressionFingerprint
    {
        #region Instance Fields
        private ExpressionFingerprint _test;
        private ExpressionFingerprint _ifTrue;
        private ExpressionFingerprint _ifFalse;
        #endregion

        #region Constructors
        private ConditionalExpressionFingerprint(
            ConditionalExpression expression)
            : base(expression)
        {
        }
        #endregion

        #region Instance Methods
        public ExpressionFingerprint Test
        {
            get
            {
                return _test;
            }
        }

        public ExpressionFingerprint IfTrue
        {
            get
            {
                return _ifTrue;
            }
        }

        public ExpressionFingerprint IfFalse
        {
            get
            {
                return _ifFalse;
            }
        }
        #endregion

        #region Static Methods
        public static ConditionalExpressionFingerprint Create(
            ConditionalExpression expression, ParserContext context)
        {
            ExpressionFingerprint test = Create(expression.Test, context);
            if (test == null && expression.Test != null)
                return null;

            ExpressionFingerprint ifTrue = Create(expression.IfTrue, context);
            if (ifTrue == null && expression.IfTrue != null)
                return null;

            ExpressionFingerprint ifFalse = Create(expression.IfFalse, context);
            if (ifFalse == null && expression.IfFalse != null)
                return null;

            return new ConditionalExpressionFingerprint(expression) {
                    _test = test, _ifTrue = ifTrue, _ifFalse = ifFalse
                };
        }
        #endregion

        #region Instance Methods
        public override void AddToHashCodeCombiner(HashCodeCombiner combiner)
        {
            base.AddToHashCodeCombiner(combiner);

            combiner.AddFingerprint(_test);
            combiner.AddFingerprint(_ifTrue);
            combiner.AddFingerprint(_ifFalse);
        }

        public override bool Equals(object obj)
        {
            ConditionalExpressionFingerprint other = 
                (obj as ConditionalExpressionFingerprint);

            if (other == null)
                return false;

            return (Object.Equals(_test, other._test)
                && Object.Equals(_ifTrue, other._ifTrue)
                && Object.Equals(_ifFalse, other._ifFalse)
                && base.Equals(other));
        }

        public override Expression ToExpression(ParserContext context)
        {
            Expression test = ToExpression(_test, context);
            Expression ifTrue = ToExpression(_ifTrue, context);
            Expression ifFalse = ToExpression(_ifFalse, context);

            return Expression.Condition(test, ifTrue, ifFalse);
        }
        #endregion
    }
}

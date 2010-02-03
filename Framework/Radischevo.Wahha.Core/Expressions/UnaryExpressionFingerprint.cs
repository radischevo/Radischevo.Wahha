#pragma warning disable 659

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Radischevo.Wahha.Core.Expressions
{
    /// <summary>
    /// The most common appearance of a UnaryExpression 
    /// is a cast or other conversion operator.
    /// </summary>
    internal sealed class UnaryExpressionFingerprint : ExpressionFingerprint
    {
        #region Instance Fields
        private MethodInfo _method;
        private ExpressionFingerprint _operand;
        #endregion

        #region Constructors
        private UnaryExpressionFingerprint(UnaryExpression expression)
            : base(expression)
        {
            _method = expression.Method;
        }
        #endregion

        #region Instance Properties
        public MethodInfo Method
        {
            get
            {
                return _method;
            }
        }

        public ExpressionFingerprint Operand
        {
            get
            {
                return _operand;
            }
        }
        #endregion

        #region Static Methods
        public static UnaryExpressionFingerprint Create(
            UnaryExpression expression, ParserContext context)
        {
            ExpressionFingerprint operand = Create(expression.Operand, context);
            if (operand == null && expression.Operand != null)
                return null;

            return new UnaryExpressionFingerprint(expression) {
                    _operand = operand
                };
        }
        #endregion

        #region Instance Methods
        public override void AddToHashCodeCombiner(HashCodeCombiner combiner)
        {
            base.AddToHashCodeCombiner(combiner);

            combiner.AddObject(Method);
            combiner.AddFingerprint(Operand);
        }

        public override bool Equals(object obj)
        {
            UnaryExpressionFingerprint other = 
                (obj as UnaryExpressionFingerprint);

            if (other == null)
                return false;

            return (_method == other._method
                && Object.Equals(_operand, other._operand)
                && base.Equals(other));
        }

        public override Expression ToExpression(ParserContext context)
        {
            Expression operand = ToExpression(_operand, context);

            if (NodeType == ExpressionType.UnaryPlus)
                return Expression.UnaryPlus(operand, Method);
            
            return Expression.MakeUnary(NodeType, operand, Type, _method);
        }
        #endregion
    }
}

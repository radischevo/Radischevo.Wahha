using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Radischevo.Wahha.Core.Expressions {

    /// <summary>
    /// Class, useful for interpreting things like array[index].
    /// This particular fingerprint doesn't support the 
    /// BinaryExpression.Conversion property, which is used 
    /// in some coalescing operations.
    /// </summary>
    internal sealed class BinaryExpressionFingerprint : ExpressionFingerprint
    {
        #region Instance Fields
        private bool _isLiftedToNull;
        private MethodInfo _method;
        private ExpressionFingerprint _left;
        private ExpressionFingerprint _right;
        #endregion

        #region Constructors
        private BinaryExpressionFingerprint(BinaryExpression expression)
            : base(expression)
        {
            _method = expression.Method;
            _isLiftedToNull = expression.IsLiftedToNull;
        }
        #endregion

        #region Instance Properties
        public bool IsLiftedToNull
        {
            get
            {
                return _isLiftedToNull;
            }
        }

        public MethodInfo Method
        {
            get
            {
                return _method;
            }
        }

        public ExpressionFingerprint Left
        {
            get
            {
                return _left;
            }
        }

        public ExpressionFingerprint Right
        {
            get
            {
                return _right;
            }
        }
        #endregion

        #region Static Methods
        public static BinaryExpressionFingerprint Create(
            BinaryExpression expression, ParserContext context)
        {
            if (expression.Conversion != null)
                return null;

            ExpressionFingerprint left = Create(expression.Left, context);
            if (left == null && expression.Left != null)
                return null;

            ExpressionFingerprint right = Create(expression.Right, context);
            if (right == null && expression.Right != null)
                return null;

            return new BinaryExpressionFingerprint(expression) {
                    _left = left, _right = right
                };
        }
        #endregion

        #region Instance Methods
        public override void AddToHashCodeCombiner(HashCodeCombiner combiner)
        {
            base.AddToHashCodeCombiner(combiner);

            combiner.AddInt32(IsLiftedToNull.GetHashCode());
            combiner.AddObject(Method);
            combiner.AddFingerprint(Left);
            combiner.AddFingerprint(Right);
        }

        public override bool Equals(object obj)
        {
            BinaryExpressionFingerprint other = 
                (obj as BinaryExpressionFingerprint);
            
            if (other == null)
                return false;

            return (_isLiftedToNull == other._isLiftedToNull
                && _method == other._method
                && Object.Equals(_left, other._left)
                && Object.Equals(_right, other._right)
                && base.Equals(other));
        }

        public override Expression ToExpression(ParserContext context)
        {
            Expression left = ToExpression(_left, context);
            Expression right = ToExpression(_right, context);

            return Expression.MakeBinary(NodeType, left, right, _isLiftedToNull, _method);
        }
        #endregion
    }
}


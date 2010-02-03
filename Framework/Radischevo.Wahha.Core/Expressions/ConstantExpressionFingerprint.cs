#pragma warning disable 659

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Radischevo.Wahha.Core.Expressions
{
    /// <summary>
    /// A ConstantExpression might represent a captured local variable, so we can't compile
    /// the value directly into the cached function. Instead, a placeholder is generated
    /// and the value is hoisted into a local variables array. This placeholder can then
    /// be compiled and cached, and the array lookup happens at runtime.
    /// </summary>
    internal sealed class ConstantExpressionFingerprint : ExpressionFingerprint
    {
        #region Instance Fields
        private int _index;
        #endregion

        #region Constructors
        private ConstantExpressionFingerprint(ConstantExpression expression)
            : base(expression)
        {
        }
        #endregion

        #region Instance Properties
        public int Index
        {
            get
            {
                return _index;
            }
        }
        #endregion

        #region Static Methods
        public static ConstantExpressionFingerprint Create(
            ConstantExpression expression, ParserContext context)
        {
            ConstantExpressionFingerprint fingerprint =
                new ConstantExpressionFingerprint(expression)
                {
                    _index = context.HoistedValues.Count
                };

            context.HoistedValues.Add(expression.Value);
            return fingerprint;
        }
        #endregion

        #region Instance Methods
        public override void AddToHashCodeCombiner(HashCodeCombiner combiner)
        {
            base.AddToHashCodeCombiner(combiner);
            combiner.AddInt32(_index);
        }

        public override bool Equals(object obj)
        {
            ConstantExpressionFingerprint other = 
                (obj as ConstantExpressionFingerprint);

            if (other == null)
                return false;

            return (_index == other._index && base.Equals(other));
        }

        public override Expression ToExpression(ParserContext context)
        {
            BinaryExpression index = Expression.ArrayIndex(
                ParserContext.HoistedValuesParameter, 
                Expression.Constant(_index));

            return Expression.Convert(index, Type);
        }
        #endregion
    }
}

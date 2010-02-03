#pragma warning disable 659

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Radischevo.Wahha.Core.Expressions
{
    internal sealed class MemberExpressionFingerprint : ExpressionFingerprint
    {
        #region Instance Fields
        private MemberInfo _member;
        private ExpressionFingerprint _target;
        #endregion

        #region Constructors
        private MemberExpressionFingerprint(MemberExpression expression)
            : base(expression)
        {
            _member = expression.Member;
        }
        #endregion

        #region Instance Properties
        public MemberInfo Member
        {
            get
            {
                return _member;
            }
        }

        public ExpressionFingerprint Target
        {
            get
            {
                return _target;
            }
        }
        #endregion

        #region Static Methods
        public static MemberExpressionFingerprint Create(
            MemberExpression expression, ParserContext context)
        {
            ExpressionFingerprint target = Create(expression.Expression, context);
            if (target == null && expression.Expression != null)
                return null;

            return new MemberExpressionFingerprint(expression) {
                    _target = target
                };
        }
        #endregion

        #region Instance Methods
        public override void AddToHashCodeCombiner(HashCodeCombiner combiner)
        {
            base.AddToHashCodeCombiner(combiner);

            combiner.AddObject(Member);
            combiner.AddFingerprint(Target);
        }

        public override bool Equals(object obj)
        {
            MemberExpressionFingerprint other = 
                (obj as MemberExpressionFingerprint);

            if (other == null)
                return false;

            return (_member == other._member
                && Object.Equals(_target, other._target)
                && base.Equals(other));
        }

        public override Expression ToExpression(ParserContext context)
        {
            return Expression.MakeMemberAccess(
                ToExpression(_target, context), _member);
        }
        #endregion
    }
}

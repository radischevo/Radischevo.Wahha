#pragma warning disable 659

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Radischevo.Wahha.Core.Expressions
{
    internal sealed class MethodCallExpressionFingerprint : ExpressionFingerprint
    {
        #region Instance Fields
        private MethodInfo _method;
        private ExpressionFingerprint _target;
        private ReadOnlyCollection<ExpressionFingerprint> _arguments;
        #endregion

        #region Constructors
        private MethodCallExpressionFingerprint(
            MethodCallExpression expression)
            : base(expression)
        {
            _method = expression.Method;
        }
        #endregion

        #region Instance Properties
        public ReadOnlyCollection<ExpressionFingerprint> Arguments
        {
            get
            {
                return _arguments;
            }
        }

        public MethodInfo Method
        {
            get
            {
                return _method;
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
        public static MethodCallExpressionFingerprint Create(
            MethodCallExpression expression, ParserContext context)
        {
            ReadOnlyCollection<ExpressionFingerprint> arguments = 
                Create(expression.Arguments, context);
            if (arguments == null)
                return null;

            ExpressionFingerprint target = Create(expression.Object, context);
            if (target == null && expression.Object != null)
                return null;

            return new MethodCallExpressionFingerprint(expression) {
                    _arguments = arguments,
                    _target = target
                };
        }
        #endregion

        #region Instance Methods
        public override void AddToHashCodeCombiner(HashCodeCombiner combiner)
        {
            base.AddToHashCodeCombiner(combiner);

            combiner.AddEnumerable(Arguments);
            combiner.AddObject(Method);
            combiner.AddFingerprint(Target);
        }

        public override bool Equals(object obj)
        {
            MethodCallExpressionFingerprint other = 
                (obj as MethodCallExpressionFingerprint);

            if (other == null)
                return false;

            return (_arguments.SequenceEqual(other._arguments)
                && _method == other._method
                && Object.Equals(_target, other._target)
                && base.Equals(other));
        }

        public override Expression ToExpression(ParserContext context)
        {
            Expression target = ToExpression(_target, context);
            IEnumerable<Expression> arguments = ToExpression(_arguments, context);

            return Expression.Call(target, _method, arguments);
        }
        #endregion
    }
}

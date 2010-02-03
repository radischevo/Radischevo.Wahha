using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq
{
    /// <summary>
    /// Rewrites an expression tree so that 
    /// locally isolatable sub-expressions are evaluated 
    /// and converted into ConstantExpression nodes.
    /// </summary>
    public static class PartialEvaluator
    {
        #region Nested Types
        /// <summary>
        /// Evaluates & replaces sub-trees when first candidate is reached (top-down)
        /// </summary>
        private class SubtreeEvaluator : ExpressionVisitor
        {
            #region Instance Fields
            private HashSet<Expression> _candidates;
            #endregion

            #region Constructors
            private SubtreeEvaluator(HashSet<Expression> candidates)
            {
                _candidates = candidates;
            }
            #endregion

            #region Static Methods
            public static Expression Eval(HashSet<Expression> candidates, Expression exp)
            {
                return new SubtreeEvaluator(candidates).Visit(exp);
            }
            #endregion

            #region Instance Methods
            protected override Expression Visit(Expression exp)
            {
                if (exp == null)
                    return null;

                if (_candidates.Contains(exp))
                    return Evaluate(exp);

                return base.Visit(exp);
            }

            private Expression Evaluate(Expression e)
            {
                if (e.NodeType == ExpressionType.Constant)
                    return e;

                Type type = e.Type;
                if (type.IsValueType)
                    e = Expression.Convert(e, typeof(object));

                Expression<Func<object>> lambda = Expression.Lambda<Func<object>>(e);
                Func<object> fn = lambda.Compile();

                return Expression.Constant(fn(), type);
            }
            #endregion
        }

        /// <summary>
        /// Performs bottom-up analysis to determine which nodes can possibly
        /// be part of an evaluated sub-tree.
        /// </summary>
        private class Nominator : ExpressionVisitor
        {
            #region Instance Fields
            private Func<Expression, bool> _canBeEvaluated;
            private bool _cannotBeEvaluated;
            private HashSet<Expression> _candidates;
            #endregion

            #region Constructors
            private Nominator(Func<Expression, bool> canBeEvaluated)
            {
                _candidates = new HashSet<Expression>();
                _canBeEvaluated = canBeEvaluated;
            }
            #endregion

            #region Static Methods
            public static HashSet<Expression> Nominate(Func<Expression, bool> canBeEvaluated, Expression expression)
            {
                Nominator nominator = new Nominator(canBeEvaluated);
                nominator.Visit(expression);
                return nominator._candidates;
            }
            #endregion

            #region Instance Methods
            protected override Expression Visit(Expression expression)
            {
                if (expression != null)
                {
                    bool saveCannotBeEvaluated = _cannotBeEvaluated;
                    _cannotBeEvaluated = false;
                    base.Visit(expression);

                    if (!_cannotBeEvaluated)
                    {
                        if (_canBeEvaluated(expression))
                            _candidates.Add(expression);
                        else
                            _cannotBeEvaluated = true;
                    }
                    _cannotBeEvaluated |= saveCannotBeEvaluated;
                }
                return expression;
            }
            #endregion
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Performs evaluation & replacement of independent sub-trees
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <param name="fnCanBeEvaluated">A function that decides whether a given 
        /// expression node can be part of the local function.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression Eval(Expression expression, Func<Expression, bool> canBeEvaluated)
        {
            return SubtreeEvaluator.Eval(Nominator.Nominate(canBeEvaluated, expression), expression);
        }

        /// <summary>
        /// Performs evaluation & replacement of independent sub-trees
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression Eval(Expression expression)
        {
            return Eval(expression, PartialEvaluator.CanBeEvaluatedLocally);
        }

        private static bool CanBeEvaluatedLocally(Expression expression)
        {
            return (expression.NodeType != ExpressionType.Parameter);
        }
        #endregion
    }
}

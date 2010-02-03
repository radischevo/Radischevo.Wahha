using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Jeltofiol.Wahha.Core;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    /// <summary>
    /// Rewrites top level skips to a client-side skip
    /// </summary>
    public class ClientSkipRewriter : DbExpressionVisitor
    {
        private ClientSkipRewriter()
        {
        }

        public static Expression Rewrite(Expression expression)
        {
            return new ClientSkipRewriter().Visit(expression);
        }

        protected override Expression VisitProjection(DbProjectionExpression proj)
        {
            if (proj.Select.Skip != null)
            {
                Expression newTake = (proj.Select.Take != null) ? Expression.Add(proj.Select.Skip, proj.Select.Take) : null;
                if (newTake != null)
                {
                    newTake = PartialEvaluator.Eval(newTake);
                }
                var newSelect = proj.Select.SetSkip(null).SetTake(newTake);
                var elementType = proj.Type.GetSequenceElementType();
                var agg = proj.Aggregator;
                var p = agg != null ? agg.Parameters[0] : Expression.Parameter(elementType, "p");
                var skip = Expression.Call(typeof(Enumerable), "Skip", new Type[]{elementType}, p, proj.Select.Skip);
                if (agg != null) {
                    agg = (LambdaExpression)DbExpressionReplacer.Replace(agg, p, skip);
                }
                else {
                    agg = Expression.Lambda(skip, p);
                }
                return new DbProjectionExpression(newSelect, proj.Projector, agg);
            }
            return proj;
        }
    }
}
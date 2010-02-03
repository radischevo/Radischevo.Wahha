using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    /// <summary>
    /// Rewrite aggregate expressions, moving them into same select expression that has the group-by clause
    /// </summary>
    public class AggregateRewriter : DbExpressionVisitor
    {
        ILookup<DbTableAlias, DbAggregateSubqueryExpression> lookup;
        Dictionary<DbAggregateSubqueryExpression, Expression> map;

        private AggregateRewriter(Expression expr)
        {
            this.map = new Dictionary<DbAggregateSubqueryExpression, Expression>();
            this.lookup = AggregateGatherer.Gather(expr).ToLookup(a => a.GroupByAlias);
        }

        public static Expression Rewrite(Expression expr)
        {
            return new AggregateRewriter(expr).Visit(expr);
        }

        protected override Expression VisitSelect(DbSelectExpression select)
        {
            select = (DbSelectExpression)base.VisitSelect(select);
            if (lookup.Contains(select.Alias))
            {
                List<DbColumnDeclaration> aggColumns = new List<DbColumnDeclaration>(select.Columns);
                foreach (DbAggregateSubqueryExpression ae in lookup[select.Alias])
                {
                    string name = "agg" + aggColumns.Count;
                    DbColumnDeclaration cd = new DbColumnDeclaration(name, ae.AggregateInGroupSelect);
                    this.map.Add(ae, new DbColumnExpression(ae.Type, null, ae.GroupByAlias, name));
                    aggColumns.Add(cd);
                }
                return new DbSelectExpression(select.Alias, aggColumns, select.From, select.Where, select.OrderBy, 
                    select.GroupBy, select.IsDistinct, select.Skip, select.Take);
            }
            return select;
        }

        protected override Expression VisitAggregateSubquery(DbAggregateSubqueryExpression aggregate)
        {
            Expression mapped;
            if (this.map.TryGetValue(aggregate, out mapped))
                return mapped;
            
            return this.Visit(aggregate.AggregateAsSubquery);
        }

        class AggregateGatherer : DbExpressionVisitor
        {
            List<DbAggregateSubqueryExpression> aggregates = new List<DbAggregateSubqueryExpression>();

            private AggregateGatherer()
            {
            }

            internal static List<DbAggregateSubqueryExpression> Gather(Expression expression)
            {
                AggregateGatherer gatherer = new AggregateGatherer();
                gatherer.Visit(expression);
                return gatherer.aggregates;
            }

            protected override Expression VisitAggregateSubquery(DbAggregateSubqueryExpression aggregate)
            {
                this.aggregates.Add(aggregate);
                return base.VisitAggregateSubquery(aggregate);
            }
        }
    }
}
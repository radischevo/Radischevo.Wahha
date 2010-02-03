using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Jeltofiol.Wahha.Core;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    /// <summary>
    /// Determines if a SelectExpression contains any aggregate expressions
    /// </summary>
    internal class AggregateChecker : DbExpressionVisitor
    {
        bool hasAggregate = false;
        private AggregateChecker()
        {
        }

        internal static bool HasAggregates(DbSelectExpression expression)
        {
            AggregateChecker checker = new AggregateChecker();
            checker.Visit(expression);
            return checker.hasAggregate;
        }

        protected override Expression VisitAggregate(DbAggregateExpression aggregate)
        {
            this.hasAggregate = true;
            return aggregate;
        }

        protected override Expression VisitSelect(DbSelectExpression select)
        {
            // only consider aggregates in these locations
            this.Visit(select.Where);
            this.VisitOrderBy(select.OrderBy);
            this.VisitColumnDeclarations(select.Columns);
            return select;
        }

        protected override Expression VisitSubquery(DbSubqueryExpression subquery)
        {
            // don't count aggregates in subqueries
            return subquery;
        }
    }
}
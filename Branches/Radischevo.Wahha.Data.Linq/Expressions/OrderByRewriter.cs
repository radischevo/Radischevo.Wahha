// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    /// <summary>
    /// Moves order-bys to the outermost select if possible
    /// </summary>
    public class OrderByRewriter : DbExpressionVisitor
    {
        IList<DbOrderExpression> gatheredOrderings;
        bool isOuterMostSelect;

        private OrderByRewriter()
        {
            this.isOuterMostSelect = true;
        }

        public static Expression Rewrite(Expression expression)
        {
            return new OrderByRewriter().Visit(expression);
        }

        protected override Expression VisitSelect(DbSelectExpression select)
        {
            bool saveIsOuterMostSelect = this.isOuterMostSelect;
            try
            {
                this.isOuterMostSelect = false;
                select = (DbSelectExpression)base.VisitSelect(select);

                bool hasOrderBy = select.OrderBy != null && select.OrderBy.Count > 0;
                bool hasGroupBy = select.GroupBy != null && select.GroupBy.Count > 0;
                bool canHaveOrderBy = saveIsOuterMostSelect || select.Take != null || select.Skip != null;
                bool canReceiveOrderings = canHaveOrderBy && !hasGroupBy && !select.IsDistinct;

                if (hasOrderBy)
                {
                    this.PrependOrderings(select.OrderBy);
                }

                IEnumerable<DbOrderExpression> orderings = null;
                if (canReceiveOrderings)
                {
                    orderings = this.gatheredOrderings;
                }
                else if (canHaveOrderBy)
                {
                    orderings = select.OrderBy;
                }
                bool canPassOnOrderings = !saveIsOuterMostSelect && !hasGroupBy && !select.IsDistinct;
                ReadOnlyCollection<DbColumnDeclaration> columns = select.Columns;
                if (this.gatheredOrderings != null)
                {
                    if (canPassOnOrderings)
                    {
                        var producedAliases = DeclaredAliasGatherer.Gather(select.From);
                        // reproject order expressions using this select's alias so the outer select will have properly formed expressions
                        BindResult project = this.RebindOrderings(this.gatheredOrderings, select.Alias, producedAliases, select.Columns);
                        this.gatheredOrderings = null;
                        this.PrependOrderings(project.Orderings);
                        columns = project.Columns;
                    }
                    else
                    {
                        this.gatheredOrderings = null;
                    }
                }
                if (orderings != select.OrderBy || columns != select.Columns)
                {
                    select = new DbSelectExpression(select.Alias, columns, select.From, select.Where, orderings, select.GroupBy, select.IsDistinct, select.Skip, select.Take);
                }
                return select;
            }
            finally
            {
                this.isOuterMostSelect = saveIsOuterMostSelect;
            }
        }

        protected override Expression VisitSubquery(DbSubqueryExpression subquery)
        {
            var saveOrderings = this.gatheredOrderings;
            this.gatheredOrderings = null;
            var result = base.VisitSubquery(subquery);
            this.gatheredOrderings = saveOrderings;
            return result;
        }

        protected override Expression VisitJoin(DbJoinExpression join)
        {
            // make sure order by expressions lifted up from the left side are not lost
            // when visiting the right side
            Expression left = this.VisitSource(join.Left);
            IList<DbOrderExpression> leftOrders = this.gatheredOrderings;
            this.gatheredOrderings = null; // start on the right with a clean slate
            Expression right = this.VisitSource(join.Right);
            this.PrependOrderings(leftOrders);
            Expression condition = this.Visit(join.Condition);
            if (left != join.Left || right != join.Right || condition != join.Condition)
            {
                return new DbJoinExpression(join.Join, left, right, condition);
            }
            return join;
        }

        /// <summary>
        /// Add a sequence of order expressions to an accumulated list, prepending so as
        /// to give precedence to the new expressions over any previous expressions
        /// </summary>
        /// <param name="newOrderings"></param>
        protected void PrependOrderings(IList<DbOrderExpression> newOrderings)
        {
            if (newOrderings != null)
            {
                if (this.gatheredOrderings == null)
                {
                    this.gatheredOrderings = new List<DbOrderExpression>();
                }
                for (int i = newOrderings.Count - 1; i >= 0; i--)
                {
                    this.gatheredOrderings.Insert(0, newOrderings[i]);
                }
                // trim off obvious duplicates
                HashSet<string> unique = new HashSet<string>();
                for (int i = 0; i < this.gatheredOrderings.Count;) 
                {
                    DbColumnExpression column = this.gatheredOrderings[i].Expression as DbColumnExpression;
                    if (column != null)
                    {
                        string hash = column.Alias + ":" + column.Name;
                        if (unique.Contains(hash))
                        {
                            this.gatheredOrderings.RemoveAt(i);
                            // don't increment 'i', just continue
                            continue;
                        }
                        else
                        {
                            unique.Add(hash);
                        }
                    }
                    i++;
                }
            }
        }

        protected class BindResult
        {
            ReadOnlyCollection<DbColumnDeclaration> columns;
            ReadOnlyCollection<DbOrderExpression> orderings;

            public BindResult(IEnumerable<DbColumnDeclaration> columns, IEnumerable<DbOrderExpression> orderings)
            {
                this.columns = columns as ReadOnlyCollection<DbColumnDeclaration>;
                if (this.columns == null)
                {
                    this.columns = new List<DbColumnDeclaration>(columns).AsReadOnly();
                }
                this.orderings = orderings as ReadOnlyCollection<DbOrderExpression>;
                if (this.orderings == null)
                {
                    this.orderings = new List<DbOrderExpression>(orderings).AsReadOnly();
                }
            }
            public ReadOnlyCollection<DbColumnDeclaration> Columns
            {
                get { return this.columns; }
            }
            public ReadOnlyCollection<DbOrderExpression> Orderings
            {
                get { return this.orderings; }
            }
        }

        /// <summary>
        /// Rebind order expressions to reference a new alias and add to column declarations if necessary
        /// </summary>
        protected virtual BindResult RebindOrderings(IEnumerable<DbOrderExpression> orderings, DbTableAlias alias,
            HashSet<DbTableAlias> existingAliases, IEnumerable<DbColumnDeclaration> existingColumns)
        {
            List<DbColumnDeclaration> newColumns = null;
            List<DbOrderExpression> newOrderings = new List<DbOrderExpression>();
            foreach (DbOrderExpression ordering in orderings)
            {
                Expression expr = ordering.Expression;
                DbColumnExpression column = expr as DbColumnExpression;
                if (column == null || (existingAliases != null && existingAliases.Contains(column.Alias)))
                {
                    // check to see if a declared column already contains a similar expression
                    int iOrdinal = 0;
                    foreach (DbColumnDeclaration decl in existingColumns)
                    {
                        DbColumnExpression declColumn = decl.Expression as DbColumnExpression;
                        if (decl.Expression == ordering.Expression ||
                            (column != null && declColumn != null && column.Alias == declColumn.Alias && column.Name == declColumn.Name))
                        {
                            // found it, so make a reference to this column
                            expr = new DbColumnExpression(column.Type, column.DbType, alias, decl.Name);
                            break;
                        }
                        iOrdinal++;
                    }
                    // if not already projected, add a new column declaration for it
                    if (expr == ordering.Expression)
                    {
                        if (newColumns == null)
                        {
                            newColumns = new List<DbColumnDeclaration>(existingColumns);
                            existingColumns = newColumns;
                        }
                        string colName = column != null ? column.Name : "c" + iOrdinal;
                        newColumns.Add(new DbColumnDeclaration(colName, ordering.Expression));
                        expr = new DbColumnExpression(expr.Type, null, alias, colName);
                    }
                    newOrderings.Add(new DbOrderExpression(ordering.OrderType, expr));
                }
            }
            return new BindResult(existingColumns, newOrderings);
        }
    }
}

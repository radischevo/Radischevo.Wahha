// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    /// <summary>
    /// Rewrites take & skip expressions into uses 
    /// of T-SQL row_number function
    /// </summary>
    public class SkipToRowNumberRewriter : DbExpressionVisitor
    {
        private SkipToRowNumberRewriter()
        {
        }

        public static Expression Rewrite(Expression expression)
        {
            return new SkipToRowNumberRewriter().Visit(expression);
        }

        protected override Expression VisitSelect(DbSelectExpression select)
        {
            select = (DbSelectExpression)base.VisitSelect(select);
            if (select.Skip != null)
            {
                DbSelectExpression newSelect = select.SetSkip(null).SetTake(null);
                bool canAddColumn = !select.IsDistinct && (select.GroupBy == null || select.GroupBy.Count == 0);
                if (!canAddColumn)
                {
                    newSelect = newSelect.AddRedundantSelect(new DbTableAlias());
                }
                newSelect = newSelect.AddColumn(new DbColumnDeclaration("rownum", new DbRowNumberExpression(select.OrderBy)));

                // add layer for WHERE clause that references new rownum column
                newSelect = newSelect.AddRedundantSelect(new DbTableAlias());
                newSelect = newSelect.RemoveColumn(newSelect.Columns.Single(c => c.Name == "rownum"));

                var newAlias = ((DbSelectExpression)newSelect.From).Alias;
                DbColumnExpression rnCol = new DbColumnExpression(typeof(int), null, newAlias, "rownum");
                Expression where;
                if (select.Take != null)
                {
                    where = new DbBetweenExpression(rnCol, Expression.Add(select.Skip, Expression.Constant(1)), 
                        Expression.Add(select.Skip, select.Take));
                }
                else
                {
                    where = rnCol.GreaterThan(select.Skip);
                }
                if (newSelect.Where != null)
                {
                    where = newSelect.Where.And(where);
                }
                newSelect = newSelect.SetWhere(where);

                select = newSelect;
            }
            return select;
        }
    }
}
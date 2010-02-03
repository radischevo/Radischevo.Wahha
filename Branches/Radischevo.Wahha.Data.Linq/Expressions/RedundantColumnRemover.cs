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
    /// Removes duplicate column declarations that refer to the same underlying column
    /// </summary>
    public class RedundantColumnRemover : DbExpressionVisitor
    {
        Dictionary<DbColumnExpression, DbColumnExpression> map;

        private RedundantColumnRemover()
        {
            this.map = new Dictionary<DbColumnExpression, DbColumnExpression>();
        }

        public static Expression Remove(Expression expression)
        {
            return new RedundantColumnRemover().Visit(expression);
        }

        protected override Expression VisitColumn(DbColumnExpression column)
        {
            DbColumnExpression mapped;
            if (this.map.TryGetValue(column, out mapped))
            {
                return mapped;
            }
            return column;
        }

        protected override Expression VisitSelect(DbSelectExpression select)
        {
            select = (DbSelectExpression)base.VisitSelect(select);

            // look for redundant column declarations
            List<DbColumnDeclaration> cols = select.Columns.OrderBy(c => c.Name).ToList();
            BitArray removed = new BitArray(select.Columns.Count);
            bool anyRemoved = false;
            for (int i = 0, n = cols.Count; i < n - 1; i++)
            {
                DbColumnDeclaration ci = cols[i];
                DbColumnExpression cix = ci.Expression as DbColumnExpression;
                DbDataType qt = cix != null ? cix.DbType : null;
                DbColumnExpression cxi = new DbColumnExpression(ci.Expression.Type, qt, select.Alias, ci.Name);
                for (int j = i + 1; j < n; j++)
                {
                    if (!removed.Get(j))
                    {
                        DbColumnDeclaration cj = cols[j];
                        if (SameExpression(ci.Expression, cj.Expression))
                        {
                            // any reference to 'j' should now just be a reference to 'i'
                            DbColumnExpression cxj = new DbColumnExpression(cj.Expression.Type, qt, select.Alias, cj.Name);
                            this.map.Add(cxj, cxi);
                            removed.Set(j, true);
                            anyRemoved = true;
                        }
                    }
                }
            }
            if (anyRemoved)
            {
                List<DbColumnDeclaration> newDecls = new List<DbColumnDeclaration>();
                for (int i = 0, n = cols.Count; i < n; i++)
                {
                    if (!removed.Get(i))
                    {
                        newDecls.Add(cols[i]);
                    }
                }
                select = select.SetColumns(newDecls);
            }
            return select;
        }

        bool SameExpression(Expression a, Expression b)
        {
            if (a == b) return true;
            DbColumnExpression ca = a as DbColumnExpression;
            DbColumnExpression cb = b as DbColumnExpression;
            return (ca != null && cb != null && ca.Alias == cb.Alias && ca.Name == cb.Name);
        }
    }
}
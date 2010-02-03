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
    /// Duplicate the query expression by making a copy with new table aliases
    /// </summary>
    public class QueryDuplicator : DbExpressionVisitor
    {
        Dictionary<DbTableAlias, DbTableAlias> map = new Dictionary<DbTableAlias, DbTableAlias>();

        public static Expression Duplicate(Expression expression)
        {
            return new QueryDuplicator().Visit(expression);
        }

        protected override Expression VisitTable(DbTableExpression table)
        {
            DbTableAlias newAlias = new DbTableAlias();
            this.map[table.Alias] = newAlias;
            return new DbTableExpression(newAlias, table.Type, table.Name);
        }

        protected override Expression VisitSelect(DbSelectExpression select)
        {
            DbTableAlias newAlias = new DbTableAlias();
            this.map[select.Alias] = newAlias;
            select = (DbSelectExpression)base.VisitSelect(select);
            return new DbSelectExpression(newAlias, select.Columns, select.From, select.Where, select.OrderBy, 
                select.GroupBy, select.IsDistinct, select.Skip, select.Take);
        }

        protected override Expression VisitColumn(DbColumnExpression column)
        {
            DbTableAlias newAlias;
            if (this.map.TryGetValue(column.Alias, out newAlias))
            {
                return new DbColumnExpression(column.Type, column.DbType, newAlias, column.Name);
            }
            return column;
        }
    }
}
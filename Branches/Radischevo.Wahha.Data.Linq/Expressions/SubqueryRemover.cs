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
    /// Removes one or more SelectExpression's by rewriting the expression tree to not include them, promoting
    /// their from clause expressions and rewriting any column expressions that may have referenced them to now
    /// reference the underlying data directly.
    /// </summary>
    public class SubqueryRemover : DbExpressionVisitor
    {
        HashSet<DbSelectExpression> selectsToRemove;
        Dictionary<DbTableAlias, Dictionary<string, Expression>> map;

        private SubqueryRemover(IEnumerable<DbSelectExpression> selectsToRemove)
        {
            this.selectsToRemove = new HashSet<DbSelectExpression>(selectsToRemove);
            this.map = this.selectsToRemove.ToDictionary(d => d.Alias, 
                d => d.Columns.ToDictionary(d2 => d2.Name, d2 => d2.Expression));
        }

        public static DbSelectExpression Remove(DbSelectExpression outerSelect, params DbSelectExpression[] selectsToRemove)
        {
            return Remove(outerSelect, (IEnumerable<DbSelectExpression>)selectsToRemove);
        }

        public static DbSelectExpression Remove(DbSelectExpression outerSelect, IEnumerable<DbSelectExpression> selectsToRemove)
        {
            return (DbSelectExpression)new SubqueryRemover(selectsToRemove).Visit(outerSelect);
        }

        public static DbProjectionExpression Remove(DbProjectionExpression projection, params DbSelectExpression[] selectsToRemove)
        {
            return Remove(projection, (IEnumerable<DbSelectExpression>)selectsToRemove);
        }

        public static DbProjectionExpression Remove(DbProjectionExpression projection, IEnumerable<DbSelectExpression> selectsToRemove)
        {
            return (DbProjectionExpression)new SubqueryRemover(selectsToRemove).Visit(projection);
        }

        protected override Expression VisitSelect(DbSelectExpression select)
        {
            if (this.selectsToRemove.Contains(select))
            {
                return this.Visit(select.From);
            }
            else
            {
                return base.VisitSelect(select);
            }
        }

        protected override Expression VisitColumn(DbColumnExpression column)
        {
            Dictionary<string, Expression> nameMap;
            if (this.map.TryGetValue(column.Alias, out nameMap))
            {
                Expression expr;
                if (nameMap.TryGetValue(column.Name, out expr))
                {
                    return this.Visit(expr);
                }
                throw new Exception("Reference to undefined column");
            }
            return column;
        }
    }
}
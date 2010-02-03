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
    /// Gathers all columns referenced by the given expression
    /// </summary>
    public class ReferencedColumnGatherer : DbExpressionVisitor
    {
        HashSet<DbColumnExpression> columns = new HashSet<DbColumnExpression>();
        bool first = true;

        public static HashSet<DbColumnExpression> Gather(Expression expression)
        {
            var visitor = new ReferencedColumnGatherer();
            visitor.Visit(expression);
            return visitor.columns;
        }

        protected override Expression VisitColumn(DbColumnExpression column)
        {
            this.columns.Add(column);
            return column;
        }

        protected override Expression VisitSelect(DbSelectExpression select)
        {
            if (first)
            {
                first = false;
                return base.VisitSelect(select);
            }
            return select;
        }
    }
}
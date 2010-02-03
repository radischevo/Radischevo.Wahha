// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    /// <summary>
    /// returns the list of SelectExpressions accessible from the source expression
    /// </summary>
    public class SelectGatherer : DbExpressionVisitor
    {
        List<DbSelectExpression> selects = new List<DbSelectExpression>();

        public static ReadOnlyCollection<DbSelectExpression> Gather(Expression expression)
        {
            var gatherer = new SelectGatherer();
            gatherer.Visit(expression);
            return gatherer.selects.AsReadOnly();
        }

        protected override Expression VisitSelect(DbSelectExpression select)
        {
            this.selects.Add(select);
            return select; // don't visit sub-queries
        }
    }
}
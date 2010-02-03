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
    public class NamedValueGatherer : DbExpressionVisitor
    {
        HashSet<DbNamedValueExpression> namedValues = new HashSet<DbNamedValueExpression>(new NamedValueComparer());

        private NamedValueGatherer()
        {
        }

        public static ReadOnlyCollection<DbNamedValueExpression> Gather(Expression expr)
        {
            NamedValueGatherer gatherer = new NamedValueGatherer();
            gatherer.Visit(expr);
            return gatherer.namedValues.ToList().AsReadOnly();
        }

        protected override Expression VisitNamedValue(DbNamedValueExpression value)
        {
            this.namedValues.Add(value);
            return value;
        }

        private class NamedValueComparer : IEqualityComparer<DbNamedValueExpression>
        {
            public bool Equals(DbNamedValueExpression x, DbNamedValueExpression y)
            {
                return x.Name == y.Name;
            }

            public int GetHashCode(DbNamedValueExpression obj)
            {
                return obj.Name.GetHashCode();
            }
        }
    }
}
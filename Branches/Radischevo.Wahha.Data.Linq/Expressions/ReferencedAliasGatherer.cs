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
    ///  returns the set of all aliases produced by a query source
    /// </summary>
    public class ReferencedAliasGatherer : DbExpressionVisitor
    {
        HashSet<DbTableAlias> aliases;

        private ReferencedAliasGatherer()
        {
            this.aliases = new HashSet<DbTableAlias>();
        }

        public static HashSet<DbTableAlias> Gather(Expression source)
        {
            var gatherer = new ReferencedAliasGatherer();
            gatherer.Visit(source);
            return gatherer.aliases;
        }

        protected override Expression VisitColumn(DbColumnExpression column)
        {
            this.aliases.Add(column.Alias);
            return column;
        }
    }
}

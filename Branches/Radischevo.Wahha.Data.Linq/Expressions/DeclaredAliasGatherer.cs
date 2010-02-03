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
    public class DeclaredAliasGatherer : DbExpressionVisitor
    {
        HashSet<DbTableAlias> aliases;

        private DeclaredAliasGatherer()
        {
            this.aliases = new HashSet<DbTableAlias>();
        }

        public static HashSet<DbTableAlias> Gather(Expression source)
        {
            var gatherer = new DeclaredAliasGatherer();
            gatherer.Visit(source);
            return gatherer.aliases;
        }

        protected override Expression VisitSelect(DbSelectExpression select)
        {
            this.aliases.Add(select.Alias);
            return select;
        }

        protected override Expression VisitTable(DbTableExpression table)
        {
            this.aliases.Add(table.Alias);
            return table;
        }
    }
}

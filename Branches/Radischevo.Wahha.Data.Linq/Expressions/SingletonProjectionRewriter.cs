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
    /// Rewrites nested singleton projection into server-side joins
    /// </summary>
    public class SingletonProjectionRewriter : DbExpressionVisitor
    {
        QueryLanguage language;
        bool isTopLevel = true;
        DbSelectExpression currentSelect;

        private SingletonProjectionRewriter(QueryLanguage language)
        {
            this.language = language;
        }

        public static Expression Rewrite(QueryLanguage language, Expression expression)
        {
            return new SingletonProjectionRewriter(language).Visit(expression);
        }

        protected override Expression VisitClientJoin(DbClientJoinExpression join)
        {
            // treat client joins as new top level
            var saveTop = this.isTopLevel;
            var saveSelect = this.currentSelect;
            this.isTopLevel = true;
            this.currentSelect = null;
            Expression result = base.VisitClientJoin(join);
            this.isTopLevel = saveTop;
            this.currentSelect = saveSelect;
            return result;
        }

        protected override Expression VisitProjection(DbProjectionExpression proj)
        {
            if (isTopLevel)
            {
                isTopLevel = false;
                this.currentSelect = proj.Select;
                Expression projector = this.Visit(proj.Projector);
                if (projector != proj.Projector || this.currentSelect != proj.Select)
                {
                    return new DbProjectionExpression(this.currentSelect, projector, proj.Aggregator);
                }
                return proj;
            }

            if (proj.IsSingleton && this.CanJoinOnServer(this.currentSelect))
            {
                DbTableAlias newAlias = new DbTableAlias();
                this.currentSelect = this.currentSelect.AddRedundantSelect(newAlias);

                // remap any references to the outer select to the new alias;
                DbSelectExpression source = (DbSelectExpression)ColumnMapper.Map(proj.Select, newAlias, this.currentSelect.Alias);

                // add outer-join test
                DbProjectionExpression pex = new DbProjectionExpression(source, proj.Projector).AddOuterJoinTest();

                var pc = ColumnProjector.ProjectColumns(this.language.CanBeColumn, pex.Projector, this.currentSelect.Columns, 
                    this.currentSelect.Alias, newAlias, proj.Select.Alias);

                DbJoinExpression join = new DbJoinExpression(DbJoinType.OuterApply, this.currentSelect.From, pex.Select, null);

                this.currentSelect = new DbSelectExpression(this.currentSelect.Alias, pc.Columns, join, null);
                return this.Visit(pc.Projector);
            }

            var saveTop = this.isTopLevel;
            var saveSelect = this.currentSelect;
            this.isTopLevel = true;
            this.currentSelect = null;
            Expression result = base.VisitProjection(proj);
            this.isTopLevel = saveTop;
            this.currentSelect = saveSelect;
            return result;
        }

        private bool CanJoinOnServer(DbSelectExpression select)
        {
            // can add singleton (1:0,1) join if no grouping/aggregates or distinct
            return !select.IsDistinct
                && (select.GroupBy == null || select.GroupBy.Count == 0)
                && !AggregateChecker.HasAggregates(select);
        }

        protected override Expression VisitSubquery(DbSubqueryExpression subquery)
        {
            return subquery;
        }

        protected override Expression VisitStatement(DbStatementExpression command)
        {
            this.isTopLevel = true;
            return base.VisitStatement(command);
        }
    }
}
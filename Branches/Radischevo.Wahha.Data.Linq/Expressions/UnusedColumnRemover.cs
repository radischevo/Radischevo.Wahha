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
    /// Removes column declarations in SelectExpression's that are not referenced
    /// </summary>
    public class UnusedColumnRemover : DbExpressionVisitor
    {
        Dictionary<DbTableAlias, HashSet<string>> allColumnsUsed;

        private UnusedColumnRemover()
        {
            this.allColumnsUsed = new Dictionary<DbTableAlias, HashSet<string>>();
        }

        public static Expression Remove(Expression expression) 
        {
            return new UnusedColumnRemover().Visit(expression);
        }

        private void MarkColumnAsUsed(DbTableAlias alias, string name)
        {
            HashSet<string> columns;
            if (!this.allColumnsUsed.TryGetValue(alias, out columns))
            {
                columns = new HashSet<string>();
                this.allColumnsUsed.Add(alias, columns);
            }
            columns.Add(name);
        }

        private bool IsColumnUsed(DbTableAlias alias, string name)
        {
            HashSet<string> columnsUsed;
            if (this.allColumnsUsed.TryGetValue(alias, out columnsUsed))
            {
                if (columnsUsed != null)
                {
                    return columnsUsed.Contains(name);
                }
            }
            return false;
        }

        private void ClearColumnsUsed(DbTableAlias alias)
        {
            this.allColumnsUsed[alias] = new HashSet<string>();
        }

        protected override Expression VisitColumn(DbColumnExpression column)
        {
            MarkColumnAsUsed(column.Alias, column.Name);
            return column;
        }

        protected override Expression VisitSubquery(DbSubqueryExpression subquery) 
        {
            if ((subquery.NodeType == (ExpressionType)DbExpressionType.Scalar ||
                subquery.NodeType == (ExpressionType)DbExpressionType.In) &&
                subquery.Select != null) 
            {
                System.Diagnostics.Debug.Assert(subquery.Select.Columns.Count == 1);
                MarkColumnAsUsed(subquery.Select.Alias, subquery.Select.Columns[0].Name);
            }
 	        return base.VisitSubquery(subquery);
        }

        protected override Expression VisitSelect(DbSelectExpression select)
        {
            // visit column projection first
            ReadOnlyCollection<DbColumnDeclaration> columns = select.Columns;

            List<DbColumnDeclaration> alternate = null;
            for (int i = 0, n = select.Columns.Count; i < n; i++)
            {
                DbColumnDeclaration decl = select.Columns[i];
                if (select.IsDistinct || IsColumnUsed(select.Alias, decl.Name))
                {
                    Expression expr = this.Visit(decl.Expression);
                    if (expr != decl.Expression)
                    {
                        decl = new DbColumnDeclaration(decl.Name, expr);
                    }
                }
                else
                {
                    decl = null;  // null means it gets omitted
                }
                if (decl != select.Columns[i] && alternate == null)
                {
                    alternate = new List<DbColumnDeclaration>();
                    for (int j = 0; j < i; j++)
                    {
                        alternate.Add(select.Columns[j]);
                    }
                }
                if (decl != null && alternate != null)
                {
                    alternate.Add(decl);
                }
            }
            if (alternate != null)
            {
                columns = alternate.AsReadOnly();
            }

            Expression take = this.Visit(select.Take);
            Expression skip = this.Visit(select.Skip);
            ReadOnlyCollection<Expression> groupbys = this.VisitExpressionList(select.GroupBy);
            ReadOnlyCollection<DbOrderExpression> orderbys = this.VisitOrderBy(select.OrderBy);
            Expression where = this.Visit(select.Where);
            Expression from = this.Visit(select.From);

            ClearColumnsUsed(select.Alias);

            if (columns != select.Columns 
                || take != select.Take 
                || skip != select.Skip
                || orderbys != select.OrderBy 
                || groupbys != select.GroupBy
                || where != select.Where 
                || from != select.From)
            {
                select = new DbSelectExpression(select.Alias, columns, from, where, 
                    orderbys, groupbys, select.IsDistinct, skip, take);
            }

            return select;
        }

        protected override Expression VisitProjection(DbProjectionExpression projection)
        {
            // visit mapping in reverse order
            Expression projector = this.Visit(projection.Projector);
            DbSelectExpression select = (DbSelectExpression)this.Visit(projection.Select);
            return this.UpdateProjection(projection, select, projector, projection.Aggregator);
        }

        protected override Expression VisitClientJoin(DbClientJoinExpression join)
        {
            var innerKey = this.VisitExpressionList(join.InnerKey);
            var outerKey = this.VisitExpressionList(join.OuterKey);
            DbProjectionExpression projection = (DbProjectionExpression)this.Visit(join.Projection);
            if (projection != join.Projection || innerKey != join.InnerKey || outerKey != join.OuterKey)
            {
                return new DbClientJoinExpression(projection, outerKey, innerKey);
            }
            return join;
        }

        protected override Expression VisitJoin(DbJoinExpression join)
        {
            if (join.Join == DbJoinType.SingletonLeftOuter)
            {
                // first visit right side w/o looking at condition
                Expression right = this.Visit(join.Right);
                DbAliasedExpression ax = right as DbAliasedExpression;
                if (ax != null && !this.allColumnsUsed.ContainsKey(ax.Alias))
                {
                    // if nothing references the alias on the right, then the join is redundant
                    return this.Visit(join.Left);
                }
                // otherwise do it the right way
                Expression cond = this.Visit(join.Condition);
                Expression left = this.Visit(join.Left);
                right = this.Visit(join.Right);
                return this.UpdateJoin(join, join.Join, left, right, cond);
            }
            else
            {
                // visit join in reverse order
                Expression condition = this.Visit(join.Condition);
                Expression right = this.VisitSource(join.Right);
                Expression left = this.VisitSource(join.Left);
                return this.UpdateJoin(join, join.Join, left, right, condition);
            }
        }
    }
}
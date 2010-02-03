using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    /// <summary>
    /// rewrites nested projections into client-side joins
    /// </summary>
    public class ClientJoinedProjectionRewriter : DbExpressionVisitor
    {
        bool isTopLevel = true;
        QueryLanguage language;
        DbSelectExpression currentSelect;

        private ClientJoinedProjectionRewriter(QueryLanguage language)
        {
            this.language = language;
        }

        public static Expression Rewrite(QueryLanguage language, Expression expression)
        {
            return new ClientJoinedProjectionRewriter(language).Visit(expression);
        }

        protected override Expression VisitProjection(DbProjectionExpression proj)
        {
            DbSelectExpression save = this.currentSelect;
            this.currentSelect = proj.Select;
            try
            {
                if (!this.isTopLevel)
                {
                    if (this.CanJoinOnClient(this.currentSelect))
                    {
                        // make a query that combines all the constraints from the outer queries into a single select
                        DbSelectExpression newOuterSelect = (DbSelectExpression)QueryDuplicator.Duplicate(save);

                        // remap any references to the outer select to the new alias;
                        DbSelectExpression newInnerSelect = (DbSelectExpression)ColumnMapper.Map(proj.Select, newOuterSelect.Alias, save.Alias);
                        // add outer-join test
                        DbProjectionExpression newInnerProjection = new DbProjectionExpression(newInnerSelect, proj.Projector).AddOuterJoinTest();
                        newInnerSelect = newInnerProjection.Select;
                        Expression newProjector = newInnerProjection.Projector;

                        DbTableAlias newAlias = new DbTableAlias();
                        var pc = ColumnProjector.ProjectColumns(this.language.CanBeColumn, newProjector, newOuterSelect.Columns, newAlias, newOuterSelect.Alias, newInnerSelect.Alias);

                        DbJoinExpression join = new DbJoinExpression(DbJoinType.OuterApply, newOuterSelect, newInnerSelect, null);
                        DbSelectExpression joinedSelect = new DbSelectExpression(newAlias, pc.Columns, join, null, null, null, proj.IsSingleton, null, null);

                        // apply client-join treatment recursively
                        this.currentSelect = joinedSelect;
                        newProjector = this.Visit(pc.Projector); 

                        // compute keys (this only works if join condition was a single column comparison)
                        List<Expression> outerKeys = new List<Expression>();
                        List<Expression> innerKeys = new List<Expression>();
                        if (this.GetEquiJoinKeyExpressions(newInnerSelect.Where, newOuterSelect.Alias, outerKeys, innerKeys))
                        {
                            // outerKey needs to refer to the outer-scope's alias
                            var outerKey = outerKeys.Select(k => ColumnMapper.Map(k, save.Alias, newOuterSelect.Alias));
                            // innerKey needs to refer to the new alias for the select with the new join
                            var innerKey = innerKeys.Select(k => ColumnMapper.Map(k, joinedSelect.Alias, ((DbColumnExpression)k).Alias));
                            DbProjectionExpression newProjection = new DbProjectionExpression(joinedSelect, newProjector, proj.Aggregator);
                            return new DbClientJoinExpression(newProjection, outerKey, innerKey);
                        }
                    }
                }
                else
                {
                    this.isTopLevel = false;
                }

                return base.VisitProjection(proj);
            }
            finally 
            {
                this.currentSelect = save;
            }
        }

        private bool CanJoinOnClient(DbSelectExpression select)
        {
            // can add singleton (1:0,1) join if no grouping/aggregates or distinct
            return !select.IsDistinct
                && (select.GroupBy == null || select.GroupBy.Count == 0)
                && !AggregateChecker.HasAggregates(select);
        }

        private bool GetEquiJoinKeyExpressions(Expression predicate, DbTableAlias outerAlias, 
            List<Expression> outerExpressions, List<Expression> innerExpressions)
        {
            if (predicate.NodeType == ExpressionType.Equal)
            {
                var b = (BinaryExpression)predicate;
                DbColumnExpression leftCol = this.GetColumnExpression(b.Left);
                DbColumnExpression rightCol = this.GetColumnExpression(b.Right);
                if (leftCol != null && rightCol != null)
                {
                    if (leftCol.Alias == outerAlias)
                    {
                        outerExpressions.Add(b.Left);
                        innerExpressions.Add(b.Right);
                        return true;
                    }
                    else if (rightCol.Alias == outerAlias)
                    {
                        innerExpressions.Add(b.Left);
                        outerExpressions.Add(b.Right);
                        return true;
                    }
                }
            }

            bool hadKey = false;
            var parts = predicate.Split(ExpressionType.And, ExpressionType.AndAlso);
            if (parts.Length > 1)
            {
                foreach (var part in parts)
                {
                    bool hasOuterAliasReference = ReferencedAliasGatherer.Gather(part).Contains(outerAlias);
                    if (hasOuterAliasReference)
                    {
                        if (!GetEquiJoinKeyExpressions(part, outerAlias, outerExpressions, innerExpressions))
                            return false;
                        hadKey = true;
                    }
                }
            }

            return hadKey;
        }

        private DbColumnExpression GetColumnExpression(Expression expression)
        {
            // ignore converions 
            while (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked)
            {
                expression = ((UnaryExpression)expression).Operand;
            }
            return (expression as DbColumnExpression);
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
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
    /// Removes select expressions that don't add any additional semantic value
    /// </summary>
    public class RedundantSubqueryRemover : DbExpressionVisitor
    {
        private RedundantSubqueryRemover() 
        {
        }

        public static Expression Remove(Expression expression)
        {
            expression = new RedundantSubqueryRemover().Visit(expression);
            expression = SubqueryMerger.Merge(expression);
            return expression;
        }

        protected override Expression VisitSelect(DbSelectExpression select)
        {
            select = (DbSelectExpression)base.VisitSelect(select);

            // first remove all purely redundant subqueries
            List<DbSelectExpression> redundant = RedundantSubqueryGatherer.Gather(select.From);
            if (redundant != null)
            {
                select = SubqueryRemover.Remove(select, redundant);
            }

            return select;
        }

        protected override Expression VisitProjection(DbProjectionExpression proj)
        {
            proj = (DbProjectionExpression)base.VisitProjection(proj);
            if (proj.Select.From is DbSelectExpression) 
            {
                List<DbSelectExpression> redundant = RedundantSubqueryGatherer.Gather(proj.Select);
                if (redundant != null) 
                {
                    proj = SubqueryRemover.Remove(proj, redundant);
                }
            }
            return proj;
        }

        internal static bool IsSimpleProjection(DbSelectExpression select)
        {
            foreach (DbColumnDeclaration decl in select.Columns)
            {
                DbColumnExpression col = decl.Expression as DbColumnExpression;
                if (col == null || decl.Name != col.Name)
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool IsNameMapProjection(DbSelectExpression select)
        {
            if (select.From is DbTableExpression) return false;
            DbSelectExpression fromSelect = select.From as DbSelectExpression;
            if (fromSelect == null || select.Columns.Count != fromSelect.Columns.Count)
                return false;
            ReadOnlyCollection<DbColumnDeclaration> fromColumns = fromSelect.Columns;
            // test that all columns in 'select' are refering to columns in the same position
            // in from.
            for (int i = 0, n = select.Columns.Count; i < n; i++)
            {
                DbColumnExpression col = select.Columns[i].Expression as DbColumnExpression;
                if (col == null || !(col.Name == fromColumns[i].Name))
                    return false;
            }
            return true;
        }

        internal static bool IsInitialProjection(DbSelectExpression select)
        {
            return select.From is DbTableExpression;
        }

        class RedundantSubqueryGatherer : DbExpressionVisitor
        {
            List<DbSelectExpression> redundant;

            private RedundantSubqueryGatherer()
            {
            }

            internal static List<DbSelectExpression> Gather(Expression source)
            {
                RedundantSubqueryGatherer gatherer = new RedundantSubqueryGatherer();
                gatherer.Visit(source);
                return gatherer.redundant;
            }

            private static bool IsRedudantSubquery(DbSelectExpression select)
            {
                return (IsSimpleProjection(select) || IsNameMapProjection(select))
                    && !select.IsDistinct
                    && select.Take == null
                    && select.Skip == null
                    && select.Where == null
                    && (select.OrderBy == null || select.OrderBy.Count == 0)
                    && (select.GroupBy == null || select.GroupBy.Count == 0);
            }

            protected override Expression VisitSelect(DbSelectExpression select)
            {
                if (IsRedudantSubquery(select))
                {
                    if (this.redundant == null)
                    {
                        this.redundant = new List<DbSelectExpression>();
                    }
                    this.redundant.Add(select);
                }
                return select;
            }

            protected override Expression VisitSubquery(DbSubqueryExpression subquery)
            {
                // don't gather inside scalar & exists
                return subquery;
            }
        }

        class SubqueryMerger : DbExpressionVisitor
        {
            private SubqueryMerger()
            {
            }

            internal static Expression Merge(Expression expression)
            {
                return new SubqueryMerger().Visit(expression);
            }

            bool isTopLevel = true;

            protected override Expression VisitSelect(DbSelectExpression select)
            {
                bool wasTopLevel = isTopLevel;
                isTopLevel = false;

                select = (DbSelectExpression)base.VisitSelect(select);

                // next attempt to merge subqueries that would have been removed by the above
                // logic except for the existence of a where clause
                while (CanMergeWithFrom(select, wasTopLevel))
                {
                    DbSelectExpression fromSelect = GetLeftMostSelect(select.From);

                    // remove the redundant subquery
                    select = SubqueryRemover.Remove(select, fromSelect);

                    // merge where expressions 
                    Expression where = select.Where;
                    if (fromSelect.Where != null)
                    {
                        if (where != null)
                        {
                            where = fromSelect.Where.And(where);
                        }
                        else
                        {
                            where = fromSelect.Where;
                        }
                    }
                    var orderBy = select.OrderBy != null && select.OrderBy.Count > 0 ? select.OrderBy : fromSelect.OrderBy;
                    var groupBy = select.GroupBy != null && select.GroupBy.Count > 0 ? select.GroupBy : fromSelect.GroupBy;
                    Expression skip = select.Skip != null ? select.Skip : fromSelect.Skip;
                    Expression take = select.Take != null ? select.Take : fromSelect.Take;
                    bool isDistinct = select.IsDistinct | fromSelect.IsDistinct;

                    if (where != select.Where
                        || orderBy != select.OrderBy
                        || groupBy != select.GroupBy
                        || isDistinct != select.IsDistinct
                        || skip != select.Skip
                        || take != select.Take)
                    {
                        select = new DbSelectExpression(select.Alias, select.Columns, select.From, where, 
                            orderBy, groupBy, isDistinct, skip, take);
                    }
                }

                return select;
            }

            private static DbSelectExpression GetLeftMostSelect(Expression source)
            {
                DbSelectExpression select = source as DbSelectExpression;
                if (select != null) return select;
                DbJoinExpression join = source as DbJoinExpression;
                if (join != null) return GetLeftMostSelect(join.Left);
                return null;
            }

            private static bool IsColumnProjection(DbSelectExpression select)
            {
                for (int i = 0, n = select.Columns.Count; i < n; i++)
                {
                    var cd = select.Columns[i];
                    if (cd.Expression.NodeType != (ExpressionType)DbExpressionType.Column &&
                        cd.Expression.NodeType != ExpressionType.Constant)
                        return false;
                }
                return true;
            }

            private static bool CanMergeWithFrom(DbSelectExpression select, bool isTopLevel)
            {
                DbSelectExpression fromSelect = GetLeftMostSelect(select.From);
                if (fromSelect == null)
                    return false;
                if (!IsColumnProjection(fromSelect))
                    return false;
                bool selHasNameMapProjection = IsNameMapProjection(select);
                bool selHasOrderBy = select.OrderBy != null && select.OrderBy.Count > 0;
                bool selHasGroupBy = select.GroupBy != null && select.GroupBy.Count > 0;
                bool selHasAggregates = AggregateChecker.HasAggregates(select);
                bool frmHasOrderBy = fromSelect.OrderBy != null && fromSelect.OrderBy.Count > 0;
                bool frmHasGroupBy = fromSelect.GroupBy != null && fromSelect.GroupBy.Count > 0;
                // both cannot have orderby
                if (selHasOrderBy && frmHasOrderBy)
                    return false;
                // both cannot have groupby
                if (selHasGroupBy && frmHasGroupBy)
                    return false;
                // cannot move forward order-by if outer has group-by
                if (frmHasOrderBy && (selHasGroupBy || selHasAggregates || select.IsDistinct))
                    return false;
                // cannot move forward group-by if outer has where clause
                if (frmHasGroupBy /*&& (select.Where != null)*/) // need to assert projection is the same in order to move group-by forward
                    return false;
                // cannot move forward a take if outer has take or skip or distinct
                if (fromSelect.Take != null && (select.Take != null || select.Skip != null || select.IsDistinct || selHasAggregates || selHasGroupBy))
                    return false;
                // cannot move forward a skip if outer has skip or distinct
                if (fromSelect.Skip != null && (select.Skip != null || select.IsDistinct || selHasAggregates || selHasGroupBy))
                    return false;
                // cannot move forward a distinct if outer has take, skip, groupby or a different projection
                if (fromSelect.IsDistinct && (select.Take != null || select.Skip != null || !selHasNameMapProjection || 
                    selHasGroupBy || selHasAggregates || (selHasOrderBy && !isTopLevel)))
                    return false;
                return true;
            }
        }
    }
}
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
    /// Attempts to rewrite cross-apply and outer-apply joins as inner and left-outer joins
    /// </summary>
    public class CrossApplyRewriter : DbExpressionVisitor
    {
        public static Expression Rewrite(Expression expression)
        {
            return new CrossApplyRewriter().Visit(expression);
        }

        protected override Expression VisitJoin(DbJoinExpression join)
        {
            join = (DbJoinExpression)base.VisitJoin(join);

            if (join.Join == DbJoinType.CrossApply || join.Join == DbJoinType.OuterApply)
            {
                if (join.Right is DbTableExpression)
                {
                    return new DbJoinExpression(DbJoinType.CrossJoin, join.Left, join.Right, null);
                }
                else
                {
                    DbSelectExpression select = join.Right as DbSelectExpression;
                    // Only consider rewriting cross apply if 
                    //   1) right side is a select
                    //   2) other than in the where clause in the right-side select, no left-side declared aliases are referenced
                    //   3) and has no behavior that would change semantics if the where clause is removed (like groups, aggregates, take, skip, etc).
                    // Note: it is best to attempt this after redundant subqueries have been removed.
                    if (select != null
                        && select.Take == null
                        && select.Skip == null
                        && !AggregateChecker.HasAggregates(select)
                        && (select.GroupBy == null || select.GroupBy.Count == 0))
                    {
                        DbSelectExpression selectWithoutWhere = select.SetWhere(null);
                        HashSet<DbTableAlias> referencedAliases = ReferencedAliasGatherer.Gather(selectWithoutWhere);
                        HashSet<DbTableAlias> declaredAliases = DeclaredAliasGatherer.Gather(join.Left);
                        referencedAliases.IntersectWith(declaredAliases);
                        if (referencedAliases.Count == 0)
                        {
                            Expression where = select.Where;
                            select = selectWithoutWhere;
                            var pc = ColumnProjector.ProjectColumns(this.CanBeColumn, where, select.Columns, select.Alias, DeclaredAliasGatherer.Gather(select.From));
                            select = select.SetColumns(pc.Columns);
                            where = pc.Projector;
                            DbJoinType jt = (where == null) ? DbJoinType.CrossJoin : 
                                (join.Join == DbJoinType.CrossApply ? DbJoinType.InnerJoin : DbJoinType.LeftOuter);
                            return new DbJoinExpression(jt, join.Left, select, where);
                        }
                    }
                }
            }

            return join;
        }

        private bool CanBeColumn(Expression expr)
        {
            return expr != null && expr.NodeType == (ExpressionType)DbExpressionType.Column;
        }
    }
}
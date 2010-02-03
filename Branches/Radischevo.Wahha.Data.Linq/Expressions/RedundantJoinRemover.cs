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
    /// Removes joins expressions that are identical to joins that already exist
    /// </summary>
    public class RedundantJoinRemover : DbExpressionVisitor
    {
        Dictionary<DbTableAlias, DbTableAlias> map;

        private RedundantJoinRemover()
        {
            this.map = new Dictionary<DbTableAlias, DbTableAlias>();
        }

        public static Expression Remove(Expression expression)
        {
            return new RedundantJoinRemover().Visit(expression);
        }

        protected override Expression VisitJoin(DbJoinExpression join)
        {
            Expression result = base.VisitJoin(join);
            join = result as DbJoinExpression;
            if (join != null)
            {
                DbAliasedExpression right = join.Right as DbAliasedExpression;
                if (right != null)
                {
                    DbAliasedExpression similarRight = (DbAliasedExpression)this.FindSimilarRight(join.Left as DbJoinExpression, join);
                    if (similarRight != null)
                    {
                        this.map.Add(right.Alias, similarRight.Alias);
                        return join.Left;
                    }
                }
            }
            return result;
        }

        private Expression FindSimilarRight(DbJoinExpression join, DbJoinExpression compareTo)
        {
            if (join == null)
                return null;
            if (join.Join == compareTo.Join)
            {
                if (join.Right.NodeType == compareTo.Right.NodeType
                    && DbExpressionComparer.AreEqual(join.Right, compareTo.Right))
                {
                    if (join.Condition == compareTo.Condition)
                        return join.Right;
                    var scope = new ScopedDictionary<DbTableAlias, DbTableAlias>(null);
                    scope.Add(((DbAliasedExpression)join.Right).Alias, ((DbAliasedExpression)compareTo.Right).Alias);
                    if (DbExpressionComparer.AreEqual(null, scope, join.Condition, compareTo.Condition))
                        return join.Right;
                }
            }
            Expression result = FindSimilarRight(join.Left as DbJoinExpression, compareTo);
            if (result == null)
            {
                result = FindSimilarRight(join.Right as DbJoinExpression, compareTo);
            }
            return result;
        }

        protected override Expression VisitColumn(DbColumnExpression column)
        {
            DbTableAlias mapped;
            if (this.map.TryGetValue(column.Alias, out mapped))
            {
                return new DbColumnExpression(column.Type, column.DbType, mapped, column.Name);
            }
            return column;
        }
    }
}

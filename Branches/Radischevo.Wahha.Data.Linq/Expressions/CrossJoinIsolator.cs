using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    /// <summary>
    /// Isolates cross joins from other types of joins using nested sub queries
    /// </summary>
    public class CrossJoinIsolator : DbExpressionVisitor
    {
        ILookup<DbTableAlias, DbColumnExpression> columns;
        Dictionary<DbColumnExpression, DbColumnExpression> map = new Dictionary<DbColumnExpression, DbColumnExpression>();
        DbJoinType? lastJoin;

        public static Expression Isolate(Expression expression)
        {
            return new CrossJoinIsolator().Visit(expression);
        }

        protected override Expression VisitSelect(DbSelectExpression select)
        {
            var saveColumns = this.columns;
            this.columns = ReferencedColumnGatherer.Gather(select).ToLookup(c => c.Alias);
            var saveLastJoin = this.lastJoin;
            this.lastJoin = null;
            var result = base.VisitSelect(select);
            this.columns = saveColumns;
            this.lastJoin = saveLastJoin;
            return result;
        }

        protected override Expression VisitJoin(DbJoinExpression join)
        {
            var saveLastJoin = this.lastJoin;
            this.lastJoin = join.Join;
            join = (DbJoinExpression)base.VisitJoin(join);
            this.lastJoin = saveLastJoin;

            if (this.lastJoin != null && (join.Join == DbJoinType.CrossJoin) != (this.lastJoin == DbJoinType.CrossJoin))
            {
                var result = this.MakeSubquery(join);
                return result;
            }
            return join;
        }

        private bool IsCrossJoin(Expression expression)
        {
            var jex = expression as DbJoinExpression;
            if (jex != null)
            {
                return jex.Join == DbJoinType.CrossJoin;
            }
            return false;
        }

        private Expression MakeSubquery(Expression expression)
        {
            var newAlias = new DbTableAlias();
            var aliases = DeclaredAliasGatherer.Gather(expression);

            var decls = new List<DbColumnDeclaration>();
            foreach (var ta in aliases) 
            {
                foreach (var col in this.columns[ta])
                {
                    string name = decls.GetAvailableColumnName(col.Name);
                    var decl = new DbColumnDeclaration(name, col);
                    decls.Add(decl);
                    var newCol = new DbColumnExpression(col.Type, col.DbType, newAlias, col.Name);
                    this.map.Add(col, newCol);
                }
            }

            return new DbSelectExpression(newAlias, decls, expression, null);
        }

        protected override Expression VisitColumn(DbColumnExpression column)
        {
            DbColumnExpression mapped;
            if (this.map.TryGetValue(column, out mapped))
            {
                return mapped;
            }
            return column;
        }
    }
}
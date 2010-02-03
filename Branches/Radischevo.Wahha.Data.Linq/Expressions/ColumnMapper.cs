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
    /// Rewrite all column references to one or more aliases to a new single alias
    /// </summary>
    public class ColumnMapper : DbExpressionVisitor
    {
        HashSet<DbTableAlias> oldAliases;
        DbTableAlias newAlias;

        private ColumnMapper(IEnumerable<DbTableAlias> oldAliases, DbTableAlias newAlias)
        {
            this.oldAliases = new HashSet<DbTableAlias>(oldAliases);
            this.newAlias = newAlias;
        }

        public static Expression Map(Expression expression, DbTableAlias newAlias, IEnumerable<DbTableAlias> oldAliases)
        {
            return new ColumnMapper(oldAliases, newAlias).Visit(expression);
        }

        public static Expression Map(Expression expression, DbTableAlias newAlias, params DbTableAlias[] oldAliases)
        {
            return Map(expression, newAlias, (IEnumerable<DbTableAlias>)oldAliases);
        }

        protected override Expression VisitColumn(DbColumnExpression column)
        {
            if (this.oldAliases.Contains(column.Alias))
            {
                return new DbColumnExpression(column.Type, column.DbType, this.newAlias, column.Name);
            }
            return column;
        }
    }
}

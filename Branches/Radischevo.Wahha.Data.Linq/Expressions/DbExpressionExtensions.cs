using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public static class DbExpressionExtensions
    {
        public static DbSelectExpression SetColumns(this DbSelectExpression select, IEnumerable<DbColumnDeclaration> columns)
        {
            return new DbSelectExpression(select.Alias, columns.OrderBy(c => c.Name), select.From, select.Where, 
                select.OrderBy, select.GroupBy, select.IsDistinct, select.Skip, select.Take);
        }

        public static DbSelectExpression AddColumn(this DbSelectExpression select, DbColumnDeclaration column)
        {
            List<DbColumnDeclaration> columns = new List<DbColumnDeclaration>(select.Columns);
            columns.Add(column);
            return select.SetColumns(columns);
        }

        public static DbSelectExpression RemoveColumn(this DbSelectExpression select, DbColumnDeclaration column)
        {
            List<DbColumnDeclaration> columns = new List<DbColumnDeclaration>(select.Columns);
            columns.Remove(column);
            return select.SetColumns(columns);
        }

        public static string GetAvailableColumnName(this IList<DbColumnDeclaration> columns, string baseName)
        {
            string name = baseName;
            int n = 0;
            while (!IsUniqueName(columns, name))
            {
                name = baseName + (n++);
            }
            return name;
        }

        private static bool IsUniqueName(IList<DbColumnDeclaration> columns, string name)
        {
            foreach (var col in columns)
            {
                if (col.Name == name)
                {
                    return false;
                }
            }
            return true;
        }

        public static DbProjectionExpression AddOuterJoinTest(this DbProjectionExpression proj)
        {
            string colName = proj.Select.Columns.GetAvailableColumnName("Test");
            DbSelectExpression newSource = proj.Select.AddColumn(new DbColumnDeclaration(colName, Expression.Constant(1, typeof(int?))));
            Expression newProjector =
                new DbOuterJoinedExpression(
                    new DbColumnExpression(typeof(int?), null, newSource.Alias, colName),
                    proj.Projector
                    );
            return new DbProjectionExpression(newSource, newProjector, proj.Aggregator);
        }

        public static DbSelectExpression SetDistinct(this DbSelectExpression select, bool isDistinct)
        {
            if (select.IsDistinct != isDistinct)
            {
                return new DbSelectExpression(select.Alias, select.Columns, select.From, select.Where, select.OrderBy, 
                    select.GroupBy, isDistinct, select.Skip, select.Take);
            }
            return select;
        }

        public static DbSelectExpression SetWhere(this DbSelectExpression select, Expression where)
        {
            if (where != select.Where)
            {
                return new DbSelectExpression(select.Alias, select.Columns, select.From, where, select.OrderBy, select.GroupBy, select.IsDistinct, select.Skip, select.Take);
            }
            return select;
        }

        public static DbSelectExpression SetOrderBy(this DbSelectExpression select, IEnumerable<DbOrderExpression> orderBy)
        {
            return new DbSelectExpression(select.Alias, select.Columns, select.From, select.Where, orderBy, 
                select.GroupBy, select.IsDistinct, select.Skip, select.Take);
        }

        public static DbSelectExpression AddOrderExpression(this DbSelectExpression select, DbOrderExpression ordering)
        {
            List<DbOrderExpression> orderby = new List<DbOrderExpression>();
            if (select.OrderBy != null)
                orderby.AddRange(select.OrderBy);
            orderby.Add(ordering);
            return select.SetOrderBy(orderby);
        }

        public static DbSelectExpression RemoveOrderExpression(this DbSelectExpression select, DbOrderExpression ordering)
        {
            if (select.OrderBy != null && select.OrderBy.Count > 0)
            {
                List<DbOrderExpression> orderby = new List<DbOrderExpression>(select.OrderBy);
                orderby.Remove(ordering);
                return select.SetOrderBy(orderby);
            }
            return select;
        }

        public static DbSelectExpression SetGroupBy(this DbSelectExpression select, IEnumerable<Expression> groupBy)
        {
            return new DbSelectExpression(select.Alias, select.Columns, select.From, select.Where, 
                select.OrderBy, groupBy, select.IsDistinct, select.Skip, select.Take);
        }

        public static DbSelectExpression AddGroupExpression(this DbSelectExpression select, Expression expression)
        {
            List<Expression> groupby = new List<Expression>();
            if (select.GroupBy != null)
                groupby.AddRange(select.GroupBy);
            groupby.Add(expression);
            return select.SetGroupBy(groupby);
        }

        public static DbSelectExpression RemoveGroupExpression(this DbSelectExpression select, Expression expression)
        {
            if (select.GroupBy != null && select.GroupBy.Count > 0)
            {
                List<Expression> groupby = new List<Expression>(select.GroupBy);
                groupby.Remove(expression);
                return select.SetGroupBy(groupby);
            }
            return select;
        }

        public static DbSelectExpression SetSkip(this DbSelectExpression select, Expression skip)
        {
            if (skip != select.Skip)
            {
                return new DbSelectExpression(select.Alias, select.Columns, select.From, select.Where, select.OrderBy, 
                    select.GroupBy, select.IsDistinct, skip, select.Take);
            }
            return select;
        }

        public static DbSelectExpression SetTake(this DbSelectExpression select, Expression take)
        {
            if (take != select.Take)
            {
                return new DbSelectExpression(select.Alias, select.Columns, select.From, select.Where, 
                    select.OrderBy, select.GroupBy, select.IsDistinct, select.Skip, take);
            }
            return select;
        }

        public static DbSelectExpression AddRedundantSelect(this DbSelectExpression select, DbTableAlias newAlias)
        {
            var newColumns = select.Columns.Select(d =>
                new DbColumnDeclaration(d.Name,
                    new DbColumnExpression(
                        d.Expression.Type,
                        (d.Expression is DbColumnExpression) ? ((DbColumnExpression)d.Expression).DbType : null,
                        newAlias, d.Name
                        )));
            var newFrom = new DbSelectExpression(newAlias, select.Columns, select.From, select.Where, select.OrderBy, 
                select.GroupBy, select.IsDistinct, select.Skip, select.Take);
            return new DbSelectExpression(select.Alias, newColumns, newFrom, null, null, null, false, null, null);
        }

        public static DbSelectExpression RemoveRedundantFrom(this DbSelectExpression select)
        {
            DbSelectExpression fromSelect = select.From as DbSelectExpression;
            if (fromSelect != null)
            {
                return SubqueryRemover.Remove(select, fromSelect);
            }
            return select;
        }

        public static DbSelectExpression SetFrom(this DbSelectExpression select, Expression from)
        {
            if (select.From != from)
            {
                return new DbSelectExpression(select.Alias, select.Columns, from, select.Where, select.OrderBy, 
                    select.GroupBy, select.IsDistinct, select.Skip, select.Take);
            }
            return select;
        }
    }
}

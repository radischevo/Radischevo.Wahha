using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

using Jeltofiol.Wahha.Core;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    /// <summary>
    /// A custom expression node used to 
    /// represent an SQL SELECT expression
    /// </summary>
    public class DbSelectExpression : DbAliasedExpression
    {
        #region Instance Fields
        private bool _isDistinct;
        private ReadOnlyCollection<DbColumnDeclaration> _columns;
        private Expression _from;
        private Expression _where;
        private ReadOnlyCollection<DbOrderExpression> _orderBy;
        private ReadOnlyCollection<Expression> _groupBy;
        private Expression _take;
        private Expression _skip;
        #endregion

        #region Constructors
        public DbSelectExpression(DbTableAlias alias,
            IEnumerable<DbColumnDeclaration> columns,
            Expression from, Expression where,
            IEnumerable<DbOrderExpression> orderBy,
            IEnumerable<Expression> groupBy,
            bool isDistinct, Expression skip, Expression take)
            : base(DbExpressionType.Select, typeof(void), alias)
        {
            _isDistinct = isDistinct;
            _columns = columns.AsReadOnly();
            _from = from;
            _where = where;
            _orderBy = orderBy.AsReadOnly();
            _groupBy = groupBy.AsReadOnly();
            _take = take;
            _skip = skip;
        }

        public DbSelectExpression(DbTableAlias alias,
            IEnumerable<DbColumnDeclaration> columns,
            Expression from, Expression where,
            IEnumerable<DbOrderExpression> orderBy,
            IEnumerable<Expression> groupBy)
            : this(alias, columns, from, where, orderBy, 
                groupBy, false, null, null)
        {
        }

        public DbSelectExpression(DbTableAlias alias, 
            IEnumerable<DbColumnDeclaration> columns,
            Expression from, Expression where)
            : this(alias, columns, from, where, null, null)
        {
        }
        #endregion

        #region Instance Properties
        public ReadOnlyCollection<DbColumnDeclaration> Columns
        {
            get 
            { 
                return _columns; 
            }
        }
        public Expression From
        {
            get 
            { 
                return _from; 
            }
        }
        public Expression Where
        {
            get 
            { 
                return _where; 
            }
        }
        public ReadOnlyCollection<DbOrderExpression> OrderBy
        {
            get 
            { 
                return _orderBy; 
            }
        }
        public ReadOnlyCollection<Expression> GroupBy
        {
            get 
            { 
                return _groupBy; 
            }
        }
        public bool IsDistinct
        {
            get 
            { 
                return _isDistinct; 
            }
        }
        public Expression Skip
        {
            get 
            { 
                return _skip; 
            }
        }
        public Expression Take
        {
            get 
            { 
                return _take; 
            }
        }
        #endregion
    }
}

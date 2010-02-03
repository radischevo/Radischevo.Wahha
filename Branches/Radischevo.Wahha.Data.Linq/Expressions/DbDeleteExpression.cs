using System;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public class DbDeleteExpression : DbStatementExpression
    {
        #region Instance Fields
        private DbTableExpression _table;
        private Expression _where;
        #endregion

        #region Constructors
        public DbDeleteExpression(DbTableExpression table, Expression where)
            : base(DbExpressionType.Delete, typeof(int))
        {
            _table = table;
            _where = where;
        }
        #endregion

        #region Instance Properties
        public DbTableExpression Table
        {
            get 
            { 
                return _table; 
            }
        }

        public Expression Where
        {
            get 
            { 
                return _where; 
            }
        }
        #endregion
    }
}

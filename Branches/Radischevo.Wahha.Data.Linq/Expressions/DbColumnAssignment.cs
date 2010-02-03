using System;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public class DbColumnAssignment
    {
        #region Instance Fields
        private DbColumnExpression _column;
        private Expression _expression;
        #endregion

        #region Constructors
        public DbColumnAssignment(
            DbColumnExpression column, Expression expression)
        {
            _column = column;
            _expression = expression;
        }
        #endregion

        #region Instance Properties
        public DbColumnExpression Column
        {
            get 
            { 
                return _column; 
            }
        }

        public Expression Expression
        {
            get 
            { 
                return _expression; 
            }
        }
        #endregion
    }
}

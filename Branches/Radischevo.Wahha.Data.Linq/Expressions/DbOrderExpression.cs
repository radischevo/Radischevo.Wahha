using System;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    /// <summary>
    /// A pairing of an expression and an 
    /// order type for use in a SQL Order By clause
    /// </summary>
    public class DbOrderExpression
    {
        #region Instance Fields
        private DbOrderType _orderType;
        private Expression _expression;
        #endregion

        #region Constructors
        public DbOrderExpression(DbOrderType orderType, Expression expression)
        {
            _orderType = orderType;
            _expression = expression;
        }
        #endregion

        #region Instance Properties
        public DbOrderType OrderType
        {
            get 
            { 
                return _orderType; 
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

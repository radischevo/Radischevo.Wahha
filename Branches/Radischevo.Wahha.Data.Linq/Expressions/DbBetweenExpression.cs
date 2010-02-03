using System;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public class DbBetweenExpression : DbExpression
    {
        #region Instance Fields
        private Expression _expression;
        private Expression _lower;
        private Expression _upper;
        #endregion

        #region Constructors
        public DbBetweenExpression(Expression expression, 
            Expression lower, Expression upper)
            : base(DbExpressionType.Between, expression.Type)
        {
            _expression = expression;
            _lower = lower;
            _upper = upper;
        }
        #endregion

        #region Instance Properties
        public Expression Expression
        {
            get 
            { 
                return _expression; 
            }
        }
        public Expression Lower
        {
            get 
            { 
                return _lower; 
            }
        }
        public Expression Upper
        {
            get 
            { 
                return _upper; 
            }
        }
        #endregion
    }
}

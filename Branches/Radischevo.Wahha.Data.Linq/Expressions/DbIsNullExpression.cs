using System;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    /// <summary>
    /// Allows is-null tests against 
    /// value-types like int and float
    /// </summary>
    public class DbIsNullExpression : DbExpression
    {
        #region Instance Fields
        private Expression _expression;
        #endregion

        #region Constructors
        public DbIsNullExpression(Expression expression)
            : base(DbExpressionType.IsNull, typeof(bool))
        {
            _expression = expression;
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
        #endregion
    }
}

using System;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public class DbConditionalExpression : DbStatementExpression
    {
        #region Instance Fields
        private Expression _check;
        private Expression _ifTrue;
        private Expression _ifFalse;
        #endregion

        #region Constructors
        public DbConditionalExpression(Expression check, Expression ifTrue, Expression ifFalse)
            : base(DbExpressionType.If, ifTrue.Type)
        {
            _check = check;
            _ifTrue = ifTrue;
            _ifFalse = ifFalse;
        }
        #endregion

        #region Instance Properties
        public Expression Check
        {
            get 
            { 
                return _check; 
            }
        }

        public Expression IfTrue
        {
            get 
            { 
                return _ifTrue; 
            }
        }

        public Expression IfFalse
        {
            get 
            { 
                return _ifFalse; 
            }
        }
        #endregion
    }
}

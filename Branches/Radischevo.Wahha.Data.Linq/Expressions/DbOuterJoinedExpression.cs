using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public class DbOuterJoinedExpression : DbExpression
    {
        #region Instance Fields
        private Expression _test;
        private Expression _expression;
        #endregion

        #region Constructors
        public DbOuterJoinedExpression(Expression test, Expression expression)
            : base(DbExpressionType.OuterJoined, expression.Type)
        {
            _test = test;
            _expression = expression;
        }
        #endregion

        #region Instance Properties
        public Expression Test
        {
            get 
            { 
                return _test; 
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

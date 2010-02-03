using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    /// <summary>
    /// A custom expression node 
    /// representing a SQL join clause
    /// </summary>
    public class DbJoinExpression : DbExpression
    {
        #region Instance Fields
        private DbJoinType _join;
        private Expression _left;
        private Expression _right;
        private Expression _condition;
        #endregion

        #region Constructors
        public DbJoinExpression(DbJoinType type, Expression left, Expression right, Expression condition)
            : base(DbExpressionType.Join, typeof(void))
        {
            _join = type;
            _left = left;
            _right = right;
            _condition = condition;
        }
        #endregion

        #region Instance Properties
        public DbJoinType Join
        {
            get 
            { 
                return _join; 
            }
        }
        
        public Expression Left
        {
            get 
            { 
                return _left; 
            }
        }
        
        public Expression Right
        {
            get 
            { 
                return _right; 
            }
        }

        public Expression Condition
        {
            get 
            { 
                return _condition; 
            }
        }
        #endregion
    }
}

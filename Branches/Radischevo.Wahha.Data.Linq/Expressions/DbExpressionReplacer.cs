using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    /// <summary>
    /// Replaces references to one specific instance of 
    /// an expression node with another node.
    /// Supports DbExpression nodes
    /// </summary>
    public class DbExpressionReplacer : DbExpressionVisitor
    {
        #region Instance Fields
        private Expression _searchFor;
        private Expression _replaceWith;
        #endregion

        #region Constructors
        private DbExpressionReplacer(Expression searchFor, Expression replaceWith)
        {
            _searchFor = searchFor;
            _replaceWith = replaceWith;
        }
        #endregion

        #region Static Methods
        public static Expression Replace(Expression expression, 
            Expression searchFor, Expression replaceWith)
        {
            return new DbExpressionReplacer(searchFor, replaceWith).Visit(expression);
        }

        public static Expression ReplaceAll(Expression expression, 
            Expression[] searchFor, Expression[] replaceWith)
        {
            for (int i = 0, n = searchFor.Length; i < n; i++)
                expression = Replace(expression, searchFor[i], replaceWith[i]);
            
            return expression;
        }
        #endregion

        #region Instance Methods
        protected override Expression Visit(Expression exp)
        {
            if (exp == _searchFor)
                return _replaceWith;
            
            return base.Visit(exp);
        }
        #endregion
    }
}

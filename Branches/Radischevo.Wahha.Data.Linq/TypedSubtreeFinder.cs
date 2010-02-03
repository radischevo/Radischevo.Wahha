using System;
using System.Linq;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq
{
    /// <summary>
    /// Finds the first sub-expression that is of a specified type
    /// </summary>
    internal class TypedSubtreeFinder : ExpressionVisitor
    {
        #region Instance Fields
        private Expression _root;
        private Type _type;
        #endregion

        #region Constructors
        private TypedSubtreeFinder(Type type)
        {
            _type = type;
        }
        #endregion

        #region Static Methods
        public static Expression Find(Expression expression, Type type)
        {
            TypedSubtreeFinder finder = new TypedSubtreeFinder(type);
            finder.Visit(expression);

            return finder._root;
        }
        #endregion

        #region Instance Methods
        protected override Expression Visit(Expression exp)
        {
            Expression result = base.Visit(exp);
            // remember the first sub-expression that produces an IQueryable
            if (_root == null && result != null && 
                _type.IsAssignableFrom(result.Type))
                _root = result;

            return result;
        }
        #endregion
    }
}

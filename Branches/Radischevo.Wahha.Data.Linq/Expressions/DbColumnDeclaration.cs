using System;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    /// <summary>
    /// A declaration of a column in 
    /// a SQL SELECT expression
    /// </summary>
    public class DbColumnDeclaration
    {
        #region Instance Fields
        private string _name;
        private Expression _expression;
        #endregion

        #region Constructors
        public DbColumnDeclaration(string name, Expression expression)
        {
            _name = name;
            _expression = expression;
        }
        #endregion

        #region Instance Properties
        public string Name
        {
            get 
            { 
                return _name; 
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

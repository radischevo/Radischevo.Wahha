using System;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public class DbVariableDeclaration
    {
        #region Instance Fields
        private string _name;
        private DbDataType _type;
        private Expression _expression;
        #endregion

        #region Constructors
        public DbVariableDeclaration(
            string name, DbDataType type, Expression expression)
        {
            _name = name;
            _type = type;
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

        public DbDataType DbType
        {
            get
            {
                return _type;
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

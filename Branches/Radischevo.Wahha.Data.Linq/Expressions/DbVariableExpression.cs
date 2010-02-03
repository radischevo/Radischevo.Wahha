using System;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public class DbVariableExpression : Expression
    {
        #region Instance Fields
        private string _name;
        private DbDataType _dbType;
        #endregion

        #region Constructors
        public DbVariableExpression(string name, Type type, DbDataType dbType)
            : base((ExpressionType)DbExpressionType.Variable, type)
        {
            _name = name;
            _dbType = dbType;
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
                return _dbType; 
            }
        }
        #endregion
    }
}

using System;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public class DbNamedValueExpression : DbExpression
    {
        #region Instance Fields
        private string _name;
        private DbDataType _dbType;
        private Expression _value;
        #endregion

        #region Constructors
        public DbNamedValueExpression(string name, 
            DbDataType dbType, Expression value)
            : base(DbExpressionType.NamedValue, value.Type)
        {
            _name = name;
            _dbType = dbType;
            _value = value;
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

        public Expression Value
        {
            get 
            { 
                return _value; 
            }
        }
        #endregion
    }
}

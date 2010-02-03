using System;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    /// <summary>
    /// A custom expression node that represents a 
    /// reference to a column in a SQL query
    /// </summary>
    public class DbColumnExpression : 
        DbExpression, IEquatable<DbColumnExpression>
    {
        #region Instance Fields
        private DbTableAlias _alias;
        private DbDataType _dbType;
        private string _name;
        #endregion

        #region Constructors
        public DbColumnExpression(Type type, DbDataType dbType, 
            DbTableAlias alias, string name)
            : base(DbExpressionType.Column, type)
        {
            _alias = alias;
            _name = name;
            _dbType = dbType;
        }
        #endregion

        #region Instance Properties
        public DbTableAlias Alias
        {
            get 
            { 
                return _alias; 
            }
        }

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

        #region Instance Methods
        public override string ToString()
        {
            return String.Format("{0}.C({1})", _alias.ToString(), _name);
        }

        public override int GetHashCode()
        {
            return _alias.GetHashCode() + _name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DbColumnExpression);
        }

        public bool Equals(DbColumnExpression other)
        {
            return (other != null
                && ((object)this) == (object)other
                 || (_alias == other._alias && _name == other.Name));
        }
        #endregion
    }
}

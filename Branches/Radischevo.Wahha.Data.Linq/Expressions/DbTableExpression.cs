using System;
using Jeltofiol.Wahha.Data.Linq.Mapping;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    /// <summary>
    /// A custom expression node that represents 
    /// a table reference in a SQL query
    /// </summary>
    public class DbTableExpression : DbAliasedExpression
    {
        #region Instance Fields
        private MetaType _type;
        private string _name;
        #endregion

        #region Constructors
        public DbTableExpression(DbTableAlias alias,
            MetaType type, string name)
            : base(DbExpressionType.Table, typeof(void), alias)
        {
            _type = type;
            _name = name;
        }
        #endregion

        #region Instance Properties
        public MetaType Type
        {
            get 
            { 
                return _type; 
            }
        }

        public string Name
        {
            get 
            { 
                return _name; 
            }
        }
        #endregion

        #region Instance Methods
        public override string ToString()
        {
            return "T(" + _name + ")";
        }
        #endregion
    }
}

using System;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public abstract class DbAliasedExpression : DbExpression
    {
        #region Instance Fields
        private DbTableAlias _alias;
        #endregion

        #region Constructors
        protected DbAliasedExpression(DbExpressionType nodeType, 
            Type type, DbTableAlias alias)
            : base(nodeType, type)
        {
            _alias = alias;
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
        #endregion
    }
}

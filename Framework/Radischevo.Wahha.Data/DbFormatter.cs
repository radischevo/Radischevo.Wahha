using System;
using Jeltofiol.Wahha.Data.Query.Expressions;

namespace Jeltofiol.Wahha.Data.Query
{
    public abstract class DbFormatter
    {
        #region Constructors
        protected DbFormatter() 
        { }
        #endregion

        #region Instance Methods
        public abstract string Format(SqlExpression node);
        #endregion
    }
}

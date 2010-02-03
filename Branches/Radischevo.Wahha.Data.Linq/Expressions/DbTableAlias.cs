using System;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public class DbTableAlias
    {
        #region Constructors
        public DbTableAlias()
        {
        }
        #endregion

        #region Instance Methods
        public override string ToString()
        {
            return ("A:" + GetHashCode());
        }
        #endregion
    }
}

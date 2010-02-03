using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Jeltofiol.Wahha.Core;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public class DbRowNumberExpression : DbExpression
    {
        #region Instance Fields
        private ReadOnlyCollection<DbOrderExpression> _orderBy;
        #endregion

        #region Constructors
        public DbRowNumberExpression(IEnumerable<DbOrderExpression> orderBy)
            : base(DbExpressionType.RowCount, typeof(int))
        {
            _orderBy = orderBy.AsReadOnly();
        }
        #endregion

        #region Instance Properties
        public ReadOnlyCollection<DbOrderExpression> OrderBy
        {
            get 
            { 
                return _orderBy; 
            }
        }
        #endregion
    }
}

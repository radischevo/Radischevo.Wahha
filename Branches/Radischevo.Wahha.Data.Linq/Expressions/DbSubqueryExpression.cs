using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public abstract class DbSubqueryExpression : DbExpression
    {
        #region Instance Fields
        private DbSelectExpression _select;
        #endregion

        #region Constructors
        protected DbSubqueryExpression(DbExpressionType expressionType, 
            Type type, DbSelectExpression select)
            : base(expressionType, type)
        {
            Debug.Assert(expressionType == DbExpressionType.Scalar || 
                expressionType == DbExpressionType.Exists || 
                expressionType == DbExpressionType.In);

            _select = select;
        }
        #endregion

        #region Instance Properties
        public DbSelectExpression Select
        {
            get 
            { 
                return _select; 
            }
        }
        #endregion
    }
}

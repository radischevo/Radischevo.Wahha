using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

using Jeltofiol.Wahha.Core;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public class DbExpressionSet : DbStatementExpression
    {
        #region Instance Fields
        private ReadOnlyCollection<Expression> _expressions;
        #endregion

        #region Constructors
        public DbExpressionSet(IList<Expression> expressions)
            : base(DbExpressionType.Block, 
                expressions[expressions.Count - 1].Type)
        {
            _expressions = expressions.AsReadOnly();
        }

        public DbExpressionSet(params Expression[] expressions)
            : this((IList<Expression>)expressions)
        {
        }
        #endregion

        #region Instance Properties
        public ReadOnlyCollection<Expression> Expressions
        {
            get 
            { 
                return _expressions; 
            }
        }
        #endregion
    }
}

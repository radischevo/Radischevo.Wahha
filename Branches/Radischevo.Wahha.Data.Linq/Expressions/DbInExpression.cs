using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

using Jeltofiol.Wahha.Core;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public class DbInExpression : DbSubqueryExpression
    {
        #region Instance Fields
        private Expression _expression;
        private ReadOnlyCollection<Expression> _values;
        #endregion

        #region Constructors
        public DbInExpression(Expression expression, DbSelectExpression select)
            : base(DbExpressionType.In, typeof(bool), select)
        {
            _expression = expression;
        }

        public DbInExpression(Expression expression, IEnumerable<Expression> values)
            : base(DbExpressionType.In, typeof(bool), null)
        {
            _expression = expression;
            _values = values.AsReadOnly();
        }
        #endregion

        #region Instance Properties
        public Expression Expression
        {
            get 
            { 
                return _expression; 
            }
        }

        public ReadOnlyCollection<Expression> Values
        {
            get 
            { 
                return _values; 
            }
        }
        #endregion
    }
}

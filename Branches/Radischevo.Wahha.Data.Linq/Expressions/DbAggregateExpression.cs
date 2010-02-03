using System;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public class DbAggregateExpression : DbExpression
    {
        #region Instance Fields
        private DbAggregateType _type;
        private Expression _argument;
        private bool _isDistinct;
        #endregion

        #region Constructors
        public DbAggregateExpression(Type type, DbAggregateType aggType, 
            Expression argument, bool isDistinct)
            : base(DbExpressionType.Aggregate, type)
        {
            _type = aggType;
            _argument = argument;
            _isDistinct = isDistinct;
        }
        #endregion

        #region Instance Properties
        public DbAggregateType AggregateType
        {
            get 
            { 
                return _type; 
            }
        }

        public Expression Argument
        {
            get 
            { 
                return _argument; 
            }
        }

        public bool IsDistinct
        {
            get 
            { 
                return _isDistinct; 
            }
        }
        #endregion
    }
}

using System;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public class DbAggregateSubqueryExpression : DbExpression
    {
        #region Instance Fields
        private DbTableAlias _groupByAlias;
        private Expression _aggregateInGroupSelect;
        private DbScalarExpression _aggregateAsSubquery;
        #endregion

        #region Constructors
        public DbAggregateSubqueryExpression(DbTableAlias groupByAlias, 
            Expression aggregateInGroupSelect, DbScalarExpression aggregateAsSubquery)
            : base(DbExpressionType.AggregateSubquery, aggregateAsSubquery.Type)
        {
            _aggregateInGroupSelect = aggregateInGroupSelect;
            _groupByAlias = groupByAlias;
            _aggregateAsSubquery = aggregateAsSubquery;
        }
        #endregion

        #region Instance Properties
        public DbTableAlias GroupByAlias 
        { 
            get 
            { 
                return _groupByAlias; 
            } 
        }

        public Expression AggregateInGroupSelect 
        { 
            get 
            { 
                return _aggregateInGroupSelect; 
            } 
        }

        public DbScalarExpression AggregateAsSubquery 
        { 
            get 
            { 
                return _aggregateAsSubquery; 
            } 
        }
        #endregion
    }
}

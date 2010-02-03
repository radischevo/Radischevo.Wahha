using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

using Jeltofiol.Wahha.Core;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public class DbClientJoinExpression : DbExpression
    {
        #region Instance Fields
        private DbProjectionExpression _projection;
        private ReadOnlyCollection<Expression> _outerKey;
        private ReadOnlyCollection<Expression> _innerKey;
        #endregion

        #region Constructors
        public DbClientJoinExpression(DbProjectionExpression projection, 
            IEnumerable<Expression> outerKey, IEnumerable<Expression> innerKey)
            : base(DbExpressionType.ClientJoin, projection.Type)
        {
            _outerKey = outerKey.AsReadOnly();
            _innerKey = innerKey.AsReadOnly();
            _projection = projection;
        }
        #endregion

        #region Instance Properties
        public ReadOnlyCollection<Expression> OuterKey
        {
            get 
            { 
                return _outerKey; 
            }
        }

        public ReadOnlyCollection<Expression> InnerKey
        {
            get 
            { 
                return _innerKey; 
            }
        }

        public DbProjectionExpression Projection
        {
            get 
            { 
                return _projection; 
            }
        }
        #endregion
    }
}

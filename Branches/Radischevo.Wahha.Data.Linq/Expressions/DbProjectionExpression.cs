using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    /// <summary>
    /// A custom expression representing the construction 
    /// of one or more result objects from a 
    /// SQL select expression
    /// </summary>
    public class DbProjectionExpression : DbExpression
    {
        #region Instance Fields
        private DbSelectExpression _select;
        private Expression _projector;
        private LambdaExpression _aggregator;
        #endregion

        #region Constructors
        public DbProjectionExpression(DbSelectExpression source, 
            Expression projector)
            : this(source, projector, null)
        {
        }

        public DbProjectionExpression(DbSelectExpression source, 
            Expression projector, LambdaExpression aggregator)
            : base(DbExpressionType.Projection, (aggregator != null) ? 
            aggregator.Body.Type : typeof(IEnumerable<>).MakeGenericType(projector.Type))
        {
            this._select = source;
            this._projector = projector;
            this._aggregator = aggregator;
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
        
        public Expression Projector
        {
            get 
            { 
                return _projector; 
            }
        }
        
        public LambdaExpression Aggregator
        {
            get 
            { 
                return _aggregator; 
            }
        }
        
        public bool IsSingleton
        {
            get 
            { 
                return (_aggregator != null && 
                    _aggregator.Body.Type == _projector.Type); 
            }
        }

        public override string ToString()
        {
            return "";
            //return DbExpressionWriter.WriteToString(this);
        }
        #endregion
    }
}

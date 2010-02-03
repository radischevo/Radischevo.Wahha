using System;
using System.Linq.Expressions;
using Jeltofiol.Wahha.Data.Linq.Mapping;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public class DbEntityExpression : DbExpression
    {
        #region Instance Fields
        private MetaType _entity;
        private Expression _expression;
        #endregion

        #region Constructors
        public DbEntityExpression(MetaType entity, Expression expression)
            : base(DbExpressionType.Entity, expression.Type)
        {
            _entity = entity;
            _expression = expression;
        }
        #endregion

        #region Instance Properties
        public MetaType Entity
        {
            get 
            { 
                return _entity; 
            }
        }

        public Expression Expression
        {
            get 
            { 
                return _expression; 
            }
        }
        #endregion
    }
}

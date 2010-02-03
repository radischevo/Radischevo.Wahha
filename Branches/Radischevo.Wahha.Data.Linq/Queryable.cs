using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Jeltofiol.Wahha.Core;

namespace Jeltofiol.Wahha.Data.Linq
{
    /// <summary>
    /// A default implementation of IQueryable for use with QueryProvider
    /// </summary>
    public class Queryable<T> : IQueryable<T>, IQueryable, 
        IEnumerable<T>, IEnumerable, IOrderedQueryable<T>, IOrderedQueryable
    {
        #region Instance Fields
        private IQueryProvider _provider;
        private Expression _expression;
        #endregion

        #region Constructors
        public Queryable(IQueryProvider provider)
        {
            Precondition.Require(provider, Error.ArgumentNull("provider"));

            _provider = provider;
            _expression = Expression.Constant(this);
        }

        public Queryable(QueryProvider provider, Expression expression)
        {
            Precondition.Require(provider, Error.ArgumentNull("provider"));
            Precondition.Require(expression, Error.ArgumentNull("expression"));
            Precondition.Require(typeof(IQueryable<T>).IsAssignableFrom(expression.Type), 
                Error.ExpressionTypeMustImplementIQueryable("expression"));
            
            _provider = provider;
            _expression = expression;
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

        public Type ElementType
        {
            get 
            { 
                return typeof(T); 
            }
        }

        public IQueryProvider Provider
        {
            get 
            { 
                return _provider; 
            }
        }
        #endregion

        #region Instance Methods
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_provider.Execute(_expression)).GetEnumerator();
        }

        public override string ToString()
        {
            ITextQuery query = (_provider as ITextQuery);
            if (query != null)
                return query.GetQueryText(_expression);
            
            return String.Empty;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_provider.Execute(_expression)).GetEnumerator();
        }
        #endregion
    }
}

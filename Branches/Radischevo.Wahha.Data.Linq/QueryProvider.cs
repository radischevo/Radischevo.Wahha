using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Jeltofiol.Wahha.Core;

namespace Jeltofiol.Wahha.Data.Linq
{
    /// <summary>
    /// A base abstract LINQ query provider.
    /// </summary>
    public abstract class QueryProvider : IQueryProvider, ITextQuery
    {
        #region Constructors
        protected QueryProvider()
        {
        }
        #endregion

        #region Instance Methods
        IQueryable<T> IQueryProvider.CreateQuery<T>(Expression expression)
        {
            return new Queryable<T>(this, expression);
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            Type elementType = expression.Type.GetSequenceElementType();
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(Queryable<>)
                    .MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        T IQueryProvider.Execute<T>(Expression expression)
        {
            return (T)Execute(expression);
        }

        object IQueryProvider.Execute(Expression expression)
        {
            return Execute(expression);
        }

        public abstract string GetQueryText(Expression expression);
        public abstract object Execute(Expression expression);
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Jeltofiol.Wahha.Core;

namespace Jeltofiol.Wahha.Data.Linq
{
    public static class Aggregator
    {
        #region Static Methods
        /// <summary>
        /// Get a function that coerces a sequence of one type into another type.
        /// This is primarily used for aggregators stored in ProjectionExpression's, 
        /// which are used to represent the final transformation of the entire result set of a query.
        /// </summary>
        public static LambdaExpression Aggregate(Type expectedType, Type actualType)
        {
            Type actualElementType = actualType.GetSequenceElementType();
            
            if (!expectedType.IsAssignableFrom(actualType))
            {
                Type expectedElementType = expectedType.GetSequenceElementType();
                ParameterExpression p = Expression.Parameter(actualType, "p");
                Expression body = null;
                
                if (expectedType.IsAssignableFrom(actualElementType))
                    body = Expression.Call(typeof(Enumerable), "SingleOrDefault", new Type[] { actualElementType }, p);                
                else if (expectedType.IsGenericType && expectedType.GetGenericTypeDefinition() == typeof(IQueryable<>))
                    body = Expression.Call(typeof(Queryable), "AsQueryable", new Type[] { expectedElementType }, 
                        CoerceElement(expectedElementType, p));
                else if (expectedType.IsArray && expectedType.GetArrayRank() == 1)
                    body = Expression.Call(typeof(Enumerable), "ToArray", new Type[] { expectedElementType }, 
                        CoerceElement(expectedElementType, p));
                else if (expectedType.IsAssignableFrom(typeof(List<>).MakeGenericType(actualElementType)))
                    body = Expression.Call(typeof(Enumerable), "ToList", new Type[] { expectedElementType }, 
                        CoerceElement(expectedElementType, p));
                else
                {
                    ConstructorInfo ci = expectedType.GetConstructor(new Type[] { actualType });
                    if (ci != null)
                        body = Expression.New(ci, p);
                }
                
                if (body != null)
                    return Expression.Lambda(body, p);
            }
            return null;
        }

        private static Expression CoerceElement(Type expectedElementType, Expression expression)
        {
            Type elementType = expression.Type.GetSequenceElementType();
            if (expectedElementType != elementType && (expectedElementType.IsAssignableFrom(elementType) || 
                elementType.IsAssignableFrom(expectedElementType)))
                return Expression.Call(typeof(Enumerable), "Cast", 
                    new Type[] { expectedElementType }, expression);
            
            return expression;
        }
        #endregion
    }
}

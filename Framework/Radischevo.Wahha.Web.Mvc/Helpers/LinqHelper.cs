using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Core.Expressions;
using Radischevo.Wahha.Web.Routing;

namespace Radischevo.Wahha.Web.Mvc
{
    internal static class LinqHelper
    {
        #region Static Methods
        private static object ExtractArgumentValue(Expression expression)
        {
            object value = null;
            ConstantExpression ce = (expression as ConstantExpression);
            if (ce == null)
            {
                Expression<Func<object>> le =
                    Expression.Lambda<Func<object>>(
                    Expression.Convert(expression, typeof(object)),
                    new ParameterExpression[0]);
                try
                {
                    value = le.Compile()();
                }
                catch
                {
                    value = null;
                }
            }
            else
                value = ce.Value;

            return value;
        }

        public static object[] ExtractArgumentsToArray(MethodCallExpression expression)
        {
            Precondition.Require(expression, Error.ArgumentNull("expression"));
            List<object> plist = new List<object>();

            foreach (Expression e in expression.Arguments)
                plist.Add(ExtractArgumentValue(e));
            
            return plist.ToArray();
        }

        public static ValueDictionary ExtractArgumentsToDictionary(MethodCallExpression expression)
        {
            Precondition.Require(expression, Error.ArgumentNull("expression"));
            ValueDictionary plist = new ValueDictionary();
            ParameterInfo[] parameters = expression.Method.GetParameters();

            if (parameters.Length > 0)
            {
                for (int i = 0; i < parameters.Length; i++)
                    plist.Add(parameters[i].Name, 
                        ExtractArgumentValue(expression.Arguments[i]));
            }
            return plist;
        }

        public static Func<TValue> WrapModelAccessor<TClass, TValue>(
            Expression<Func<TClass, TValue>> expression, TClass instance)
            where TClass : class
        {
            return () => {
                try
                {
                    return CachedExpressionCompiler.Compile(expression)(instance);
                }
                catch (NullReferenceException)
                {
                    return default(TValue);
                }
            };
        }
        #endregion
    }
}

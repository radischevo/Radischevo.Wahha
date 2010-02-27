using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

		public static string GetExpressionText(LambdaExpression expression)
		{
			Stack<string> nameParts = new Stack<string>();
			Expression part = expression.Body;

			while (part != null)
			{
				if (part.NodeType == ExpressionType.Call)
				{
					MethodCallExpression mce = (MethodCallExpression)part;
					if (!IsSingleArgumentIndexer(mce))
						break;

					nameParts.Push(GetIndexerInvocation(
						mce.Arguments.Single(),
						expression.Parameters.ToArray()));

					part = mce.Object;
				}
				else if (part.NodeType == ExpressionType.ArrayIndex)
				{
					BinaryExpression be = (BinaryExpression)part;

					nameParts.Push(GetIndexerInvocation(
						be.Right,
						expression.Parameters.ToArray()));

					part = be.Left;
				}
				else if (part.NodeType == ExpressionType.MemberAccess)
				{
					MemberExpression mep = (MemberExpression)part;
					nameParts.Push(mep.Member.Name);

					part = mep.Expression;
				}
				else
					break;
			}

			if (nameParts.Count > 0 && String.Equals(nameParts.Peek(),
				"model", StringComparison.OrdinalIgnoreCase))
				nameParts.Pop();

			if (nameParts.Count == 0)
				return String.Empty;

			return nameParts.Aggregate((left, right) =>
				left + "." + right).ToLowerInvariant();
		}

		private static string GetIndexerInvocation(Expression expression, 
			ParameterExpression[] parameters)
		{
			Expression converted = Expression.Convert(expression, typeof(object));
			ParameterExpression fakeParameter = Expression.Parameter(typeof(object), null);
			Expression<Func<object, object>> lambda = Expression.Lambda<Func<object, object>>(converted, fakeParameter);
			Func<object, object> func;

			try
			{
				func = CachedExpressionCompiler.Compile(lambda);
			}
			catch (InvalidOperationException)
			{
				throw Error.InvalidIndexerExpression(expression, parameters[0]);
			}
			return "[" + Convert.ToString(func(null), CultureInfo.InvariantCulture) + "]";
		}

		public static bool IsSingleArgumentIndexer(Expression expression)
		{
			MethodCallExpression me = (expression as MethodCallExpression);
			if (me == null || me.Arguments.Count != 1)
				return false;

			return me.Method.DeclaringType.GetDefaultMembers()
				.OfType<PropertyInfo>()
				.Any(p => p.GetGetMethod() == me.Method);
		}
        #endregion
    }
}

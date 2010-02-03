using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Radischevo.Wahha.Core
{
    public static class ObjectExtensions
    {
        #region Static Fields
        private static readonly char[] _expressionPartSeparator = new char[] { '.' };
        private static readonly char[] _indexExprStartChars = new char[] { '[', '(' };
        private static readonly char[] _indexExprEndChars = new char[] { ']', ')' };
        #endregion

        #region Extension Methods
        public static IEnumerable<T> ToEnumerable<T>(this T element)
        {
			yield return element;
        }

        public static Indexable<T> ToIndexable<T>(this T instance)
            where T : class
        {
            return new Indexable<T>(instance);
        }

        public static T Evaluate<T>(this object container, string expression)
        {
            return Converter.ChangeType<T>(Evaluate(container, expression));
        }

        public static string Evaluate(this object container, string expression, string format)
        {
            object value = Evaluate(container, expression);
            if (String.IsNullOrEmpty(format))
                return Convert.ToString(value, CultureInfo.InvariantCulture);

            return String.Format("{0:" + format + "}", value);
        }

        public static object Evaluate(this object container, string expression)
        {
            Precondition.Require(expression, Error.ArgumentNull("expression"));
            expression = expression.Trim();

            if (container == null)
                return null;

            if (expression.Length == 0)
                return container;

            string[] parts = expression.Split(_expressionPartSeparator);
            return Evaluate(container, parts);
        }

		public static TResult Evaluate<T, TResult>(this T container, 
			Func<T, TResult> expression)
			where T : class
		{
			return Evaluate(container, expression, default(TResult));
		}
		
		public static TResult Evaluate<T, TResult>(this T container,
			Func<T, TResult> expression, TResult defaultValue) 
			where T : class
		{
			Precondition.Require(expression, 
				Error.ArgumentNull("expression"));

			if (container == null)
				return defaultValue;

			return expression(container);
		}
        #endregion

        #region Helper Methods
        private static object Evaluate(object container, string[] expressionParts)
        {
            object obj = container;

            for (int i = 0; i < expressionParts.Length; ++i)
            {
                if (obj == null)
                    return obj;

                string part = expressionParts[i];
                if (part.IndexOfAny(_indexExprStartChars) > -1)
                    obj = GetIndexedPropertyValue(obj, part);
                else
                    obj = GetPropertyOrFieldValue(obj, part);
            }
            return obj;
        }

        private static object GetIndexedPropertyValue(object container, string expr)
        {
            Precondition.Require(container, Error.ArgumentNull("container"));
            Precondition.Require(!String.IsNullOrEmpty(expr), Error.BindingExpressionCannotBeEmpty("expr"));

            int start = expr.IndexOfAny(_indexExprStartChars);
            int end = expr.IndexOfAny(_indexExprEndChars, start + 1);

            if (start < 0 || end < 0 || end > start)
                throw Error.InvalidBindingExpressionFormat("expr");

            string indexPart = expr.Substring(start + 1, end - start - 1).Trim();
            string propertyPart = null;
            object indexValue = null;
            bool isIntIndex = false;
            object instance;

            if (start != 0)
                propertyPart = expr.Substring(0, start);

            if (indexPart.Length != 0)
            {
                if (indexPart[0] == '\"' && indexPart[indexPart.Length - 1] == '\"' ||
                    indexPart[0] == '\'' && indexPart[indexPart.Length - 1] == '\'')
                    indexValue = indexPart.Substring(1, indexPart.Length - 2);

                else if (!char.IsDigit(indexPart[0]))
                    indexValue = indexPart;
                else
                {
                    int index;
                    if (isIntIndex = int.TryParse(indexPart, NumberStyles.Integer,
                        CultureInfo.InvariantCulture, out index))
                        indexValue = index;
                    else
                        indexValue = indexPart;
                }
            }

            if (indexValue == null)
                throw Error.InvalidIndexerExpressionFormat("expr");

            if (!String.IsNullOrEmpty(propertyPart))
                instance = GetPropertyOrFieldValue(container, propertyPart);
            else
                instance = container;

            if (instance == null)
                return indexValue;

            Array array = (instance as Array);
            if (array != null && isIntIndex)
                return array.GetValue((int)indexValue);

            if (instance is IList && isIntIndex)
                return ((IList)instance)[(int)indexValue];

            PropertyInfo pi = instance.GetType().GetProperty("Item",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null, null, new Type[] { indexValue.GetType() }, null);

            if (pi != null)
                return pi.GetValue(instance, new object[] { indexValue });

            throw Error.IndexerNotFound(instance.GetType(), "expr");
        }

        private static object GetPropertyOrFieldValue(object container, string name)
        {
            Precondition.Require(container, Error.ArgumentNull("container"));
            Precondition.Require(!String.IsNullOrEmpty(name), Error.BindingExpressionCannotBeEmpty("name"));

            Type type = container.GetType();
            PropertyInfo pi = type.GetProperty(name,
                BindingFlags.Instance | BindingFlags.Public |
                BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            if (pi != null)
                return pi.CreateAccessor().GetValue(container);

            FieldInfo fi = type.GetField(name,
                BindingFlags.Instance | BindingFlags.Public |
                BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            if (fi != null)
                return fi.CreateAccessor().GetValue(container);

            throw Error.MissingMember(container.GetType(), name);
        }
        #endregion
    }
}

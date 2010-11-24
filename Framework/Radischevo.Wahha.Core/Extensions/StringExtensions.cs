using System;
using System.Collections.Generic;
using System.Linq;

namespace Radischevo.Wahha.Core
{
    public static class StringExtensions
    {
        #region Static Extension Methods
		public static string Define(this string str)
		{
			return str ?? String.Empty;
		}

		public static string Define(this string str, string defaultValue)
		{
			return (str == null) ? defaultValue : str;
		}

		public static string Define(this string str, 
			Predicate<string> isDefined, string defaultValue)
		{
			if (isDefined == null)
				return Define(str, defaultValue);

			return (isDefined(str)) ? str : defaultValue;
		}

        public static bool Contains(this string str, 
            string value, StringComparison comparison)
        {
            return (str.IndexOf(value, comparison) > -1);
        }

        public static bool ContainsAny(this string str, params char[] chars)
        {
            return chars.Any(c => str.Contains(c));
        }

        public static bool ContainsAny(this string str, 
            IEqualityComparer<char> comparer, params char[] chars)
        {
            return chars.Any(c => str.Contains(c, comparer));
        }

        public static string Bind(this string format, object source)
        {
			Precondition.Require(format, () => Error.ArgumentNull("format"));

            string[] parts = SplitFormat(format).Select(p => p.Eval(source)).ToArray();
            return String.Join("", parts);
        }

        public static string Format(this string format, object arg)
        {
            return String.Format(format, arg);
        }

        public static string Format(this string format, object arg0, object arg1)
        {
            return String.Format(format, arg0, arg1);
        }

        public static string Format(this string format, object arg0, object arg1, object arg2)
        {
            return String.Format(format, arg0, arg1, arg2);
        }

        public static string Format(this string format, params object[] arguments)
        {
            return String.Format(format, arguments);
        }

        public static string Format(this string format, IFormatProvider provider, 
            params object[] arguments)
        {
            return String.Format(provider, format, arguments);
        }

        /// <summary>
        /// Returns the first few characters of the string with a length
        /// specified by the given parameter. If the string's length is less than the 
        /// given length the complete string is returned.
        /// </summary>
        /// <param name="str">The string to process</param>
        /// <param name="count">A number of characters to return</param>
        public static string Left(this string str, int count)
        {
            Precondition.Require(count > -1,
				() => Error.ParameterMustBeGreaterThanOrEqual("count", 0, count));

            if (str.Length > count)
                return str.Substring(0, count);
            
            return str;
        }

        /// <summary>
        /// Returns the last few characters of the string with a length
        /// specified by the given parameter. If the string's length is less than the 
        /// given length the complete string is returned. 
        /// </summary>
        /// <param name="str">The string to process</param>
        /// <param name="count">A number of characters to return</param>
        public static string Right(this string str, int count)
        {
            Precondition.Require(count > -1,
				() => Error.ParameterMustBeGreaterThanOrEqual("count", 0, count));
            
            if (str.Length > count)
                return str.Substring(str.Length - count, count);

            return str;
        }
        #endregion

        #region Helper Methods
        private static IEnumerable<ITextExpression> SplitFormat(string format)
        {
            int exprEndIndex = -1;
            int expStartIndex;

            do
            {
                expStartIndex = GetStartIndex(format, exprEndIndex + 1);
                if (expStartIndex < 0)
                {
                    if (exprEndIndex + 1 < format.Length)
                        yield return new LiteralExpression(
                            format.Substring(exprEndIndex + 1));
                    
                    break;
                }

                if (expStartIndex - exprEndIndex - 1 > 0)
                    yield return new LiteralExpression(
                        format.Substring(exprEndIndex + 1, 
                        expStartIndex - exprEndIndex - 1));

                int endBraceIndex = GetEndIndex(format, expStartIndex + 1);
                if (endBraceIndex < 0)
                {
                    yield return new FormatExpression(format.Substring(expStartIndex));
                }
                else
                {
                    exprEndIndex = endBraceIndex;
                    yield return new FormatExpression(format.Substring(expStartIndex, 
                        endBraceIndex - expStartIndex + 1));
                }
            } 
            while (expStartIndex > -1);
        }

        private static int GetStartIndex(string format, int startIndex)
        {
            int index = format.IndexOf('{', startIndex);
            if (index == -1)
                return index;

            if (index + 1 < format.Length)
            {
                char nextChar = format[index + 1];
                if (nextChar == '{')
                    return GetStartIndex(format, index + 2);
            }
            return index;
        }

        private static int GetEndIndex(string format, int startIndex)
        {
            int endBraceIndex = format.IndexOf('}', startIndex);
            if (endBraceIndex == -1)
                return endBraceIndex;
            
            int braceCount = 0;
            for (int i = endBraceIndex + 1; i < format.Length; i++)
            {
                if (format[i] == '}')
                    braceCount++;
                else
                    break;
            }
            if (braceCount % 2 == 1)
                return GetEndIndex(format, endBraceIndex + braceCount + 1);

            return endBraceIndex;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Radischevo.Wahha.Data
{
	internal static class ExpressionParameterExtractor
	{
		#region Static Methods
		public static object[] ExtractParameters(IList<Expression> arguments)
		{
			int length = arguments.Count;
			object[] values = new object[length];
			if (length > 0)
			{
				for (int i = 0; i < length; i++)
					values[i] = ExtractParameter(arguments[i]);
			}
			return values;
		}

		private static object ExtractParameter(Expression expression)
		{
			ConstantExpression ce = (expression as ConstantExpression);
			if (ce == null)
			{
				Expression<Func<object>> le = Expression.Lambda<Func<object>>(
					Expression.Convert(expression, typeof(object)),
					new ParameterExpression[0]);

				try
				{
					return le.Compile()();
				}
				catch
				{
					return null;
				}
			}
			return null;
		}
		#endregion
	}
}

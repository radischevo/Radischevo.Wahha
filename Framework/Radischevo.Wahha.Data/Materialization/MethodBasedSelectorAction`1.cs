using System;
using System.Linq.Expressions;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	internal abstract class MethodBasedSelectorAction<TAssociation> : LinkSelectorAction<TAssociation>
		where TAssociation : class
	{
		#region Instance Fields
		private MethodCallExpression _selector;
		#endregion

		#region Constructors
		protected MethodBasedSelectorAction(LambdaExpression expression)
			: base()
		{
			_selector = ConvertExpression(expression);
		}
		#endregion

		#region Instance Properties
		public MethodCallExpression Selector
		{
			get
			{
				return _selector;
			}
		}
		#endregion

		#region Static Methods
		private static MethodCallExpression ConvertExpression(LambdaExpression expression)
		{
			MethodCallExpression method = (expression.Body as MethodCallExpression);
			Precondition.Require(method, () => Error.SelectorMustBeAMethodCall("selector"));
			Precondition.Require(method.Object == expression.Parameters[0],
				() => Error.MethodCallMustTargetLambdaArgument("selector"));

			return method;
		}

		protected static object[] ExtractMethodParameters(MethodCallExpression method)
		{
			int length = method.Arguments.Count;
			object[] values = new object[length];
			if (length > 0)
			{
				for (int i = 0; i < length; i++)
					values[i] = ExtractMethodParameter(method.Arguments[i]);
			}
			return values;
		}

		private static object ExtractMethodParameter(Expression expression)
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
		#endregion
	}
}

using System;
using System.Linq.Expressions;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	internal abstract class ConstructorBasedSelectorAction<TAssociation> : LinkSelectorAction<TAssociation>
		where TAssociation : class
	{
		#region Instance Fields
		private NewExpression _factory;
		#endregion

		#region Constructors
		protected ConstructorBasedSelectorAction(LambdaExpression expression)
			: base()
		{
			_factory = ConvertExpression(expression);
		}
		#endregion

		#region Instance Properties
		public NewExpression Factory
		{
			get
			{
				return _factory;
			}
		}
		#endregion

		#region Static Methods
		private static NewExpression ConvertExpression(LambdaExpression expression)
		{
			NewExpression method = (expression.Body as NewExpression);
			Precondition.Require(method, () => Error.SelectorMustBeAMethodCall("selector"));

			if (!typeof(IDbOperation<TAssociation>).IsAssignableFrom(method.Constructor.DeclaringType))
				throw new ArgumentException("expression");

			return method;
		}

		protected static object[] ExtractMethodParameters(NewExpression constructor)
		{
			int length = constructor.Arguments.Count;
			object[] values = new object[length];
			if (length > 0)
			{
				for (int i = 0; i < length; i++)
					values[i] = ExtractMethodParameter(constructor.Arguments[i]);
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

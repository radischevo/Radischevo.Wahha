using System;
using System.Linq.Expressions;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	internal abstract class ConstructorBasedSelectorAction<TAssociation> : LinkSelectorAction<TAssociation>
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
			Precondition.Require(method, () => Error.ExpressionMustBeAConstructorCall("selector"));

			if (!typeof(IDbOperation<TAssociation>).IsAssignableFrom(method.Constructor.DeclaringType))
				throw Error.InvalidConstructorExpressionType("expression", 
					method.Constructor.DeclaringType, typeof(IDbOperation<TAssociation>));

			return method;
		}
		#endregion
	}
}

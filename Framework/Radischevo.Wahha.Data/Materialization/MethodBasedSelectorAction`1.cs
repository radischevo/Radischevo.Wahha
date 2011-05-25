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
		#endregion
	}
}

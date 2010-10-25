using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public class CollectionSelectorAction<TAssociation, TRepository>
		: SelectorAction<IEnumerable<TAssociation>>
		where TAssociation : class
		where TRepository : IRepository<TAssociation>
	{
		#region Constructors
		public CollectionSelectorAction(Expression<Func<TRepository, 
			IEnumerable<TAssociation>>> selector)
			: base(ConvertExpression(selector))
		{
		}
		#endregion

		#region Static Methods
		private static MethodCallExpression ConvertExpression(
			Expression<Func<TRepository, IEnumerable<TAssociation>>> expression)
		{
			MethodCallExpression method = (expression.Body as MethodCallExpression);
			Precondition.Require(method, () => Error.SelectorMustBeAMethodCall("selector"));
			Precondition.Require(method.Object == expression.Parameters[0],
				() => Error.MethodCallMustTargetLambdaArgument("selector"));

			return method;
		}
		#endregion

		#region Instance Methods
		protected override Func<IEnumerable<TAssociation>> CreateSelector()
		{
			return CreateSelector(typeof(TRepository));
		}
		#endregion
	}
}

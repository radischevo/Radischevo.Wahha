using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Radischevo.Wahha.Data
{
	internal sealed class OperationFactoryBasedSingleSelectorAction<TAssociation, TOperationFactory>
		: MethodBasedSelectorAction<TAssociation>
		where TAssociation : class
	{
		#region Constructors
		public OperationFactoryBasedSingleSelectorAction(
			Expression<Func<TOperationFactory, IDbOperation<TAssociation>>> selector)
			: base(selector)
		{
		}
		#endregion

		#region Instance Methods
		protected override IAssociationLoader<TAssociation> CreateSelector()
		{
			return new OperationFactoryBasedAssociationLoader<TAssociation>(typeof(TOperationFactory), 
				Selector.Method, ExpressionParameterExtractor.ExtractParameters(Selector.Arguments));
		}
		#endregion
	}

	internal sealed class OperationFactoryBasedCollectionSelectorAction<TAssociation, TOperationFactory>
		: MethodBasedSelectorAction<IEnumerable<TAssociation>>
		where TAssociation : class
	{
		#region Constructors
		public OperationFactoryBasedCollectionSelectorAction(Expression<Func<TOperationFactory,
			IDbOperation<IEnumerable<TAssociation>>>> selector)
			: base(selector)
		{
		}
		#endregion

		#region Instance Methods
		protected override IAssociationLoader<IEnumerable<TAssociation>> CreateSelector()
		{
			return new OperationFactoryBasedAssociationLoader<IEnumerable<TAssociation>>(typeof(TOperationFactory), 
				Selector.Method, ExpressionParameterExtractor.ExtractParameters(Selector.Arguments));
		}
		#endregion
	}
}

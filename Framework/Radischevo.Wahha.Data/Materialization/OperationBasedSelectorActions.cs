using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Radischevo.Wahha.Data
{
	internal sealed class OperationBasedSingleSelectorAction<TAssociation, TOperation>
		: ConstructorBasedSelectorAction<TAssociation>
		where TAssociation : class
		where TOperation : IDbOperation<TAssociation>
	{
		#region Constructors
		public OperationBasedSingleSelectorAction(Expression<Func<TOperation>> operation)
			: base(operation)
		{
		}
		#endregion

		#region Instance Methods
		protected override IAssociationLoader<TAssociation> CreateSelector()
		{
			return new OperationBasedAssociationLoader<TAssociation>(
				Factory.Constructor, ExtractMethodParameters(Factory));
		}
		#endregion
	}

	internal sealed class OperationBasedCollectionSelectorAction<TAssociation, TOperation>
		: ConstructorBasedSelectorAction<IEnumerable<TAssociation>>
		where TAssociation : class
		where TOperation : IDbOperation<IEnumerable<TAssociation>>
	{
		#region Constructors
		public OperationBasedCollectionSelectorAction(Expression<Func<TOperation>> operation)
			: base(operation)
		{
		}
		#endregion

		#region Instance Methods
		protected override IAssociationLoader<IEnumerable<TAssociation>> CreateSelector()
		{
			return new OperationBasedAssociationLoader<IEnumerable<TAssociation>>(
				Factory.Constructor, ExtractMethodParameters(Factory));
		}
		#endregion
	}
}

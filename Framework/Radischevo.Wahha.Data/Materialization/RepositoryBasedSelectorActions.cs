using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Radischevo.Wahha.Data
{
	internal sealed class RepositoryBasedSingleSelectorAction<TAssociation, TRepository>
		: MethodBasedSelectorAction<TAssociation>
		where TAssociation : class
		where TRepository : IRepository<TAssociation>
	{
		#region Constructors
		public RepositoryBasedSingleSelectorAction(Expression<Func<TRepository, TAssociation>> selector)
			: base(selector)
		{
		}
		#endregion

		#region Instance Methods
		protected override IAssociationLoader<TAssociation> CreateSelector()
		{
			return new RepositoryBasedAssociationLoader<TAssociation>(typeof(TRepository), 
				Selector.Method, ExpressionParameterExtractor.ExtractParameters(Selector.Arguments));
		}
		#endregion
	}

	internal sealed class RepositoryBasedCollectionSelectorAction<TAssociation, TRepository>
		: MethodBasedSelectorAction<IEnumerable<TAssociation>>
		where TAssociation : class
		where TRepository : IRepository<TAssociation>
	{
		#region Constructors
		public RepositoryBasedCollectionSelectorAction(Expression<Func<TRepository,
			IEnumerable<TAssociation>>> selector)
			: base(selector)
		{
		}
		#endregion

		#region Instance Methods
		protected override IAssociationLoader<IEnumerable<TAssociation>> CreateSelector()
		{
			return new RepositoryBasedAssociationLoader<IEnumerable<TAssociation>>(typeof(TRepository), 
				Selector.Method, ExpressionParameterExtractor.ExtractParameters(Selector.Arguments));
		}
		#endregion
	}
}

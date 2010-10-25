using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public abstract class ObjectMaterializer<TEntity> : IDbMaterializer<TEntity>
	{
		#region Constructors
		protected ObjectMaterializer()
		{
		}
		#endregion

		#region Instance Methods
		public TEntity Materialize(IValueSet source)
		{
			return Materialize(CreateInstance(source), source);
		}

		public virtual TEntity Materialize(TEntity entity, IValueSet source)
		{
			Precondition.Require(entity, () => Error.ArgumentNull("entity"));
			Precondition.Require(source, () => Error.ArgumentNull("source"));

			return Execute(entity, source);
		}

		protected abstract TEntity CreateInstance(IValueSet source);

		protected abstract TEntity Execute(TEntity entity, IValueSet source);

		protected ISingleAssociationSelectorBuilder<TAssociation> 
			Associate<TAssociation>(Link<TAssociation> link)
			where TAssociation : class
		{
			return new SingleAssociationBuilder<TAssociation>(
				new SingleAssociator<TAssociation>(link));
		}

		protected ICollectionAssociationSelectorBuilder<TAssociation> 
			Associate<TAssociation>(EnumerableLink<TAssociation> link)
			where TAssociation : class
		{
			return new CollectionAssociationBuilder<TAssociation>(
				new CollectionAssociator<TAssociation>(link));
		}
		#endregion
	}
}

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
		public TEntity Materialize(IDbValueSet source)
		{
			return Materialize(CreateInstance(source), source);
		}

		public virtual TEntity Materialize(TEntity entity, IDbValueSet source)
		{
			Precondition.Require(entity, () => Error.ArgumentNull("entity"));
			Precondition.Require(source, () => Error.ArgumentNull("source"));

			return Execute(entity, source);
		}

		protected abstract TEntity CreateInstance(IDbValueSet source);

		protected abstract TEntity Execute(TEntity entity, IDbValueSet source);

		protected ISingleAssociationSelectorBuilder<TAssociation> 
			Associate<TAssociation>(Link<TAssociation> link)
			where TAssociation : class
		{
			return new SingleAssociationBuilder<TAssociation>(link);
		}

		protected ICollectionAssociationSelectorBuilder<TAssociation> 
			Associate<TAssociation>(EnumerableLink<TAssociation> link)
			where TAssociation : class
		{
			return new CollectionAssociationBuilder<TAssociation>(link);
		}

		protected IEntityAssociationBuilder<TAssociation>
			Associate<TAssociation>(TAssociation association)
			where TAssociation : class
		{
			return new EntityAssociationBuilder<TAssociation>(association);
		}
		#endregion
	}
}

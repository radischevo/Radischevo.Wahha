using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public interface ISingleAssociationSelectorBuilder<TAssociation>
		: IHideObjectMembers
		where TAssociation : class
	{
		ISingleAssociationBuilder<TAssociation> With<TRepository>(
			Expression<Func<TRepository, TAssociation>> selector)
			where TRepository : IRepository<TAssociation>;
	}

	public interface ISingleAssociationBuilder<TAssociation>
		: IHideObjectMembers
		where TAssociation : class
	{
		ISingleAssociationBuilder<TAssociation> Subset(IValueSetTransformer transformer);

		ISingleAssociationBuilder<TAssociation> Validate(IValueSetValidator validator);

		Link<TAssociation> Apply();

		Link<TAssociation> Apply(IValueSet source);

		Link<TAssociation> Apply<TMaterializer>(IValueSet source)
			where TMaterializer : IDbMaterializer<TAssociation>;
	}

	public interface ICollectionAssociationSelectorBuilder<TAssociation>
		: IHideObjectMembers
		where TAssociation : class
	{
		ICollectionAssociationBuilder<TAssociation> With<TRepository>(
			Expression<Func<TRepository, IEnumerable<TAssociation>>> selector)
			where TRepository : IRepository<TAssociation>;
	}

	public interface ICollectionAssociationBuilder<TAssociation>
		: IHideObjectMembers
		where TAssociation : class
	{
		EnumerableLink<TAssociation> Apply();
	}

	public interface IEntityAssociationBuilder<TAssociation>
		: IHideObjectMembers
		where TAssociation : class
	{
		IEntityAssociationBuilder<TAssociation> Subset(IValueSetTransformer transformer);

		IEntityAssociationBuilder<TAssociation> Validate(IValueSetValidator validator);

		TAssociation Apply(IValueSet source);

		TAssociation Apply<TMaterializer>(IValueSet source)
			where TMaterializer : IDbMaterializer<TAssociation>;
	}

	internal sealed class SingleAssociationBuilder<TAssociation> :
		ISingleAssociationBuilder<TAssociation>,
		ISingleAssociationSelectorBuilder<TAssociation>
		where TAssociation : class
	{
		#region Instance Fields
		private SingleLinkAssociator<TAssociation> _associator;
		private IValueSetTransformer _transformer;
		private IValueSetValidator _validator;
		#endregion

		#region Constructors
		internal SingleAssociationBuilder(
			SingleLinkAssociator<TAssociation> associator)
		{
			_associator = associator;
		}
		#endregion

		#region Instance Methods
		public ISingleAssociationBuilder<TAssociation> Subset(IValueSetTransformer transformer)
		{
			_transformer = transformer;
			return this;
		}

		public ISingleAssociationBuilder<TAssociation> Validate(IValueSetValidator validator)
		{
			_validator = validator;
			return this;
		}

		public ISingleAssociationBuilder<TAssociation> With<TRepository>(
			Expression<Func<TRepository, TAssociation>> selector)
			where TRepository : IRepository<TAssociation>
		{
			LinkAssociatorAction<TAssociation> action = 
				new SelectorAction<TAssociation, TRepository>(selector);
			action.Order = 1;
			_associator.Actions.Add(action);

			return this;
		}

		private Link<TAssociation> Apply(LinkMaterializerAction<TAssociation> action)
		{
			action.Transformer = _transformer;
			action.Validator = _validator;
			action.Order = 2;

			_associator.Actions.Add(action);
			return Apply();
		}

		public Link<TAssociation> Apply()
		{
			_associator.Execute();
			return _associator.Link;
		}

		public Link<TAssociation> Apply(IValueSet source)
		{
			LinkMaterializerAction<TAssociation> action = 
				new LinkMaterializerAction<TAssociation>(source);

			return Apply(action);
		}

		public Link<TAssociation> Apply<TMaterializer>(IValueSet source) 
			where TMaterializer : IDbMaterializer<TAssociation>
		{
			LinkMaterializerAction<TAssociation> action =
				new LinkMaterializerAction<TAssociation, TMaterializer>(source);
			
			return Apply(action);
		}
		#endregion
	}
	
	internal sealed class CollectionAssociationBuilder<TAssociation> :
		ICollectionAssociationSelectorBuilder<TAssociation>,
		ICollectionAssociationBuilder<TAssociation>
		where TAssociation : class
	{
		#region Instance Fields
		private CollectionLinkAssociator<TAssociation> _associator;
		#endregion

		#region Constructors
		internal CollectionAssociationBuilder(
			CollectionLinkAssociator<TAssociation> associator)
		{
			_associator = associator;
		}
		#endregion

		#region Instance Methods
		public ICollectionAssociationBuilder<TAssociation> With<TRepository>(
			Expression<Func<TRepository, IEnumerable<TAssociation>>> selector) 
			where TRepository : IRepository<TAssociation>
		{
			LinkAssociatorAction<IEnumerable<TAssociation>> action =
				new CollectionLinkSelectorAction<TAssociation, TRepository>(selector);
			action.Order = 1;
			_associator.Actions.Add(action);

			return this;
		}

		public EnumerableLink<TAssociation> Apply()
		{
			_associator.Execute();
			return _associator.Link;
		}
		#endregion
	}

	internal sealed class EntityAssociationBuilder<TAssociation> :
		IEntityAssociationBuilder<TAssociation>
		where TAssociation : class
	{
		#region Instance Fields
		private EntityAssociator<TAssociation> _associator;
		private IValueSetTransformer _transformer;
		private IValueSetValidator _validator;
		#endregion

		#region Constructors
		internal EntityAssociationBuilder(EntityAssociator<TAssociation> associator)
		{
			_associator = associator;
		}
		#endregion

		#region Instance Methods
		public IEntityAssociationBuilder<TAssociation> Subset(IValueSetTransformer transformer)
		{
			_transformer = transformer;
			return this;
		}

		public IEntityAssociationBuilder<TAssociation> Validate(IValueSetValidator validator)
		{
			_validator = validator;
			return this;
		}

		private TAssociation Apply(EntityMaterializerAction<TAssociation> action)
		{
			action.Transformer = _transformer;
			action.Validator = _validator;
			action.Order = 1;

			_associator.Actions.Add(action);
			_associator.Execute();

			return _associator.Entity;
		}

		public TAssociation Apply(IValueSet source)
		{
			EntityMaterializerAction<TAssociation> action =
				new EntityMaterializerAction<TAssociation>(source);

			return Apply(action);
		}

		public TAssociation Apply<TMaterializer>(IValueSet source)
			where TMaterializer : IDbMaterializer<TAssociation>
		{
			EntityMaterializerAction<TAssociation> action =
				new EntityMaterializerAction<TAssociation, TMaterializer>(source);

			return Apply(action);
		}
		#endregion
	}
}
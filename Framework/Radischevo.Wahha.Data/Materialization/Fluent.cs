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
		ISingleAssociationBuilder<TAssociation> Subset(string prefix);

		ISingleAssociationBuilder<TAssociation> Scheme(params string[] keys);

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

	internal sealed class SingleAssociationBuilder<TAssociation> :
		ISingleAssociationBuilder<TAssociation>,
		ISingleAssociationSelectorBuilder<TAssociation>
		where TAssociation : class
	{
		#region Instance Fields
		private SingleAssociator<TAssociation> _associator;
		private SubsetMapper _mapper;
		private SubsetSchemeValidator _validator;
		#endregion

		#region Constructors
		internal SingleAssociationBuilder(
			SingleAssociator<TAssociation> associator)
		{
			_associator = associator;
		}
		#endregion

		#region Instance Methods
		public ISingleAssociationBuilder<TAssociation> Subset(string prefix)
		{
			_mapper = new SubsetMapper(prefix);
			return this;
		}

		public ISingleAssociationBuilder<TAssociation> Scheme(params string[] keys)
		{
			_validator = new SubsetSchemeValidator(keys);
			return this;
		}

		public ISingleAssociationBuilder<TAssociation> With<TRepository>(
			Expression<Func<TRepository, TAssociation>> selector)
			where TRepository : IRepository<TAssociation>
		{
			AssociatorAction<TAssociation> action = 
				new SelectorAction<TAssociation, TRepository>(selector);
			action.Order = 1;
			_associator.Actions.Add(action);

			return this;
		}

		public Link<TAssociation> Apply()
		{
			_associator.Execute();
			return _associator.Link;
		}

		public Link<TAssociation> Apply(IValueSet source)
		{
			MaterializerAction<TAssociation> action = 
				new MaterializerAction<TAssociation>(source);

			action.Transformer = _mapper;
			action.Validator = _validator;
			action.Order = 2;

			_associator.Actions.Add(action);

			return Apply();
		}

		public Link<TAssociation> Apply<TMaterializer>(IValueSet source) 
			where TMaterializer : IDbMaterializer<TAssociation>
		{
			MaterializerAction<TAssociation> action =
				new MaterializerAction<TAssociation, TMaterializer>(source);
			
			action.Order = 2;
			action.Transformer = _mapper;
			action.Validator = _validator;
			
			_associator.Actions.Add(action);

			return Apply();
		}
		#endregion
	}

	internal sealed class CollectionAssociationBuilder<TAssociation> :
		ICollectionAssociationSelectorBuilder<TAssociation>,
		ICollectionAssociationBuilder<TAssociation>
		where TAssociation : class
	{
		#region Instance Fields
		private CollectionAssociator<TAssociation> _associator;
		#endregion

		#region Constructors
		internal CollectionAssociationBuilder(
			CollectionAssociator<TAssociation> associator)
		{
			_associator = associator;
		}
		#endregion

		#region Instance Methods
		public ICollectionAssociationBuilder<TAssociation> With<TRepository>(
			Expression<Func<TRepository, IEnumerable<TAssociation>>> selector) 
			where TRepository : IRepository<TAssociation>
		{
			AssociatorAction<IEnumerable<TAssociation>> action =
				new CollectionSelectorAction<TAssociation, TRepository>(selector);
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
}

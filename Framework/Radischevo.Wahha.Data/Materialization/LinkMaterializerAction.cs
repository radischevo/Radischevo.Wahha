using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Core.Expressions;

namespace Radischevo.Wahha.Data
{
	public class LinkMaterializerAction<TAssociation> 
		: LinkAssociatorAction<TAssociation>
		where TAssociation : class
	{
		#region Instance Fields
		private IValueSet _source;
		private List<IValueSetTransformer> _transformers;
		private List<IValueSetValidator> _validators;
		#endregion

		#region Constructors
		public LinkMaterializerAction(IValueSet source)
			: base()
		{
			Precondition.Require(source, () => Error.ArgumentNull("source"));
			_source = source;
			_transformers = new List<IValueSetTransformer>();
			_validators = new List<IValueSetValidator>();

			_validators.Add(new NullValueSetValidator());
		}
		#endregion

		#region Instance Properties
		public ICollection<IValueSetTransformer> Transformers
		{
			get
			{
				return _transformers;
			}
		}

		public ICollection<IValueSetValidator> Validators
		{
			get
			{
				return _validators;
			}
		}

		public IValueSet Source
		{
			get
			{
				return _source;
			}
		}
		#endregion

		#region Static Methods
		private static Link<TAssociation> ConvertLink(ILink<TAssociation> link)
		{
			Link<TAssociation> result = (link as Link<TAssociation>);
			Precondition.Require(result, () => Error.CouldNotMaterializeCollectionLink("link"));

			return result;
		}
		#endregion

		#region Instance Methods
		protected virtual LinkValueInitializer<TAssociation> CreateInitializer()
		{
			Type type = typeof(IDbMaterializer<>).MakeGenericType(typeof(TAssociation));
			return new LinkValueInitializer<TAssociation>(type);
		}

		protected virtual bool Validate(IValueSet values)
		{
			foreach (IValueSetValidator validator in Validators)
				if (!validator.Valid(values))
					return false;

			return true;
		}

		public override ILink<TAssociation> Execute(ILink<TAssociation> link)
		{
			Link<TAssociation> association = ConvertLink(link);
			LinkValueInitializer<TAssociation> initializer = CreateInitializer();
			IValueSet subset = Source;

			foreach (IValueSetTransformer transformer in Transformers)
				subset = transformer.Transform(subset);

			if (Validate(subset) && initializer != null)
				initializer.Initialize(association, subset);

			return association;
		}
		#endregion
	}

	public class LinkMaterializerAction<TAssociation, TMaterializer> 
		: LinkMaterializerAction<TAssociation>
		where TAssociation : class
		where TMaterializer : IDbMaterializer<TAssociation>
	{
		#region Constructors
		public LinkMaterializerAction(IValueSet source)
			: base(source)
		{
		}
		#endregion

		#region Instance Methods
		protected override LinkValueInitializer<TAssociation> CreateInitializer()
		{
			return new LinkValueInitializer<TAssociation>(typeof(TMaterializer));
		}
		#endregion
	}
}

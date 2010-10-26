using System;

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
		private IValueSetTransformer _transformer;
		private IValueSetValidator _validator;
		#endregion

		#region Constructors
		public LinkMaterializerAction(IValueSet source)
			: base()
		{
			Precondition.Require(source, () => Error.ArgumentNull("source"));
			_source = source;
		}
		#endregion

		#region Instance Properties
		public IValueSetTransformer Transformer
		{
			get
			{
				if (_transformer == null)
					_transformer = new NullValueSetTransformer();

				return _transformer;
			}
			set
			{
				_transformer = value;
			}
		}

		public IValueSetValidator Validator
		{
			get
			{
				if (_validator == null)
					_validator = new NullValueSetValidator();

				return _validator;
			}
			set
			{
				_validator = value;
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

		public override void Execute(ILink<TAssociation> link)
		{
			Link<TAssociation> association = ConvertLink(link);
			LinkValueInitializer<TAssociation> initializer = CreateInitializer();
			IValueSet subset = Transformer.Transform(Source);

			if (Validator.Valid(subset) && initializer != null)
				initializer.Initialize(association, subset);
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

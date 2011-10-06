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
		private IDbValueSet _source;
		private List<IDbValueSetTransformer> _transformers;
		private List<IDbValueSetValidator> _validators;
		#endregion

		#region Constructors
		public LinkMaterializerAction(IDbValueSet source)
			: base()
		{
			Precondition.Require(source, () => Error.ArgumentNull("source"));
			_source = source;
			_transformers = new List<IDbValueSetTransformer>();
			_validators = new List<IDbValueSetValidator>();

			_validators.Add(new NullValueSetValidator());
		}
		#endregion

		#region Instance Properties
		public ICollection<IDbValueSetTransformer> Transformers
		{
			get
			{
				return _transformers;
			}
		}

		public ICollection<IDbValueSetValidator> Validators
		{
			get
			{
				return _validators;
			}
		}

		public IDbValueSet Source
		{
			get
			{
				return _source;
			}
		}
		#endregion

		#region Instance Methods
		protected virtual LinkValueInitializer<TAssociation> CreateInitializer()
		{
			Type type = typeof(IDbMaterializer<>).MakeGenericType(typeof(TAssociation));
			return new LinkValueInitializer<TAssociation>(type);
		}

		protected virtual bool Validate(IDbValueSet values)
		{
			foreach (IDbValueSetValidator validator in Validators)
				if (!validator.Valid(values))
					return false;

			return true;
		}

		public override ILink<TAssociation> Execute(ILink<TAssociation> link)
		{
			LinkValueInitializer<TAssociation> initializer = CreateInitializer();
			IDbValueSet subset = Source;

			foreach (IDbValueSetTransformer transformer in Transformers)
				subset = transformer.Transform(subset);

			if (Validate(subset) && initializer != null)
				initializer.Initialize(link, subset);

			return link;
		}
		#endregion
	}

	public class LinkMaterializerAction<TAssociation, TMaterializer> 
		: LinkMaterializerAction<TAssociation>
		where TAssociation : class
		where TMaterializer : IDbMaterializer<TAssociation>
	{
		#region Constructors
		public LinkMaterializerAction(IDbValueSet source)
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

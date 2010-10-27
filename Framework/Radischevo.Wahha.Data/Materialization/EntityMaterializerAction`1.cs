using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public class EntityMaterializerAction<TAssociation>
		: EntityAssociatorAction<TAssociation>
		where TAssociation : class
	{
		#region Instance Fields
		private IValueSet _source;
		private List<IValueSetTransformer> _transformers;
		private List<IValueSetValidator> _validators;
		#endregion

		#region Constructors
		public EntityMaterializerAction(IValueSet source)
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

		#region Instance Methods
		protected virtual EntityInitializer<TAssociation> CreateInitializer()
		{
			Type type = typeof(IDbMaterializer<>).MakeGenericType(typeof(TAssociation));
			return new EntityInitializer<TAssociation>(type);
		}

		protected virtual bool Validate(IValueSet values)
		{
			foreach (IValueSetValidator validator in Validators)
				if (!validator.Valid(values))
					return false;

			return true;
		}

		public override TAssociation Execute(TAssociation entity)
		{
			EntityInitializer<TAssociation> initializer = CreateInitializer();
			IValueSet subset = Source;

			foreach (IValueSetTransformer transformer in Transformers)
				subset = transformer.Transform(subset);

			if (Validate(subset) && initializer != null)
				return initializer.Initialize(entity, subset);

			return entity;
		}
		#endregion
	}

	public class EntityMaterializerAction<TAssociation, TMaterializer>
		: EntityMaterializerAction<TAssociation>
		where TAssociation : class
		where TMaterializer : IDbMaterializer<TAssociation>
	{
		#region Constructors
		public EntityMaterializerAction(IValueSet source)
			: base(source)
		{
		}
		#endregion

		#region Instance Methods
		protected override EntityInitializer<TAssociation> CreateInitializer()
		{
			return new EntityInitializer<TAssociation>(typeof(TMaterializer));
		}
		#endregion
	}
}

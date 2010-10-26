using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public class EntityMaterializerAction<TAssociation>
		: EntityAssociatorAction<TAssociation>
		where TAssociation : class
	{
		#region Instance Fields
		private IValueSet _source;
		private IValueSetTransformer _transformer;
		private IValueSetValidator _validator;
		#endregion

		#region Constructors
		public EntityMaterializerAction(IValueSet source)
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

		#region Instance Methods
		protected virtual EntityInitializer<TAssociation> CreateInitializer()
		{
			Type type = typeof(IDbMaterializer<>).MakeGenericType(typeof(TAssociation));
			return new EntityInitializer<TAssociation>(type);
		}

		public override void Execute(TAssociation entity)
		{
			EntityInitializer<TAssociation> initializer = CreateInitializer();
			IValueSet subset = Transformer.Transform(Source);

			if (Validator.Valid(subset) && initializer != null)
				initializer.Initialize(entity, subset);
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

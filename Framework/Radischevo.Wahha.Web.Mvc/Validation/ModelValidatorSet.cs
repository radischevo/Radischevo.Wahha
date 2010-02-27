using System;
using System.Collections.Generic;
using System.Linq;

namespace Radischevo.Wahha.Web.Mvc.Validation
{
	public sealed class ModelValidatorSet
	{
		#region Nested Types
		private sealed class EmptyValidator : ModelValidator
		{
			#region Static Fields
			private static readonly ModelValidator _instance = new EmptyValidator();
			#endregion

			#region Constructors
			public EmptyValidator()
				: base(typeof(object))
			{
			}
			#endregion

			#region Static Properties
			public static ModelValidator Instance
			{
				get
				{
					return _instance;
				}
			}
			#endregion
		}
		#endregion

		#region Instance Fields
		private ModelValidator _model;
		private ModelValidator _container;
		#endregion

		#region Constructors
		public ModelValidatorSet()
			: this(null, null)
		{
		}

		public ModelValidatorSet(ModelValidator model)
			: this(model, null)
		{
		}

		public ModelValidatorSet(ModelValidator model,
			ModelValidator container)
		{
			_model = model ?? EmptyValidator.Instance;
			_container = container ?? EmptyValidator.Instance;
		}
		#endregion

		#region Instance Properties
		public ModelValidator Model
		{
			get
			{
				return _model;
			}
		}

		public ModelValidator Container
		{
			get
			{
				return _container;
			}
		}
		#endregion

		#region Instance Methods
		public IEnumerable<ModelValidationRule> GetValidationRules()
		{
			return _model.GetValidationRules()
				.Concat(_model.Properties.SelectMany(p => p.GetValidationRules()))
				.Concat(_container.GetValidationRules());
		}
		#endregion
	}
}

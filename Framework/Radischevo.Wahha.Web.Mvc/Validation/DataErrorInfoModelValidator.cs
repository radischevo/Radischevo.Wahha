using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Radischevo.Wahha.Web.Mvc.Validation
{
	public class DataErrorInfoModelValidator : ModelValidator
	{
		#region Nested Types
		private class DataErrorInfoTypeValidationRule : ModelValidationRule
		{
			#region Constructors
			public DataErrorInfoTypeValidationRule(DataErrorInfoModelValidator validator)
				: base(validator)
			{
			}
			#endregion

			#region Instance Methods
			public override ModelValidationResult Validate(ModelValidationContext context)
			{
				IDataErrorInfo errorInfo = (context.Value as IDataErrorInfo);
				if (errorInfo != null)
				{
					string errorMessage = errorInfo.Error;
					if (!String.IsNullOrEmpty(errorMessage))
					{
						return new ModelValidationResult() {
							Member = Member,
							Message = errorMessage
						};
					}
				}
				return null;
			}
			#endregion
		}

		private class DataErrorInfoPropertyValidationRule : ModelValidationRule
		{
			#region Constructors
			public DataErrorInfoPropertyValidationRule(DataErrorInfoModelValidator validator)
				: base(validator)
			{
			}
			#endregion

			#region Instance Methods
			public override ModelValidationResult Validate(ModelValidationContext context)
			{
				IDataErrorInfo errorInfo = (context.Container as IDataErrorInfo);
				if (errorInfo != null && !String.Equals(Member, "Error", StringComparison.OrdinalIgnoreCase))
				{
					string errorMessage = errorInfo[Member];
					if (!String.IsNullOrEmpty(errorMessage))
					{
						return new ModelValidationResult() {
							Member = Member,
							Message = errorMessage
						};
					}
				}
				return null;
			}
			#endregion
		}
		#endregion

		#region Constructors
		public DataErrorInfoModelValidator(Type modelType)
            : this(null, null, modelType)
        {
        }

		public DataErrorInfoModelValidator(DataErrorInfoModelValidator container,
			string propertyName, Type modelType)
			: base(container, propertyName, modelType)
		{
		}
		#endregion

		#region Static Methods
		private static bool TypeImplementsDataErrorInfo(Type type)
		{
			return typeof(IDataErrorInfo).IsAssignableFrom(type);
		}
		#endregion

		#region Instance Methods
		protected override ModelValidator CreateValidatorForProperty(PropertyDescriptor property)
		{
			return new DataErrorInfoModelValidator(this, property.Name, property.PropertyType);
		}

		public override IEnumerable<ModelValidationRule> GetValidationRules()
		{
			if (TypeImplementsDataErrorInfo(Type))
				yield return new DataErrorInfoTypeValidationRule(this);

			if (Container != null && TypeImplementsDataErrorInfo(Container.Type))
				yield return new DataErrorInfoPropertyValidationRule(this);
		}
		#endregion
	}
}

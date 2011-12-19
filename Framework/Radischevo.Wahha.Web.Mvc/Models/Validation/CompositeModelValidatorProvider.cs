using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data;

namespace Radischevo.Wahha.Web.Mvc
{
	public abstract class CompositeModelValidatorProvider : IModelValidatorProvider
	{
		#region Nested Types
		private sealed class PropertyValidator : ModelValidator
		{
			#region Instance Fields
			private PropertyDescriptor _property;
			private IEnumerable<ModelValidator> _validators;
			#endregion
			
			#region Constructors
			public PropertyValidator(PropertyDescriptor property, 
				IEnumerable<ModelValidator> validators)
			{
				_property = property;
				_validators = validators ?? Enumerable.Empty<ModelValidator>();
			}
			#endregion
			
			#region Instance Methods
			public override IEnumerable<ValidationError> Validate (ModelValidationContext context)
			{
				if (Object.ReferenceEquals(context.Model, null))
					return Enumerable.Empty<ValidationError>();
				
				string memberKey = (String.IsNullOrEmpty(context.Member)) 
					? _property.Name : String.Format("{0}-{1}", context.Member, _property.Name);
				
				ModelValidationContext inner = new ModelValidationContext(
					memberKey, context.Model, _property.GetValue(context.Model));

				return _validators.SelectMany(a => a.Validate(inner));
			}
			#endregion
		}
		#endregion
		
		#region Constructors
		protected CompositeModelValidatorProvider ()
		{
		}
		#endregion
		
		#region Instance Methods
		protected abstract ICustomTypeDescriptor GetTypeDescriptor(Type modelType);
		
		protected abstract IEnumerable<ModelValidator> GetTypeValidators(ICustomTypeDescriptor descriptor);
		
		protected abstract IEnumerable<ModelValidator> GetPropertyValidators(PropertyDescriptor property);
		
		public IEnumerable<ModelValidator> GetValidators(Type modelType) 
		{
			Precondition.Require(modelType, () => Error.ArgumentNull("modelType"));
			
			ICustomTypeDescriptor descriptor = GetTypeDescriptor(modelType);			
			return descriptor.GetProperties().Cast<PropertyDescriptor>()
				.Select(a => new PropertyValidator(a, GetPropertyValidators(a)))
				.Cast<ModelValidator>().Concat(GetTypeValidators(descriptor));
		}
		#endregion
	}
}


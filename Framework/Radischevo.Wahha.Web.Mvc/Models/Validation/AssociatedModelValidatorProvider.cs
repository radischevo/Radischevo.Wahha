using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Radischevo.Wahha.Web.Mvc
{
	public abstract class AssociatedModelValidatorProvider : CompositeModelValidatorProvider
	{
		#region Constructors
		protected AssociatedModelValidatorProvider ()
			: base()
		{
		}
		#endregion
		
		#region Instance Methods
		protected override sealed IEnumerable<ModelValidator> GetTypeValidators (ICustomTypeDescriptor descriptor)
		{
			return GetValidators(descriptor.GetAttributes().Cast<Attribute>());
		}
		
		protected override sealed IEnumerable<ModelValidator> GetPropertyValidators (PropertyDescriptor property)
		{
			return GetValidators(property.Attributes.Cast<Attribute>()
				.Concat(GetTypeDescriptor(property.PropertyType).GetAttributes().Cast<Attribute>()));
		}
		
		protected abstract IEnumerable<ModelValidator> GetValidators(IEnumerable<Attribute> attributes);
		#endregion
	}
}


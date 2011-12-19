using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data;

namespace Radischevo.Wahha.Web.Mvc
{
	public class DataAnnotationsModelValidator : ModelValidator
	{
		#region Instance Fields
		private readonly ValidationAttribute _attribute;
		#endregion
		
		#region Constructors
		protected DataAnnotationsModelValidator (ValidationAttribute attribute)
			: base()
		{
			Precondition.Require(attribute, () => Error.ArgumentNull("attribute"));
			_attribute = attribute;
		}
		#endregion
		
		#region Instance Properties
		protected ValidationAttribute Attribute
		{
			get
			{
				return _attribute;
			}
		}
		#endregion
		
		#region Instance Methods
		public override IEnumerable<ValidationError> Validate (ModelValidationContext context)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));
			if (!_attribute.IsValid(context.Model)) 
				yield return new ValidationError(context.Member, _attribute.ErrorMessage);
		}
		#endregion
	}
}


using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data;

namespace Radischevo.Wahha.Web.Mvc
{
	public abstract class ModelValidator
	{
		#region Constructors
		protected ModelValidator ()
		{
		}
		#endregion
		
		#region Instance Methods
		public abstract IEnumerable<ValidationError> Validate(ModelValidationContext context);
		#endregion
	}
}


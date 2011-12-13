using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data;

namespace Radischevo.Wahha.Web.Mvc
{
	public sealed class DataErrorInfoModelValidator : ModelValidator
	{
		#region Constructors
		public DataErrorInfoModelValidator ()
			: base()
		{
		}
		#endregion
		
		#region Instance Methods
		public override IEnumerable<ValidationError> Validate (ModelValidationContext context)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));
			
			IDataErrorInfo model = (context.Container as IDataErrorInfo);
            if (model != null)
			{
	            string message = model.Error;
	            if (!String.IsNullOrEmpty(message)) 
					return new ValidationError(context.Member, message).ToEnumerable();
			}
			return Enumerable.Empty<ValidationError>();
		}
		#endregion
	}
}


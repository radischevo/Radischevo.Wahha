using System;
using System.Collections.Generic;
using System.ComponentModel;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public sealed class DataErrorInfoModelValidatorProvider : IModelValidatorProvider
	{
		#region Constructors
		public DataErrorInfoModelValidatorProvider ()
		{
		}
		#endregion
		
		#region Instance Methods
		public IEnumerable<ModelValidator> GetValidators (Type modelType)
		{
			Precondition.Require(modelType, () => Error.ArgumentNull("modelType"));
			
			if (typeof(IDataErrorInfo).IsAssignableFrom(modelType))
				yield return new DataErrorInfoModelValidator();
		}
		#endregion
	}
}


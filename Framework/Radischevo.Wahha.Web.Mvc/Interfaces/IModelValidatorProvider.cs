using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Web.Mvc
{
	public interface IModelValidatorProvider
	{
		#region Instance Methods
		IEnumerable<ModelValidator> GetValidators(Type modelType);
		#endregion
	}
}


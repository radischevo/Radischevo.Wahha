using System;
using System.Collections.Generic;
using System.Linq;

namespace Radischevo.Wahha.Web.Mvc
{
	public sealed class EmptyModelValidatorProvider : IModelValidatorProvider
	{
		#region Constructors
		public EmptyModelValidatorProvider ()
		{
		}
		#endregion

		#region Instance Methods
		public IEnumerable<ModelValidator> GetValidators (Type modelType)
		{
			return Enumerable.Empty<ModelValidator>();
		}
		#endregion
	}
}


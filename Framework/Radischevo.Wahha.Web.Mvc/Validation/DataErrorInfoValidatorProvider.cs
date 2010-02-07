using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Validation
{
	public class DataErrorInfoValidatorProvider : ModelValidatorProvider
	{
		#region Constructors
		public DataErrorInfoValidatorProvider()
			: base()
		{
		}
		#endregion

		#region Instance Methods
		public override ModelValidator GetValidator(Type type)
		{
			Precondition.Require(type, Error.ArgumentNull("type"));
			return new DataErrorInfoModelValidator(type);
		}
		#endregion
	}
}

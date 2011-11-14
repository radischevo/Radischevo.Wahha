using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public sealed class DefinedSchemeValidator : SubsetSchemeValidator
	{
		#region Constructors
		public DefinedSchemeValidator(params string[] columns)
			: base(columns)
		{
		}
		#endregion

		#region Instance Methods
		public override bool Valid(IDbValueSet source)
		{
			Precondition.Require(source, () => Error.ArgumentNull("source"));
			foreach (string key in Scheme)
			{
				if (Object.ReferenceEquals(null, source.GetValue<object>(key)))
					return false;
			}
			return true;
		}
		#endregion
	}
}
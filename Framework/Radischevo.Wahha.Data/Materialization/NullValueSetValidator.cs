using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	internal sealed class NullValueSetValidator : IDbValueSetValidator
	{
		#region Constructors
		public NullValueSetValidator()
		{
		}
		#endregion

		#region Instance Methods
		public bool Valid(IDbValueSet source)
		{
			return source.ContainsAny();
		}
		#endregion
	}
}

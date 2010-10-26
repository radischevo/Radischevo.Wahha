using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	internal sealed class NullValueSetValidator : IValueSetValidator
	{
		#region Constructors
		public NullValueSetValidator()
		{
		}
		#endregion

		#region Instance Methods
		public bool Valid(IValueSet source)
		{
			return source.ContainsAny();
		}
		#endregion
	}
}

using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	internal sealed class NullValueSetTransformer : IValueSetTransformer
	{
		#region Constructors
		public NullValueSetTransformer()
		{
		}
		#endregion

		#region Instance Methods
		public IValueSet Transform(IValueSet source)
		{
			return source;
		}
		#endregion
	}
}

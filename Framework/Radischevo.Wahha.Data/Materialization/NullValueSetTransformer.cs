using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	internal sealed class NullValueSetTransformer : IDbValueSetTransformer
	{
		#region Constructors
		public NullValueSetTransformer()
		{
		}
		#endregion

		#region Instance Methods
		public IDbValueSet Transform(IDbValueSet source)
		{
			return source;
		}
		#endregion
	}
}

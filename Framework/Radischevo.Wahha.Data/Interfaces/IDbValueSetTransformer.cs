using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public interface IDbValueSetTransformer
	{
		#region Instance Methods
		IDbValueSet Transform(IDbValueSet source);
		#endregion
	}
}
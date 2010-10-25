using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public interface IValueSetTransformer
	{
		#region Instance Methods
		IValueSet Transform(IValueSet source);
		#endregion
	}
}
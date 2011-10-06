using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public interface IDbValueSetValidator
	{
		#region Instance Methods
		bool Valid(IDbValueSet source);
		#endregion
	}
}

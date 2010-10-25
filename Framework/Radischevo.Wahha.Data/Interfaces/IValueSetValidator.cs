using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public interface IValueSetValidator
	{
		#region Instance Methods
		bool Valid(IValueSet source);
		#endregion
	}
}

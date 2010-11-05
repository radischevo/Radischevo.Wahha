using System;
using System.Collections;

namespace Radischevo.Wahha.Core
{
	public interface IStructuralEquatable
	{
		#region Instance Methods
		bool Equals(object other, IEqualityComparer comparer);

		int GetHashCode(IEqualityComparer comparer);
		#endregion
	}
}

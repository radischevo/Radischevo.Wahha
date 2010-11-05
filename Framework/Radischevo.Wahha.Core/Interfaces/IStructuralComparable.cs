using System;
using System.Collections;

namespace Radischevo.Wahha.Core
{
	public interface IStructuralComparable
	{
		#region Instance Methods
		int CompareTo(object other, IComparer comparer);
		#endregion
	}
}

using System;
using System.Collections;
using System.Text;

namespace Radischevo.Wahha.Core
{
	public interface ITuple
	{
		#region Instance Properties
		int Size
		{
			get;
		}
		#endregion

		#region Instance Methods
		int GetHashCode(IEqualityComparer comparer);

		string ToString(StringBuilder sb);
		#endregion
	}
}

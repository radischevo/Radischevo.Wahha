using System;
using System.Collections.Generic;
using System.Linq;

namespace Radischevo.Wahha.Data
{
	public static class SubsetExtensions
	{
		#region Extension Methods
		/// <summary>
		/// Returns the total number of elements that can be 
		/// retrieved from the sequence.
		/// </summary>
		public static int Total<T>(this IEnumerable<T> collection)
		{
			ISubset subset = (collection as ISubset);
			if (subset == null)
				return collection.Count();

			return subset.Total;
		}
		#endregion
	}
}

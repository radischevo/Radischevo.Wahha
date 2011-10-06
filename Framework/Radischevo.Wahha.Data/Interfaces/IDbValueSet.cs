using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public interface IDbValueSet : IValueSet
	{
		#region Instance Properties
		/// <summary>
		/// Gets the collection of column names, 
		/// which values were accessed by calling 
		/// value-getter methods of the current row.
		/// </summary>
		IEnumerable<string> AccessedKeys
		{
			get;
		}
		#endregion
	}
}

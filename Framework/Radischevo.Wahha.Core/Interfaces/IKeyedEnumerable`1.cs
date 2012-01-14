using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Core
{
	public interface IKeyedEnumerable<TKey, TValue> : IEnumerable<TValue>
	{
		#region Instance Properties
		TValue this[TKey key] 
		{
			get;
		}
		#endregion
	}
}


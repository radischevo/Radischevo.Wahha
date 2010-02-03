using System;
using System.Collections;
using System.Collections.Generic;

namespace Radischevo.Wahha.Data
{
	public interface ISubset : IEnumerable
	{
		#region Instance Properties
		int Total
		{
			get;
			set;
		}
		#endregion
	}

	public interface ISubset<T> : ISubset, IEnumerable<T>
	{
	}
}

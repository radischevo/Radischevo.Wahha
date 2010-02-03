using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	internal sealed class Subset<T> : ISubset<T>
	{
		#region Instance Fields
		private IEnumerable<T> _collection;
		private int _total;
		#endregion

		#region Constructors
		public Subset(IEnumerable<T> collection)
			: this(collection, -1)
		{
		}

		public Subset(IEnumerable<T> collection, int total)
		{
			Precondition.Require(collection, Error.ArgumentNull("collection"));
			_collection = collection;
			_total = total;
		}
		#endregion

		#region Instance Properties
		public int Total
		{
			get
			{
				if (_total < 0)
					_total = _collection.Count();

				return _total;
			}
			set
			{
				_total = value;
			}
		}
		#endregion

		#region Instance Methods
		public IEnumerator<T> GetEnumerator()
		{
			return _collection.GetEnumerator();
		}
		#endregion

		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion
	}
}

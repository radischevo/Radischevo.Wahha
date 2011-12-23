using System;
using System.Collections;
using System.Collections.Generic;

namespace Radischevo.Wahha.Core
{
	public class NameValueCollection<T> : IEnumerable<T>, IEnumerable
	{
		#region Instance Fields
		private Dictionary<string, T> _contents;
		#endregion
		
		#region Constructors
		public NameValueCollection ()
			: this(StringComparer.OrdinalIgnoreCase)
		{
		}
		
		public NameValueCollection (IEqualityComparer<string> keyComparer)
		{
			_contents = new Dictionary<string, T>(keyComparer);		
		}
		#endregion
		
		#region Instance Properties
		public virtual int Count 
		{
			get 
			{
				return _contents.Count;
			}
		}
		
		public virtual IEnumerable<string> Keys
		{
			get
			{
				return _contents.Keys;
			}
		}
		
		public virtual T this[string name] 
		{
			get
			{
				return _contents[name.Define()];
			}
			set
			{
				_contents[name.Define()] = value;
			}
		}
		#endregion
		
		#region Instance Methods
		public virtual void Add (string name, T item)
		{
			_contents[name.Define()] = item;
		}

		public virtual void Clear ()
		{
			_contents.Clear();
		}

		public virtual bool Contains (string name)
		{
			return _contents.ContainsKey(name.Define());
		}

		public virtual void CopyTo (T[] array, int index)
		{
			_contents.Values.CopyTo(array, index);
		}

		public virtual bool Remove (string name)
		{
			return _contents.Remove(name);
		}
		
		public virtual IEnumerator<T> GetEnumerator ()
		{
			return _contents.Values.GetEnumerator();
		}
		#endregion

		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator();
		}
		#endregion
	}
}


using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Radischevo.Wahha.Core
{
	/// <summary>
	/// Represents a read-only collection of keys and values.
	/// </summary>
	[Serializable]
	public sealed class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializable
	{
		#region Instance Fields
		private IDictionary<TKey, TValue> _contents;
		#endregion
		
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Radischevo.Wahha.Core.ReadOnlyDictionary{TKey,TValue}"/> class.
		/// </summary>
		public ReadOnlyDictionary ()
			: this(new Dictionary<TKey, TValue>())
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Radischevo.Wahha.Core.ReadOnlyDictionary{TKey,TValue}"/> class 
		/// that wraps the specified <see cref="System.Collections.Generic.IDictionary{TKey,TValue}"/>.
		/// </summary>
		/// <param name="dictionary">
		/// The <see cref="System.Collections.Generic.IDictionary{TKey,TValue}"/> to wrap.
		/// </param>
		public ReadOnlyDictionary (IDictionary<TKey, TValue> dictionary)
		{
			Precondition.Require(dictionary, () => Error.ArgumentNull("dictionary"));
			_contents = dictionary;
		}
		
		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="Radischevo.Wahha.Core.ReadOnlyDictionary{TKey,TValue}"/> class with serialized data
		/// </summary>
		/// <param name="info">An object containing the information required to 
		/// serialize the dictionary</param>
		/// <param name="context">A structure containing the source and destination 
		/// of the serialized stream associated with the dictionary</param>
		private ReadOnlyDictionary(SerializationInfo info, StreamingContext context)
			: this()
		{
			Precondition.Require(info, () => Error.ArgumentNull("info"));
			KeyValuePair<TKey, TValue>[] array = (KeyValuePair<TKey, TValue>[])info
				.GetValue("items", typeof(KeyValuePair<TKey, TValue>));
			
			if (array != null) 
			{
				for (int i = 0; i < array.Length; ++i) 
					_contents.Add(array[i]);
			}
		}
		#endregion
		
		#region Instance Properties
		/// <summary>
		/// Gets the element with the specified key.
		/// </summary>
		/// <param name="key">The key of the element to get.</param>
		public TValue this[TKey key] 
		{
			get 
			{
				return _contents[key];
			}
			set 
			{
				throw Error.DictionaryIsReadOnly();
			}
		}
		
		/// <summary>
		/// Gets the number of elements contained in the collection.
		/// </summary>
		public int Count 
		{
			get 
			{
				return _contents.Count;
			}
		}
		
		/// <summary>
		/// Gets an <see cref="System.Collections.Generic.ICollection{TKey}"/> containing the keys of
		/// the <see cref="System.Collections.Generic.IDictionary{TKey,TValue}"/>.
		/// </summary>
		public ICollection<TKey> Keys 
		{
			get 
			{
				return _contents.Keys;
			}
		}
		
		/// <summary>
		/// Gets an <see cref="System.Collections.Generic.ICollection{TValue}"/> containing the values of
		/// the <see cref="System.Collections.Generic.IDictionary{TKey,TValue}"/>.
		/// </summary>
		public ICollection<TValue> Values 
		{
			get 
			{
				return _contents.Values;
			}
		}
		#endregion
		
		#region Instance Methods
		/// <summary>
		/// Determines whether the <see cref="Radischevo.Wahha.Core.ReadOnlyDictionary{TKey,TValue}"/> 
		/// contains the specified key.
		/// </summary>
		/// <param name="key">
		/// The key to locate in the <see cref="Radischevo.Wahha.Core.ReadOnlyDictionary{TKey,TValue}"/>.
		/// </param>
		public bool ContainsKey (TKey key)
		{
			return _contents.ContainsKey(key);
		}
		
		/// <summary>
		/// Gets the value associated with the specified key.
		/// </summary>
		/// <param name="key">The key of the value to get.</param>
		/// <param name="value">When this method returns, contains the value associated with the specified
		/// key, if the key is found; otherwise, the default value for the type of the
		/// value parameter. This parameter is passed uninitialized.</param>
		public bool TryGetValue (TKey key, out TValue value)
		{
			return _contents.TryGetValue(key, out value);
		}
		
		/// <summary>
		/// Returns an enumerator that iterates through the <see cref="Radischevo.Wahha.Core.ReadOnlyDictionary{TKey,TValue}"/>.
		/// </summary>
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator ()
		{
			return _contents.GetEnumerator();
		}
		#endregion
		
		#region Interface implementations
		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly 
		{
			get 
			{
				return true;
			}
		}
		
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator();
		}
		
		bool ICollection<KeyValuePair<TKey, TValue>>.Contains (KeyValuePair<TKey, TValue> item)
		{
			return _contents.Contains(item);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo (KeyValuePair<TKey, TValue>[] array, int index)
		{
			_contents.CopyTo(array, index);
		}
		
		void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
		{
			Precondition.Require(info, () => Error.ArgumentNull("info"));
			
			KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[_contents.Count];
			if (_contents.Count > 0)
				_contents.CopyTo (array, 0);
			
			info.AddValue ("items", array);
		}
		
		void ICollection<KeyValuePair<TKey, TValue>>.Add (KeyValuePair<TKey, TValue> item)
		{
			throw Error.DictionaryIsReadOnly();
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Clear ()
		{
			throw Error.DictionaryIsReadOnly();
		}
		
		bool ICollection<KeyValuePair<TKey, TValue>>.Remove (KeyValuePair<TKey, TValue> item)
		{
			throw Error.DictionaryIsReadOnly();
		}
		
		void IDictionary<TKey, TValue>.Add (TKey key, TValue value)
		{
			throw Error.DictionaryIsReadOnly();
		}		

		bool IDictionary<TKey, TValue>.Remove (TKey key)
		{
			throw Error.DictionaryIsReadOnly();
		}
		#endregion
	}
}


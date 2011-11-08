using System;
using System.Collections;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public sealed class ValidationErrorCollection : ICollection<ValidationError>
	{
		#region Instance Fields
		private List<ValidationError> _collection;
		#endregion

		#region Constructors
		public ValidationErrorCollection()
			: this(4)
		{
		}

		public ValidationErrorCollection(int capacity)
		{
			_collection = new List<ValidationError>(capacity);
		}

		public ValidationErrorCollection(IEnumerable<ValidationError> errors)
		{
			Precondition.Require(errors, () => Error.ArgumentNull("errors"));
			_collection = new List<ValidationError>(errors);
		}
		#endregion

		#region Instance Properties
		public int Count
		{
			get
			{
				return _collection.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}
		#endregion

		#region Instance Methods
		public void Add(string message)
		{
			Add(new ValidationError(message));
		}

		public void Add(Exception exception)
		{
			Add(new ValidationError(exception));
		}

		public void Add(string key, string message)
		{
			Add(new ValidationError(key, message));
		}

		public void Add(string key, Exception exception)
		{
			Add(new ValidationError(key, exception));
		}

		public void Add(ValidationError item)
		{
			Precondition.Require(item, () => Error.ArgumentNull("item"));
			_collection.Add(item);
		}

		public void Clear()
		{
			_collection.Clear();
		}

		public bool Contains(string key)
		{
			return _collection.Exists(a => String.Equals(a.Key, 
				key, StringComparison.Ordinal));
		}

		public bool Contains(ValidationError item)
		{
			return _collection.Contains(item);
		}

		public void CopyTo(ValidationError[] array, int arrayIndex)
		{
			_collection.CopyTo(array, arrayIndex);
		}

		public void Remove(string key)
		{
			_collection.RemoveAll(a => String.Equals(a.Key,
				key, StringComparison.Ordinal));
		}

		public bool Remove(ValidationError item)
		{
			return _collection.Remove(item);
		}

		public ValidationError[] ToArray()
		{
			return _collection.ToArray();
		}
		
		public IEnumerator<ValidationError> GetEnumerator()
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

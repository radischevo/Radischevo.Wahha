using System;
using System.Collections;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Configurations
{
	public sealed class DbDataProviderFactoryCollection : IEnumerable<IDbDataProviderFactory>
	{
		#region Instance Fields
		private IDbDataProviderFactory _default;
		private Dictionary<string, IDbDataProviderFactory> _collection;
		#endregion
		
		#region Constructors
		public DbDataProviderFactoryCollection()
		{
			_collection = new Dictionary<string, IDbDataProviderFactory>(
				StringComparer.OrdinalIgnoreCase);
		}
		#endregion
		
		#region Instance Properties
		public IDbDataProviderFactory this[string name]
		{
			get
			{
				IDbDataProviderFactory factory;
				if (_collection.TryGetValue(name, out factory))
					return factory;
				
				return null;
			}
		}
		
		public IDbDataProviderFactory Default
		{
			get
			{
				if (_default == null)
					_default = GetDefaultFactory();
				
				return _default;
			}
		}
		
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
		private IDbDataProviderFactory GetDefaultFactory()
		{
			// If we have an explicitly defined default factory, return it.
			IDbDataProviderFactory factory;
			if (_collection.TryGetValue("default", out factory))
				return factory;
			
			// Then we take the first available factory
			foreach (IDbDataProviderFactory item in _collection.Values)
				return item;
			
			return null;
		}
		
		public void Add (string name, IDbDataProviderFactory item)
		{
			Precondition.Defined (name, () => Error.ArgumentNull("name"));
			Precondition.Require (item, () => Error.ArgumentNull("item"));
			
			_collection[name] = item;
			_default = null;
		}

		public void Clear ()
		{
			_collection.Clear();
			_default = null;
		}

		public bool Contains (IDbDataProviderFactory item)
		{
			return _collection.ContainsValue(item);
		}

		public bool Remove (string name)
		{
			if (_collection.Remove(name)) 
			{
				_default = null;
				return true;
			}
			return false;                              
		}
		
		public IEnumerator<IDbDataProviderFactory> GetEnumerator ()
		{
			return _collection.Values.GetEnumerator();
		}
		#endregion

		#region IEnumerable implementation
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator();
		}
		#endregion
	}
}


using System;
using System.Collections.Generic;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public class ValueProviderCollection : IValueProvider
	{
		#region Instance Fields
		private HashSet<string> _keys;
		private List<IValueProvider> _collection;
		#endregion

		#region Constructors
		public ValueProviderCollection(IEnumerable<IValueProvider> providers)
		{
			Precondition.Require(providers, () => Error.ArgumentNull("providers"));
			_collection = new List<IValueProvider>(providers);
		}
		#endregion

		#region Instance Properties
		public IEnumerable<string> Keys
		{
			get
			{
				if (_keys == null)
				{
					_keys = new HashSet<string>();
					foreach (IValueProvider provider in _collection)
						_keys.UnionWith(provider.Keys);
				}
				return _keys;
			}
		}
		#endregion

		#region Instance Methods
		public bool Contains(string prefix)
		{
			return _collection.Any(p => p.Contains(prefix));
		}

		public ValueProviderResult GetValue(string key)
		{
			return _collection.Select(p => p.GetValue(key))
				.Where(r => r != null).FirstOrDefault();
		}
		#endregion
	}
}

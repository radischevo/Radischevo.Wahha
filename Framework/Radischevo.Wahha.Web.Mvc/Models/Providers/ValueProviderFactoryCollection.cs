using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public sealed class ValueProviderFactoryCollection 
		: IEnumerable<IValueProviderFactory>
	{
		#region Instance Fields
		private Dictionary<string, IValueProviderFactory> _collection;
		#endregion

		#region Constructors
		public ValueProviderFactoryCollection()
		{
			_collection = new Dictionary<string, IValueProviderFactory>(
				StringComparer.OrdinalIgnoreCase);
		}
		#endregion

		#region Instance Methods
		private IEnumerable<IValueProviderFactory> FilterBy(ParameterSource source)
		{
			IValueProviderFactory factory;
			foreach(string name in source.Sources)
				if (_collection.TryGetValue(name, out factory))
					yield return factory;
		}

		public void Add(string name, IValueProviderFactory factory)
		{
			Precondition.Require(factory, () => Error.ArgumentNull("factory"));
			_collection[name] = factory;
		}

		public void Clear()
		{
			_collection.Clear();
		}

		public IValueProvider GetProvider(ControllerContext context)
		{
			return new ValueProviderCollection(
				_collection.Values.Reverse()
					.Select(c => c.Create(context))
					.Where(p => p != null)
				);
		}

		public IValueProvider GetProvider(ControllerContext context, ParameterSource source)
		{
			if (source.AllowAll)
				return GetProvider(context);

			return new ValueProviderCollection(
				FilterBy(source)
					.Select(c => c.Create(context))
					.Where(p => p != null)
				);
		}

		public bool Remove(string name)
		{
			return _collection.Remove(name);
		}
		#endregion

		#region IEnumerable Members
		public IEnumerator<IValueProviderFactory> GetEnumerator()
		{
			return _collection.Values.Reverse().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion
	}
}

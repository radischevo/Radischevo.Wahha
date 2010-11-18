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
		#region Nested Types
		private sealed class OrdinalWrapper : IValueProviderFactory
		{
			#region Instance Fields
			private int _order;
			private IValueProviderFactory _item;
			#endregion

			#region Constructors
			public OrdinalWrapper(int order, 
				IValueProviderFactory item)
			{
				_item = item;
				_order = order;
			}
			#endregion

			#region Instance Properties
			public int Order
			{
				get
				{
					return _order;
				}
			}
			#endregion

			#region Instance Methods
			public IValueProvider Create(ControllerContext context)
			{
				return _item.Create(context);
			}
			#endregion
		}
		#endregion

		#region Instance Fields
		private Dictionary<string, OrdinalWrapper> _collection;
		#endregion

		#region Constructors
		public ValueProviderFactoryCollection()
		{
			_collection = new Dictionary<string, OrdinalWrapper>(
				StringComparer.OrdinalIgnoreCase);
		}
		#endregion

		#region Instance Methods
		private IEnumerable<OrdinalWrapper> FilterBy(ParameterSource source)
		{
			OrdinalWrapper factory;

			foreach(string name in source.Sources)
				if (_collection.TryGetValue(name, out factory))
					yield return factory;
		}

		public void Add(string name, IValueProviderFactory factory)
		{
			Insert(name, 0, factory);
		}

		public void Insert(string name, int order, IValueProviderFactory factory)
		{
			Precondition.Require(factory, () => Error.ArgumentNull("factory"));
			_collection[name] = new OrdinalWrapper(order, factory);
		}

		public IValueProvider GetProvider(ControllerContext context)
		{
			return new ValueProviderCollection(
				_collection.Values
					.OrderBy(i => i.Order)
					.Select(c => c.Create(context))
					.Where(p => p != null)
				);
		}

		public IValueProvider GetProvider(ControllerContext context, ParameterSource source)
		{
			return new ValueProviderCollection(
				FilterBy(source)
					.OrderBy(i => i.Order)
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
			return _collection.Values
				.OrderBy(i => i.Order)
				.Cast<IValueProviderFactory>()
				.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion
	}
}

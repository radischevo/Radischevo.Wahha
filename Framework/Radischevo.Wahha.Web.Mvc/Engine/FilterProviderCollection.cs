using System;
using System.Collections.Generic;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public class FilterProviderCollection
	{
		#region Nested Types
		private sealed class FilterComparer : IComparer<Filter>
		{
			#region Instance Methods
			public int Compare(Filter x, Filter y)
			{
				if(Object.ReferenceEquals(x, y))
					return 0;
				
				if (Object.ReferenceEquals(x, null))
					return -1;

				if (Object.ReferenceEquals(y, null))
					return 1;

				int order = x.Order.CompareTo(y.Order);
				return (order == 0) ? x.Scope.CompareTo(y.Scope) : order;
			}
			#endregion
		}
		#endregion

		#region Instance Fields
		private List<IFilterProvider> _providers;
		private IComparer<Filter> _comparer;
		#endregion

		#region Constructors
		public FilterProviderCollection()
		{
			_providers = new List<IFilterProvider>();
			_comparer = new FilterComparer();
		}
		#endregion

		#region Static Methods
		private static bool AllowMultiple(object instance)
		{
			IMvcFilter filter = (instance as IMvcFilter);
			return (filter == null) ? true : filter.AllowMultiple;
		}
		#endregion

		#region Instance Methods
		public void Add(IFilterProvider provider)
		{
			Precondition.Require(provider, () => 
				Error.ArgumentNull("provider"));

			_providers.Add(provider);
		}

		private IEnumerable<Filter> RemoveDuplicates(IEnumerable<Filter> filters)
		{
			HashSet<Type> visitedTypes = new HashSet<Type>();
			foreach (Filter filter in filters)
			{
				object instance = filter.Instance;
				Type type = instance.GetType();

				if (!visitedTypes.Contains(type) || AllowMultiple(instance))
				{
					yield return filter;
					visitedTypes.Add(type);
				}
			}
		}

		public IEnumerable<Filter> GetFilters(ControllerContext context, ActionDescriptor action)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));
			Precondition.Require(action, () => Error.ArgumentNull("action"));

			IEnumerable<Filter> filters = _providers
				.SelectMany(p => p.GetFilters(context, action))
				.OrderBy(f => f, _comparer);

			return RemoveDuplicates(filters.Reverse()).Reverse();
		}

		public bool Remove(IFilterProvider provider)
		{
			return _providers.Remove(provider);
		}
		#endregion
	}
}

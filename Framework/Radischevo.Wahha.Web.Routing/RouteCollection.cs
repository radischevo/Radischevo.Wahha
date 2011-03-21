using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Routing
{
	/// <summary>
	/// Provides a collection of routes for ASP.NET routing.
	/// </summary>
	public class RouteCollection : IEnumerable<RouteBase>
	{
		#region Nested Types
		private class ReadLockDisposable : IDisposable
		{
			#region Instance Fields
			private ReaderWriterLock _lock;
			#endregion

			#region Constructors
			public ReadLockDisposable(ReaderWriterLock @lock)
			{
				_lock = @lock;
			}
			#endregion

			#region Instance Methods
			void IDisposable.Dispose()
			{
				_lock.ReleaseReaderLock();
			}
			#endregion
		}

		private class WriteLockDisposable : IDisposable
		{
			#region Instance Fields
			private ReaderWriterLock _lock;
			#endregion

			#region Constructors
			public WriteLockDisposable(ReaderWriterLock @lock)
			{
				_lock = @lock;
			}
			#endregion

			#region Instance Methods
			void IDisposable.Dispose()
			{
				_lock.ReleaseWriterLock();
			}
			#endregion
		}
		#endregion

		#region Instance Fields
		private ValueDictionary _variables;
		private Dictionary<string, RouteBase> _indexedRoutes;
		private List<RouteBase> _nonIndexedRoutes;
		private ReaderWriterLock _lock;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="RouteCollection"/> class
		/// </summary>
		public RouteCollection()
		{
			_variables = new ValueDictionary();
			_indexedRoutes = new Dictionary<string, RouteBase>();
			_nonIndexedRoutes = new List<RouteBase>();
			_lock = new ReaderWriterLock();
		}
		#endregion

		#region Instance Properties
		/// <summary>
		/// Gets the <see cref="RouteBase"/> 
		/// with the specified key
		/// </summary>
		/// <param name="key">The key to find</param>
		public RouteBase this[string key]
		{
			get
			{
				if (String.IsNullOrEmpty(key))
					return null;

				RouteBase rb;
				_indexedRoutes.TryGetValue(key, out rb);

				return rb;
			}
		}

		/// <summary>
		/// Gets the collection of global variables 
		/// used by routing subsystem.
		/// </summary>
		public ValueDictionary Variables
		{
			get
			{
				return _variables;
			}
		}
		#endregion

		#region Instance Methods
		private IDisposable GetReadLock()
		{
			_lock.AcquireReaderLock(-1);
			return new ReadLockDisposable(_lock);
		}

		private IDisposable GetWriteLock()
		{
			_lock.AcquireWriterLock(-1);
			return new WriteLockDisposable(_lock);
		}

		/// <summary>
		/// Adds a new <see cref="RouteBase"/> to the collection.
		/// </summary>
		/// <param name="item">The value of the element to add.</param>
		public void Add(RouteBase item)
		{
			Add(null, item);
		}

		/// <summary>
		/// Adds a new <see cref="RouteBase"/> to the collection.
		/// </summary>
		/// <param name="key">The key of the element to add.</param>
		/// <param name="item">The value of the element to add.</param>
		public virtual void Add(string key, RouteBase item)
		{
			Precondition.Require(item, () => Error.ArgumentNull("item"));

			using (GetWriteLock())
			{
				_nonIndexedRoutes.Add(item);
				if (!String.IsNullOrEmpty(key))
					_indexedRoutes[key] = item;
			}
		}

		/// <summary>
		/// Adds a range of <see cref="RouteBase"/> elements 
		/// to the collection.
		/// </summary>
		/// <param name="values">The values to add.</param>
		public void AddRange(IDictionary<string, RouteBase> values)
		{
			using (GetWriteLock())
			{
				foreach (KeyValuePair<string, RouteBase> kvp in values)
				{
					_nonIndexedRoutes.Add(kvp.Value);
					if (!String.IsNullOrEmpty(kvp.Key))
						_indexedRoutes[kvp.Key] = kvp.Value;
				}
			}
		}

		/// <summary>
		/// Adds a range of <see cref="RouteBase"/> elements 
		/// to the collection.
		/// </summary>
		/// <param name="values">The values to add.</param>
		public void AddRange(IEnumerable<RouteBase> values)
		{
			using (GetWriteLock())
			{
				foreach (RouteBase route in values)
					_nonIndexedRoutes.Add(route);
			}
		}

		/// <summary>
		/// Removes all keys and values from 
		/// the collection
		/// </summary>
		public virtual void Clear()
		{
			using (GetWriteLock())
			{
				_indexedRoutes.Clear();
				_nonIndexedRoutes.Clear();
			}
		}

		/// <summary>
		/// Removes the specified <see cref="RouteBase"/> from the collection.
		/// </summary>
		/// <param name="item">An element to remove.</param>
		public virtual void Remove(RouteBase item)
		{
			using (GetWriteLock())
			{
				foreach (KeyValuePair<string, RouteBase> kvp in _indexedRoutes)
				{
					if (kvp.Value == item)
					{
						_indexedRoutes.Remove(kvp.Key);
						break;
					}
				}
				_nonIndexedRoutes.Remove(item);
			}
		}

		/// <summary>
		/// Removes the <see cref="RouteBase"/> with the specified index 
		/// from the collection.
		/// </summary>
		/// <param name="index">An index of the element to remove.</param>
		public void Remove(int index)
		{
			RouteBase item = _nonIndexedRoutes[index];
			Remove(item);
		}

		/// <summary>
		/// Removes the <see cref="RouteBase"/> with 
		/// the specified key from the collection.
		/// </summary>
		/// <param name="key">The key of the element to remove.</param>
		public void Remove(string key)
		{
			RouteBase item = _indexedRoutes[key];
			Remove(item);
		}

		/// <summary>
		/// Finds the appropriate <see cref="RouteBase"/> 
		/// for the current request handling and gets 
		/// the <see cref="RouteData"/> for the current request. 
		/// This method returns null, if no route matched the request.
		/// </summary>
		/// <param name="context">The current <see cref="HttpContextBase"/> object.</param>
		public virtual RouteData GetRouteData(HttpContextBase context)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));
			Precondition.Require(context.Request, () => Error.ArgumentNull("request"));

			using (GetReadLock())
			{
				foreach (RouteBase r in _nonIndexedRoutes)
				{
					RouteData data = r.GetRouteData(context, Variables);
					if (data != null)
						return data;
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the appropriate <see cref="VirtualPathData"/> for 
		/// the provided <paramref name="values"/>, 
		/// or <value>null</value>, if no matching route found.
		/// </summary>
		/// <param name="context">The context of the current request.</param>
		/// <param name="values">The <see cref="ValueDictionary"/> 
		/// containing the route parameter values.</param>
		public virtual VirtualPathData GetVirtualPath(RequestContext context, ValueDictionary values)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));
			Precondition.Require(values, () => Error.ArgumentNull("values"));

			using (GetReadLock())
			{
				foreach (RouteBase route in _nonIndexedRoutes)
				{
					VirtualPathData vp = route.GetVirtualPath(context, values, Variables);
					if (vp != null)
						return vp;
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the appropriate <see cref="VirtualPathData"/> for 
		/// the provided route <paramref name="key"/> and <paramref name="values"/>, 
		/// or <value>null</value>, if no matching route found.
		/// </summary>
		/// <param name="context">The context of the current request.</param>
		/// <param name="key">The key of the route to use.</param>
		/// <param name="values">The <see cref="ValueDictionary"/> 
		/// containing the route parameter values.</param>
		public virtual VirtualPathData GetVirtualPath(RequestContext context,
			string key, ValueDictionary values)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));
			Precondition.Require(values, () => Error.ArgumentNull("values"));

			if (String.IsNullOrEmpty(key))
				return GetVirtualPath(context, values);

			RouteBase route = null;
			bool hasRoute = false;

			using(GetReadLock())
			{
				hasRoute = _indexedRoutes.TryGetValue(key, out route);
			}

			if (hasRoute)
			{
				VirtualPathData vp = route.GetVirtualPath(context, values, Variables);
				if (vp != null)
					return vp;
			}
			return null;
		}
		#endregion

		#region IEnumerable Members
		public IEnumerator<RouteBase> GetEnumerator()
		{
			return _nonIndexedRoutes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion
	}
}

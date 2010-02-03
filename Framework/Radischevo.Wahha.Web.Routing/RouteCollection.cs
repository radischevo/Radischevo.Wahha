using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Routing
{
    /// <summary>
    /// Provides a collection of objects 
    /// of the <see cref="RouteBase"/> class
    /// </summary>
    public class RouteCollection
    {
        #region Instance Fields
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
        #endregion

        #region Instance Methods
        /// <summary>
        /// Adds a new <see cref="RouteBase"/> to the collection
        /// </summary>
        /// <param name="item">The value of the element to add</param>
        public void Add(RouteBase item)
        {
            Add(null, item);            
        }

        /// <summary>
        /// Adds a new <see cref="RouteBase"/> to the collection
        /// </summary>
        /// <param name="key">The key of the element to add</param>
        /// <param name="item">The value of the element to add</param>
        public virtual void Add(string key, RouteBase item)
        {
            Precondition.Require(item, Error.ArgumentNull("item"));

            try
            {
                _lock.AcquireWriterLock(-1);
                _nonIndexedRoutes.Add(item);
                
                if(!String.IsNullOrEmpty(key))
                    _indexedRoutes[key] = item;
            }
            finally
            {
                _lock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Adds a range of <see cref="RouteBase"/> elements 
        /// to the collection
        /// </summary>
        /// <param name="values">The values to add</param>
        public void AddRange(IDictionary<string, RouteBase> values)
        {
            try
            {
                _lock.AcquireWriterLock(-1);

                foreach (KeyValuePair<string, RouteBase> kvp in values)
                {
                    _nonIndexedRoutes.Add(kvp.Value);

                    if (!String.IsNullOrEmpty(kvp.Key))
                        _indexedRoutes[kvp.Key] = kvp.Value;
                }
            }
            finally
            {
                _lock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Adds a range of <see cref="RouteBase"/> elements 
        /// to the collection
        /// </summary>
        /// <param name="values">The values to add</param>
        public void AddRange(IEnumerable<RouteBase> values)
        {
            try
            {
                _lock.AcquireWriterLock(-1);

                foreach (RouteBase route in values)
                    _nonIndexedRoutes.Add(route);
            }
            finally
            {
                _lock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Removes all keys and values from 
        /// the collection
        /// </summary>
        public virtual void Clear()
        {
            try
            {
                _lock.AcquireWriterLock(-1);

                _indexedRoutes.Clear();
                _nonIndexedRoutes.Clear();
            }
            finally
            {
                _lock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Removes the specified <see cref="RouteBase"/> from the collection
        /// </summary>
        /// <param name="item">An element to remove</param>
        public virtual void Remove(RouteBase item)
        {
            try
            {
                _lock.AcquireWriterLock(-1);
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
            finally
            {
                _lock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Removes the <see cref="RouteBase"/> with the specified index 
        /// from the collection
        /// </summary>
        /// <param name="item">An index of the element to remove</param>
        public void Remove(int index)
        {
            RouteBase item = _nonIndexedRoutes[index];
            Remove(item);
        }

        /// <summary>
        /// Removes the <see cref="RouteBase"/> with 
        /// the specified key from the collection
        /// </summary>
        /// <param name="key">The key of the element to remove</param>
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
        /// <param name="context">The current <see cref="HttpContext"/> object</param>
        public virtual RouteData GetRouteData(HttpContextBase context)
        {
            Precondition.Require(context, Error.ArgumentNull("context"));
            Precondition.Require(context.Request, Error.ArgumentNull("request"));

            try
            {
                _lock.AcquireReaderLock(-1);

                foreach (RouteBase r in _nonIndexedRoutes)
                {
                    RouteData data = r.GetRouteData(context);
                    if (data != null)
                        return data;
                }
            }
            finally
            {
                _lock.ReleaseReaderLock();
            }

            return null;
        }

        /// <summary>
        /// Gets the appropriate <see cref="VirtualPathData"/> for 
        /// the provided <paramref name="values"/>, 
        /// or <value>null</value>, if no matching route found.
        /// </summary>
        /// <param name="context">The context of the current request</param>
        /// <param name="value">The <see cref="ValueDictionary"/> 
        /// containing the route parameter values</param>
        public virtual VirtualPathData GetVirtualPath(RequestContext context, ValueDictionary values)
        {
            Precondition.Require(context, Error.ArgumentNull("context"));
            Precondition.Require(values, Error.ArgumentNull("values"));

            try
            {
                _lock.AcquireReaderLock(-1);

                foreach (RouteBase route in _nonIndexedRoutes)
                {
                    VirtualPathData vp = route.GetVirtualPath(context, values);
                    if (vp != null)
                        return vp;
                }
            }
            finally
            {
                _lock.ReleaseReaderLock();
            }
            return null;
        }

        /// <summary>
        /// Gets the appropriate <see cref="VirtualPathData"/> for 
        /// the provided route <paramref name="key"/> and <paramref name="values"/>, 
        /// or <value>null</value>, if no matching route found.
        /// </summary>
        /// <param name="context">The context of the current request</param>
        /// <param name="key">The key of the route to use</param>
        /// <param name="value">The <see cref="ValueDictionary"/> 
        /// containing the route parameter values</param>
        public virtual VirtualPathData GetVirtualPath(RequestContext context, 
            string key, ValueDictionary values)
        {
            Precondition.Require(context, Error.ArgumentNull("context"));
            Precondition.Require(values, Error.ArgumentNull("values"));

            if (String.IsNullOrEmpty(key))
                return GetVirtualPath(context, values);

            RouteBase route = null;
            bool hasRoute = false;

            try
            {
                _lock.AcquireReaderLock(-1);
                hasRoute = _indexedRoutes.TryGetValue(key, out route);
            }
            finally
            {
                _lock.ReleaseReaderLock();
            }

            if (hasRoute)
            {
                VirtualPathData vp = route.GetVirtualPath(context, values);
                if (vp != null)
                    return vp;
            }

            return null;
        }
        #endregion
    }
}

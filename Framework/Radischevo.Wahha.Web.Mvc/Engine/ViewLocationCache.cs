using System;
using System.Collections.Generic;
using System.Threading;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public sealed class ViewLocationCache : IViewLocationCache
    {
        #region Instance Fields
        private ReaderWriterLock _rwLock = new ReaderWriterLock();
        private Dictionary<string, string> _cache;
        #endregion

        #region Constructors
        public ViewLocationCache()
        {
            _cache = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);
        }
        #endregion

        #region Instance Properties
        public int Count
        {
            get
            {
                return _cache.Count;
            }
        }
        #endregion

        #region Instance Methods
        public string GetVirtualPath(string cacheKey)
        {
            try
            {
                _rwLock.AcquireReaderLock(Timeout.Infinite);
                if (_cache.ContainsKey(cacheKey))
                    return _cache[cacheKey];
            }
            finally
            {
                _rwLock.ReleaseReaderLock();
            }
            return null;
        }

        public void SetVirtualPath(string cacheKey, string virtualPath)
        {
            try
            {
                _rwLock.AcquireWriterLock(Timeout.Infinite);
                _cache[cacheKey] = virtualPath;
            }
            finally
            {
                _rwLock.ReleaseWriterLock();
            }
        }
        #endregion
    }

    internal sealed class NullViewLocationCache : IViewLocationCache
    {
        #region Static Fields
        private static NullViewLocationCache _instance = new NullViewLocationCache();
        #endregion

        #region Constructors
        private NullViewLocationCache() 
        { }
        #endregion

        #region Static Properties
        public static NullViewLocationCache Instance
        {
            get
            {
                return _instance;
            }
        }
        #endregion

        #region Instance Methods
        public string GetVirtualPath(string cacheKey)
        {
            return null;
        }

        public void SetVirtualPath(string cacheKey, string virtualPath)
        {   }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Threading;

namespace Radischevo.Wahha.Core
{
    public abstract class ReaderWriterCache<TKey, TValue>
    {
        #region Instance Fields
        private Dictionary<TKey, TValue> _cache;
        private ReaderWriterLock _lock;
        #endregion

        #region Constructors
        protected ReaderWriterCache()
            : this(null)
        {   }

        protected ReaderWriterCache(IEqualityComparer<TKey> comparer)
        {
            _lock = new ReaderWriterLock();
            _cache = new Dictionary<TKey, TValue>(comparer);
        }
        #endregion

        #region Instance Properties
        protected Dictionary<TKey, TValue> Cache
        {
            get
            {
                return _cache;
            }
        }
        #endregion

        #region Instance Methods
        protected TValue GetOrCreate(TKey key, Func<TValue> creator)
        {
            TValue value;

            try
            {
                _lock.AcquireReaderLock(-1);

                if (_cache.TryGetValue(key, out value))
                    return value;

                LockCookie cookie = _lock.UpgradeToWriterLock(-1);
                try
                {
                    value = creator();
                    _cache[key] = value;
                }
                finally
                {
                    _lock.DowngradeFromWriterLock(ref cookie);
                }
            }
            finally
            {
                _lock.ReleaseReaderLock();
            }
            return value;
        }

        public void Remove(TKey key)
        {
            try 
            { 
                _lock.AcquireWriterLock(-1);
                _cache.Remove(key);
            }
            finally
            {
                _lock.ReleaseWriterLock();
            }
        }
        #endregion
    }
}

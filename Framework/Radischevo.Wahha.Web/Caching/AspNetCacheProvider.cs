using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data.Caching;

namespace Radischevo.Wahha.Web.Caching
{
    public class AspNetCacheProvider : ITaggedCacheProvider
	{
		#region Nested Types
		private sealed class CacheLockDictionary : ReaderWriterCache<string, object>
		{
			#region Constructors
			public CacheLockDictionary()
				: base()
			{
			}
			#endregion

			#region Instance Methods
			public object Get(string key)
			{
				return base.GetOrCreate(key, () => new object());
			}
			#endregion
		}
		#endregion

		#region Constants
		private const string _tagKeyPrefix = "__SYS-CACHE-TAG:";
        #endregion

        #region Static Fields
        private static readonly CacheLockDictionary _locks 
            = new CacheLockDictionary();
        #endregion

        #region Constructors
        public AspNetCacheProvider()
        {
        }
        #endregion

        #region Instance Properties
        protected Cache Cache
        {
            get
            {
                return HttpRuntime.Cache;
            }
        }
        #endregion

        #region Static Methods
        private static object AcquireLock(string key)
        {
            return _locks.Get(key);
        }

        private static void ReleaseLock(string key)
        {
            _locks.Remove(key);
        }

        private static string CreateTagStoreKey(string tag)
        {
            return _tagKeyPrefix + (tag.ToUpperInvariant());
        }

        private static CacheDependency CreateTagDependency(
			Cache cache, IEnumerable<string> tags)
        {
            if (tags == null || !tags.Any())
                return null;

			long version = DateTime.Now.ToFileTimeUtc();
			foreach (string tag in tags)
			{
                cache.Add(CreateTagStoreKey(tag), version, null,
                    DateTime.MaxValue, Cache.NoSlidingExpiration,
                    CacheItemPriority.NotRemovable, null);
            }
            return new CacheDependency(null, tags.Select(s =>
                CreateTagStoreKey(s)).ToArray());
        }
        #endregion

        #region Instance Methods
        void ICacheProvider.Init(IValueSet settings)
        {
            // nothing to initialize here
        }

		public void Invalidate(IEnumerable<string> tags)
        {
            if (tags == null)
                return;

			long version = DateTime.Now.ToFileTimeUtc();
			foreach (string tag in tags)
			{
				Cache.Insert(CreateTagStoreKey(tag), version, null,
					DateTime.MaxValue, Cache.NoSlidingExpiration,
					CacheItemPriority.NotRemovable, null);
			}
        }

        public T Get<T>(string key)
        {
            Precondition.Require(key, () => Error.ArgumentNull("key"));
            return Converter.ChangeType<T>(Cache.Get(key));
        }

        public T Get<T>(string key, CacheItemSelector<T> selector, 
			DateTime expiration)
        {
            return Get<T>(key, selector, expiration, null);
        }

		public T Get<T>(string key, CacheItemSelector<T> selector,
			Func<T, DateTime> expiration) 
		{
			return Get<T>(key, selector, expiration, null);
		}

		public T Get<T>(string key, CacheItemSelector<T> selector,
			DateTime expiration, IEnumerable<string> tags) 
		{
			return Get<T>(key, selector, _ => expiration, _ => tags);
		}

        public T Get<T>(string key, CacheItemSelector<T> selector,
			Func<T, DateTime> expiration, Func<T, IEnumerable<string>> tags)
        {
            Precondition.Require(key, () => Error.ArgumentNull("key"));
			Precondition.Require(expiration, () => Error.ArgumentNull("expiration"));
            Precondition.Require(selector, () => Error.ArgumentNull("selector"));

            object value;
            if ((value = Cache.Get(key)) == null)
            {
				try
				{
					lock (AcquireLock(key))
					{
						if ((value = Cache.Get(key)) == null)
						{
							T result = selector();

							if (!ReferenceEquals(null, result))
							{
								value = result;
								Cache.Insert(key, value, 
									CreateTagDependency(Cache, (tags == null) ? null : tags(result)),
									expiration(result), Cache.NoSlidingExpiration,
									CacheItemPriority.Normal, null);
							}
						}
					}
				}
				finally
				{
					ReleaseLock(key);
				}
            }
            return Converter.ChangeType<T>(value);
        }

        public bool Add<T>(string key, T value, DateTime expiration)
        {
            return Add<T>(key, value, expiration, null);
        }

        public bool Add<T>(string key, T value, DateTime expiration,
			IEnumerable<string> tags)
        {
            Precondition.Require(key, () => Error.ArgumentNull("key"));
			Precondition.Require(value, () => Error.ArgumentNull("value"));

            object obj = Cache.Add(key, value, CreateTagDependency(Cache, tags),
                expiration, Cache.NoSlidingExpiration,
                CacheItemPriority.Normal, null);

            return (Object.ReferenceEquals(obj, null));
        }

        public void Insert<T>(string key, T value, DateTime expiration)
        {
            Insert<T>(key, value, expiration, null);
        }

        public void Insert<T>(string key, T value, DateTime expiration,
			IEnumerable<string> tags)
        {
            Precondition.Require(key, () => Error.ArgumentNull("key"));
			Precondition.Require(value, () => Error.ArgumentNull("value"));

            Cache.Insert(key, value, CreateTagDependency(Cache, tags),
                expiration, Cache.NoSlidingExpiration,
                CacheItemPriority.Normal, null);
        }

        public void Remove(string key)
        {
            Precondition.Require(key, () => Error.ArgumentNull("key"));
            Cache.Remove(key);
        }
        #endregion
    }
}

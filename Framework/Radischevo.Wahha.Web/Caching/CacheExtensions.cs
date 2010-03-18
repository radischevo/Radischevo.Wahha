using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Caching
{
    public static class CacheExtensions
    {
        #region Constants
        private const string _tagKeyPrefix = "__SYS-CACHE-TAG:";
        #endregion

        #region Static Fields
        private static readonly CacheLockDictionary _locks =
            new CacheLockDictionary();
        private static TimeSpan _defaultTimeout = TimeSpan.FromHours(1);
        #endregion

        #region Static Methods
        private static string CreateTagStoreKey(string tag)
        {
            return _tagKeyPrefix + (tag.ToUpperInvariant());
        }

        public static CacheDependency CreateTagDependency(
            this Cache cache, string[] tags)
        {
            if (tags == null || tags.Length < 1)
                return null;

            long version = DateTime.UtcNow.Ticks;
            for (int i = 0; i < tags.Length; ++i)
            {
                cache.Add(CreateTagStoreKey(tags[i]), version, null,
                    DateTime.MaxValue, Cache.NoSlidingExpiration,
                    CacheItemPriority.NotRemovable, null);
            }
            return new CacheDependency(null, tags.Select(s => 
                CreateTagStoreKey(s)).ToArray());
        }

        private static object AcquireLock(string key)
        {
            return _locks.Get(key);
        }

        private static void ReleaseLock(string key)
        {
            _locks.Remove(key);
        }
        #endregion

        #region Extension Methods
        public static void Invalidate(this Cache cache, params string[] tags)
        {
            if (tags == null)
                return;

            long version = DateTime.UtcNow.Ticks;
            for (int i = 0; i < tags.Length; ++i)
            {
                cache.Insert(CreateTagStoreKey(tags[i]), version, null,
                    DateTime.MaxValue, Cache.NoSlidingExpiration, 
                    CacheItemPriority.NotRemovable, null);
            }
        }

        /// <summary>
        /// Gets the typed value from the web application cache.
        /// </summary>
        /// <typeparam name="T">The type of the object to return.</typeparam>
        /// <param name="cache"></param>
        /// <param name="key">The key the object is stored with.</param>
        public static T Get<T>(this Cache cache, string key)
        {
            return Converter.ChangeType<T>(cache.Get(key));
        }

        public static T Get<T>(this Cache cache, string key, 
            Func<T> selector, TimeSpan timeout, params string[] tags)
        {
            return Get<T>(cache, key, selector, DateTime.UtcNow.Add(timeout), tags);
        }

        public static T Get<T>(this Cache cache, string key, 
            Func<T> selector, DateTime expiration, params string[] tags)
        {
			Precondition.Require(key, () => Error.ArgumentNull("key"));

			object value;
			if ((value = cache.Get(key)) == null)
            {
				try
				{
					lock (AcquireLock(key))
					{
						if ((value = cache.Get(key)) == null)
						{
							value = selector();
							if (value != null)
							{
								cache.Insert(key, value, CreateTagDependency(cache, tags),
									expiration, Cache.NoSlidingExpiration,
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

        public static T Get<T>(this Cache cache, string key, Func<T> selector)
        {
            return Get<T>(cache, key, selector, _defaultTimeout, null);
        }

        public static T Get<T>(this Cache cache, string key,  
            Func<T> selector, params string[] tags)
        {
            return Get<T>(cache, key, selector, _defaultTimeout, tags);
        }

        public static T Get<T>(this Cache cache, string key,
            Func<T> selector, DateTime expiration)
        {
            return Get<T>(cache, key, selector, expiration, null);
        }

        public static T Get<T>(this Cache cache, string key, 
            Func<T> selector, TimeSpan timeout)
        {
            return Get<T>(cache, key, selector, timeout, null);
        }

        public static void Insert<T>(this Cache cache, string key, 
            T value, TimeSpan timeout, params string[] tags)
        {
			Precondition.Require(key, () => Error.ArgumentNull("key"));
			Precondition.Require(value, () => Error.ArgumentNull("value"));

            cache.Insert(key, value, CreateTagDependency(cache, tags),
                DateTime.Now.Add(timeout), Cache.NoSlidingExpiration,
                CacheItemPriority.Normal, null);
        }

        public static void Insert<T>(this Cache cache, string key,
            T value)
        {
            Insert<T>(cache, key, value, _defaultTimeout, null);
        }

        public static void Insert<T>(this Cache cache, string key,
            T value, params string[] tags)
        {
            Insert<T>(cache, key, value, _defaultTimeout, tags);
        }

        public static void Insert<T>(this Cache cache, string key,
            T value, TimeSpan timeout)
        {
            Insert<T>(cache, key, value, timeout, null);
        }

        public static bool Add<T>(this Cache cache, string key,
            T value, TimeSpan timeout, params string[] tags)
        {
			Precondition.Require(key, () => Error.ArgumentNull("key"));
			Precondition.Require(value, () => Error.ArgumentNull("value"));

            object obj = cache.Add(key, value, CreateTagDependency(cache, tags),
                DateTime.Now.Add(timeout), Cache.NoSlidingExpiration,
                CacheItemPriority.Normal, null);

            return Object.ReferenceEquals(obj, null);
        }

        public static bool Add<T>(this Cache cache, string key,
            T value)
        {
            return Add<T>(cache, key, value, _defaultTimeout, null);
        }

        public static bool Add<T>(this Cache cache, string key,
            T value, params string[] tags)
        {
            return Add<T>(cache, key, value, _defaultTimeout, tags);
        }

        public static bool Add<T>(this Cache cache, string key,
            T value, TimeSpan timeout)
        {
            return Add<T>(cache, key, value, timeout, null);
        }
        #endregion
    }
}

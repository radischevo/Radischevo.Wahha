using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Caching
{
    public sealed class CacheProvider : ITaggedCacheProvider
    {
        #region Constants
        private const int _timeout = 60;
        #endregion

        #region Static Fields
        private static CacheProvider _instance;
        private static object _lock = new object();
        #endregion

        #region Instance Fields
        private ITaggedCacheProvider _provider;
        private TimeSpan _defaultTimeout;
        #endregion

        #region Constructors
        private CacheProvider(ICacheProvider provider, int timeout)
        {
            _provider = new TaggedCacheWrapper(provider);
            _defaultTimeout = TimeSpan.FromMinutes(timeout);
        }
        #endregion

        #region Static Properties
        public static CacheProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            Configuration.CacheSettings settings =
                                Configuration.Configuration.Instance.Caching;

                            _instance = Create(settings.ProviderType, settings.Settings);
                        }
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region Static Methods
        internal static bool IsProvider(Type type)
        {
            Precondition.Require(type, Error.ArgumentNull("type"));

            if (type.IsAbstract || type.IsInterface ||
                type.IsGenericTypeDefinition || type.IsGenericType ||
                type.GetInterface(typeof(ICacheProvider).Name) == null ||
                type == typeof(CacheProvider))
                return false;

            if (type.GetConstructor(Type.EmptyTypes) == null)
                return false;

            return true;
        }

        public static CacheProvider Create(Type providerType, IValueSet settings)
        {
            Precondition.Require(settings, Error.ArgumentNull("settings"));

            if (!IsProvider(providerType))
                throw Error.IncompatibleCacheProviderType(providerType);

            ICacheProvider provider = (ICacheProvider)Activator.CreateInstance(providerType);
            provider.Init(settings);

            return new CacheProvider(provider, settings.GetValue<int>("timeout", _timeout));
        }

        public static CacheProvider Create<TProvider>(IValueSet settings)
            where TProvider : ICacheProvider, new()
        {
            ICacheProvider provider = new TProvider();
            provider.Init(settings);

            return new CacheProvider(provider, settings.GetValue<int>("timeout", _timeout));
        }
        #endregion

        #region Instance Methods
        #region Gets
        /// <summary>
        /// Gets the typed value from the web application cache.
        /// </summary>
        /// <typeparam name="T">The type of the object to return.</typeparam>
        /// <param name="cache"></param>
        /// <param name="key">The key the object is stored with.</param>
        public T Get<T>(string key)
        {
            Precondition.Require(key, Error.ArgumentNull("key"));
            return _provider.Get<T>(key);
        }

		public T Get<T>(string key, CacheItemSelector<T> selector)
		{
			return Get<T>(key, selector, _defaultTimeout, null);
		}

        public T Get<T>(string key, CacheItemSelector<T> selector,
            TimeSpan timeout)
        {
            return Get<T>(key, selector, timeout);
        }

        public T Get<T>(string key, CacheItemSelector<T> selector,
            DateTime expiration)
        {
            return Get<T>(key, selector, expiration, null);
        }

        public T Get<T>(string key, CacheItemSelector<T> selector,
			IEnumerable<string> tags)
        {
            return Get<T>(key, selector, _defaultTimeout, tags);
        }

        public T Get<T>(string key, CacheItemSelector<T> selector,
			TimeSpan timeout, IEnumerable<string> tags)
        {
            return Get<T>(key, selector, timeout, tags);
        }

        public T Get<T>(string key, CacheItemSelector<T> selector,
			DateTime expiration, IEnumerable<string> tags)
        {
			return _provider.Get<T>(key, selector, expiration, tags);
        }
        #endregion

        #region Adds
        public bool Add<T>(string key, T value)
        {
            return Add<T>(key, value, _defaultTimeout);
        }

        public bool Add<T>(string key, T value, TimeSpan timeout)
        {
            return Add<T>(key, value, DateTime.UtcNow.Add(timeout));
        }

        public bool Add<T>(string key, T value, DateTime expiration)
        {
            return Add<T>(key, value, expiration, null);
        }

		public bool Add<T>(string key, T value, IEnumerable<string> tags)
        {
            return Add<T>(key, value, _defaultTimeout, tags);
        }

        public bool Add<T>(string key, T value, TimeSpan timeout,
		    IEnumerable<string> tags)
        {
            return Add<T>(key, value, DateTime.UtcNow.Add(timeout), tags);
        }

        public bool Add<T>(string key, T value, DateTime expiration,
			IEnumerable<string> tags)
        {
            Precondition.Require(key, Error.ArgumentNull("key"));
            return _provider.Add<T>(key, value, expiration, tags);
        }
        #endregion

        #region Inserts
        public void Insert<T>(string key, T value)
        {
            Insert<T>(key, value, _defaultTimeout);
        }

        public void Insert<T>(string key, T value, TimeSpan timeout)
        {
            Insert<T>(key, value, DateTime.UtcNow.Add(timeout));
        }

        public void Insert<T>(string key, T value, DateTime expiration)
        {
            Insert<T>(key, value, expiration, null);
        }

		public void Insert<T>(string key, T value, IEnumerable<string> tags)
        {
            Insert<T>(key, value, _defaultTimeout, tags);
        }

        public void Insert<T>(string key, T value, TimeSpan timeout,
			IEnumerable<string> tags)
        {
            Insert<T>(key, value, DateTime.UtcNow.Add(timeout), tags);
        }

		public void Insert<T>(string key, T value, DateTime expiration,
			IEnumerable<string> tags)
		{
			Precondition.Require(key, Error.ArgumentNull("key"));
			_provider.Insert<T>(key, value, expiration, tags);
		}
        #endregion

		public void Remove(string key)
        {
            Precondition.Require(key, Error.ArgumentNull("key"));
            _provider.Remove(key);
        }

		public void Invalidate(IEnumerable<string> tags)
		{
			_provider.Invalidate(tags);
		}

        void ICacheProvider.Init(IValueSet settings)
        {   }
        #endregion
	}
}

using System;
using System.Collections.Generic;
using System.Globalization;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data.Configurations;

namespace Radischevo.Wahha.Data.Caching
{
    public sealed class CacheProvider : ITaggedCacheProvider
    {
        #region Static Fields
        private static CacheProvider _instance;
        private static object _lock = new object();
        #endregion

        #region Instance Fields
        private ITaggedCacheProvider _provider;
        #endregion

        #region Constructors
        private CacheProvider(ICacheProvider provider)
        {
            _provider = new TaggedCacheWrapper(provider);
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
                            CacheSettings settings = Configuration.Instance.Caching;
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
            Precondition.Require(type, () => Error.ArgumentNull("type"));

            if (!typeof(ICacheProvider).IsAssignableFrom(type) ||
                type == typeof(CacheProvider))
                return false;

            return true;
        }

        public static CacheProvider Create(Type providerType, IValueSet settings)
        {
            Precondition.Require(settings, () => Error.ArgumentNull("settings"));

            if (!IsProvider(providerType))
                throw Error.IncompatibleCacheProviderType(providerType);

			ICacheProvider provider = (ICacheProvider)ServiceLocator.Instance.GetService(providerType);
            provider.Init(settings);

            return new CacheProvider(provider);
        }

        public static CacheProvider Create<TProvider>(IValueSet settings)
            where TProvider : ICacheProvider
        {
			ICacheProvider provider = ServiceLocator.Instance.GetService<TProvider>();
            provider.Init(settings);

			return new CacheProvider(provider);
        }
        #endregion

        #region Instance Methods
        #region Gets
        
        public T Get<T>(string key)
        {
            Precondition.Require(key, () => Error.ArgumentNull("key"));
            return _provider.Get<T>(key);
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
			return _provider.Get<T>(key, selector, expiration, tags);
        }

		public T Get<T>(string key, CacheItemSelector<T> selector,
			Func<T, DateTime> expiration, Func<T, IEnumerable<string>> tags) 
		{
			return _provider.Get<T>(key, selector, expiration, tags);
		}
        #endregion

        #region Adds
        public bool Add<T>(string key, T value, DateTime expiration)
        {
            return Add<T>(key, value, expiration, null);
        }

        public bool Add<T>(string key, T value, DateTime expiration,
			IEnumerable<string> tags)
        {
            Precondition.Require(key, () => Error.ArgumentNull("key"));
            return _provider.Add<T>(key, value, expiration, tags);
        }
        #endregion

        #region Inserts
        public void Insert<T>(string key, T value, DateTime expiration)
        {
            Insert<T>(key, value, expiration, null);
        }

		public void Insert<T>(string key, T value, DateTime expiration,
			IEnumerable<string> tags)
		{
			Precondition.Require(key, () => Error.ArgumentNull("key"));
			_provider.Insert<T>(key, value, expiration, tags);
		}
        #endregion

		public void Remove(string key)
        {
            Precondition.Require(key, () => Error.ArgumentNull("key"));
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

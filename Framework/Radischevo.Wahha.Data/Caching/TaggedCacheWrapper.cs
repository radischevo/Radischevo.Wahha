using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Caching
{
    internal class TaggedCacheWrapper : ITaggedCacheProvider
    {
        #region Instance Fields
        private ICacheProvider _provider;
        private bool _supportTags;
        #endregion

        #region Constructors
        internal TaggedCacheWrapper(ICacheProvider provider)
        {
            Precondition.Require(provider, () => Error.ArgumentNull("provider"));

            _provider = provider;
            _supportTags = (_provider is ITaggedCacheProvider);
        }
        #endregion

        #region Instance Properties
        private ITaggedCacheProvider TaggedProvider
        {
            get
            {
                if (!_supportTags)
                    throw Error.CachingProviderDoesNotSupportTags(_provider.GetType());

                return (ITaggedCacheProvider)_provider;
            }
        }
        #endregion

        #region Instance Methods
        public void Init(IValueSet settings)
        {
            _provider.Init(settings);
        }

		public void Invalidate(IEnumerable<string> tags)
        {
            TaggedProvider.Invalidate(tags);
        }

        public T Get<T>(string key)
        {
            return _provider.Get<T>(key);
        }

        public T Get<T>(string key, CacheItemSelector<T> selector, 
			DateTime expiration)
        {
            return _provider.Get<T>(key, selector, expiration);
        }

        public T Get<T>(string key, CacheItemSelector<T> selector,
			DateTime expiration, IEnumerable<string> tags)
        {
            if (tags == null)
                return _provider.Get<T>(key, selector, expiration);

            return TaggedProvider.Get<T>(key, selector, expiration, tags);
        }

        public bool Add<T>(string key, T value, DateTime expiration)
        {
            return _provider.Add<T>(key, value, expiration);
        }

        public bool Add<T>(string key, T value, DateTime expiration,
			IEnumerable<string> tags)
        {
            if (tags == null)
                return _provider.Add<T>(key, value, expiration);
            
            return TaggedProvider.Add<T>(key, value, expiration, tags);
        }

        public void Insert<T>(string key, T value, DateTime expiration)
        {
            _provider.Insert<T>(key, value, expiration);
        }

        public void Insert<T>(string key, T value, DateTime expiration,
			IEnumerable<string> tags)
        {
            if (tags == null)
                _provider.Insert<T>(key, value, expiration);
            else
                TaggedProvider.Insert<T>(key, value, expiration, tags);
        }

        public void Remove(string key)
        {
            _provider.Remove(key);
        }
        #endregion
    }
}

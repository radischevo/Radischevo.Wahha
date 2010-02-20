using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Caching
{
    public sealed class NullCacheProvider : ITaggedCacheProvider
    {
        #region Instance Methods
        public void Init(IValueSet settings)
        {
        }

		public void Invalidate(IEnumerable<string> tags)
        {
        }

        public T Get<T>(string key)
        {
            return default(T);
        }

		public T Get<T>(string key, CacheItemSelector<T> selector, 
			DateTime expiration)
		{
			return selector();
		}

		public T Get<T>(string key, CacheItemSelector<T> selector,
			DateTime expiration, IEnumerable<string> tags)
		{
			return selector();
		}

        public bool Add<T>(string key, T value, DateTime expiration)
        {
            return true;
        }

        public bool Add<T>(string key, T value, DateTime expiration,
			IEnumerable<string> tags)
        {
            return true;
        }

        public void Insert<T>(string key, T value, DateTime expiration)
        {
        }

        public void Insert<T>(string key, T value, DateTime expiration,
			IEnumerable<string> tags)
        {
        }

        public void Remove(string key)
        {
        }
        #endregion
    }
}

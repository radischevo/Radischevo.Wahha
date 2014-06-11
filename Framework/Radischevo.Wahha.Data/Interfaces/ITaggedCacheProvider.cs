using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Data.Caching
{
    public interface ITaggedCacheProvider : ICacheProvider
    {
        void Invalidate(IEnumerable<string> tags);

        T Get<T>(string key, CacheItemSelector<T> selector,
			DateTime expiration, IEnumerable<string> tags);

		T Get<T>(string key, CacheItemSelector<T> selector,
			Func<T, DateTime> expiration, Func<T, IEnumerable<string>> tags);

		bool Add<T>(string key, T value, DateTime expiration, IEnumerable<string> tags);

		void Insert<T>(string key, T value, DateTime expiration, IEnumerable<string> tags);
    }
}

using System;

namespace Radischevo.Wahha.Data.Caching
{
    public interface ITaggedCacheProvider : ICacheProvider
    {
        void Invalidate(string[] tags);

        T Get<T>(string key, CacheItemSelector<T> selector, 
			DateTime expiration, string[] tags);

        bool Add<T>(string key, T value, DateTime expiration, string[] tags);

        void Insert<T>(string key, T value, DateTime expiration, string[] tags);
    }
}

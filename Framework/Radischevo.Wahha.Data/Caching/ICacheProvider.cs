using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Caching
{
    public interface ICacheProvider
    {
        void Init(IValueSet settings);

        T Get<T>(string key);

        T Get<T>(string key, CacheItemSelector<T> selector, DateTime expiration);

        bool Add<T>(string key, T value, DateTime expiration);

        void Insert<T>(string key, T value, DateTime expiration);

        void Remove(string key);
    }
}

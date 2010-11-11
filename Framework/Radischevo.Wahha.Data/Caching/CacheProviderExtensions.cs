using System;

namespace Radischevo.Wahha.Data.Caching
{
	public static class CacheProviderExtensions
	{
		#region Extension Methods
		public static void Invalidate(this ITaggedCacheProvider provider, params string[] tags)
		{
			provider.Invalidate(tags);
		}

		public static T Get<T>(this ITaggedCacheProvider provider, 
			string key, CacheItemSelector<T> selector,
			DateTime expiration, params string[] tags)
		{
			return provider.Get<T>(key, selector, expiration, tags);
		}

		public static bool Add<T>(this ITaggedCacheProvider provider, 
			string key, T value, DateTime expiration, params string[] tags)
		{
			return provider.Add<T>(key, value, expiration, tags);
		}

		public static void Insert<T>(this ITaggedCacheProvider provider, string key, 
			T value, DateTime expiration, params string[] tags)
		{
			provider.Insert<T>(key, value, expiration, tags);
		}
		#endregion
	}
}

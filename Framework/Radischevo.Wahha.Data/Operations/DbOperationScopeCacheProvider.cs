using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data.Caching;

namespace Radischevo.Wahha.Data
{
	internal sealed class DbOperationScopeCacheProvider : ITaggedCacheProvider
	{
		#region Instance Fields
		private bool _disposed;
		private bool _allowDirtyWrites;
		private readonly ITaggedCacheProvider _provider;
		private readonly HashSet<string> _invalidations;
		private readonly List<Action<ITaggedCacheProvider>> _deferredActions;
		#endregion

		#region Constructors
		public DbOperationScopeCacheProvider(ITaggedCacheProvider provider)
		{
			Precondition.Require(provider, () => Error.ArgumentNull("provider"));

			_provider = provider;
			_invalidations = new HashSet<string>();
			_deferredActions = new List<Action<ITaggedCacheProvider>>();
		}
		#endregion

		#region Instance Properties
		public bool AllowDirtyWrites
		{
			get
			{
				return _allowDirtyWrites;
			}
			set
			{
				_allowDirtyWrites = value;
			}
		}
		#endregion

		#region Instance Methods
		private void Init(IValueSet settings)
		{
		}

		public void Commit()
		{
			try
			{
				foreach (Action<ITaggedCacheProvider> action in _deferredActions)
					action(_provider);
			}
			finally
			{
				Rollback();
			}
		}

		public void Rollback()
		{
			_deferredActions.Clear();
			_invalidations.Clear();
		}

		public void Invalidate(IEnumerable<string> tags)
		{
			Precondition.Require(tags, () => Error.ArgumentNull("tags"));
			if (_allowDirtyWrites)
			{
				_provider.Invalidate(tags);
			}
			else
			{
				_deferredActions.Add(a => a.Invalidate(tags));
				_invalidations.UnionWith(tags);
			}
		}

		public T Get<T>(string key)
		{
			return _provider.Get<T>(key);
		}

		public T Get<T>(string key, CacheItemSelector<T> selector, DateTime expiration)
		{
			return _provider.Get<T>(key, selector, expiration);
		}

		public T Get<T>(string key, CacheItemSelector<T> selector, 
			DateTime expiration, IEnumerable<string> tags)
		{
			Precondition.Require(selector, () => 
				Error.ArgumentNull("selector"));

			if (tags != null && _invalidations.Overlaps(tags))
				return selector();

			return _provider.Get(key, selector, expiration, tags);
		}

		public bool Add<T>(string key, T value, DateTime expiration)
		{
			return Add(key, value, expiration, null);
		}

		public bool Add<T>(string key, T value, DateTime expiration, IEnumerable<string> tags)
		{
			if (_allowDirtyWrites)
				return _provider.Add(key, value, expiration, tags);
			 
			_deferredActions.Add(a => a.Add(key, value, expiration, tags));
			return true;
		}

		public void Insert<T>(string key, T value, DateTime expiration)
		{
			Insert(key, value, expiration, null);
		}

		public void Insert<T>(string key, T value, DateTime expiration, IEnumerable<string> tags)
		{
			if (_allowDirtyWrites)
				_provider.Insert(key, value, expiration, tags);
			else
				_deferredActions.Add(a => a.Insert(key, value, expiration, tags));
		}

		public void Remove(string key)
		{
			if (_allowDirtyWrites)
				_provider.Remove(key);
			else
				_deferredActions.Add(a => a.Remove(key));
		}
		#endregion

		#region Interface Implementation
		void ICacheProvider.Init(IValueSet settings)
		{
			Init(settings);
		}
		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Data;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data.Caching;
using Radischevo.Wahha.Data.Configurations;

namespace Radischevo.Wahha.Data
{
	/// <summary>
	/// Represents the controlled scope which can 
	/// be used to execute several database operations 
	/// within a single transaction.
	/// </summary>
	public class DbOperationScope : DbOperationContext, IDisposable
	{
		#region Nested Types
		private sealed class ScopedCacheProvider : ITaggedCacheProvider
		{
			#region Instance Fields
			private bool _disposed;
			private bool _allowDirtyReads;
			private readonly ITaggedCacheProvider _provider;
			private readonly HashSet<string> _invalidations;
			private readonly List<Action<ITaggedCacheProvider>> _deferredActions;
			#endregion

			#region Constructors
			public ScopedCacheProvider(ITaggedCacheProvider provider)
			{
				Precondition.Require(provider, () => Error.ArgumentNull("provider"));

				_provider = provider;
				_invalidations = new HashSet<string>();
				_deferredActions = new List<Action<ITaggedCacheProvider>>();
			}
			#endregion

			#region Instance Properties
			public bool AllowDirtyReads
			{
				get
				{
					return _allowDirtyReads;
				}
				set
				{
					_allowDirtyReads = value;
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

				_deferredActions.Add(a => a.Invalidate(tags));
				_invalidations.UnionWith(tags);
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
				Precondition.Require(selector, () => Error.ArgumentNull("selector"));
				if (_allowDirtyReads || (tags != null && _invalidations.Overlaps(tags)))
				{
					T value = selector();
					if (!_allowDirtyReads)
						_deferredActions.Add(a => a.Insert(key, value, expiration, tags));

					return value;
				}
				return _provider.Get(key, selector, expiration, tags);
			}

			public bool Add<T>(string key, T value, DateTime expiration)
			{
				return Add(key, value, expiration, null);
			}

			public bool Add<T>(string key, T value, DateTime expiration, IEnumerable<string> tags)
			{
				if (!_allowDirtyReads)
					_deferredActions.Add(a => a.Add(key, value, expiration, tags));

				return true;
			}

			public void Insert<T>(string key, T value, DateTime expiration)
			{
				Insert(key, value, expiration, null);
			}

			public void Insert<T>(string key, T value, DateTime expiration, IEnumerable<string> tags)
			{
				if (!_allowDirtyReads)
					_deferredActions.Add(a => a.Insert(key, value, expiration, tags));
			}

			public void Remove(string key)
			{
				_deferredActions.Add(a => a.Remove(key));
			}
			#endregion

			#region Interface Implementations
			void ICacheProvider.Init(IValueSet settings)
			{
				Init(settings);
			}
			#endregion
		}
		#endregion

		#region Instance Fields
		private readonly IDbDataProvider _provider;
		private readonly ScopedCacheProvider _cache;
		private readonly bool _hasOwnedContext;
		private IsolationLevel _isolationLevel;
		private bool _transactionActive;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Radischevo.Wahha.Data.DbOperationScope"/> class.
		/// </summary>
		public DbOperationScope()
			: this(CreateDefaultProvider())
		{
			_hasOwnedContext = true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Radischevo.Wahha.Data.DbOperationScope"/> class.
		/// </summary>
		/// <param name="factory">The <see cref="Radischevo.Wahha.Data.IDbDataProviderFactory"/> 
		/// used to create an instance of <see cref="Radischevo.Wahha.Data.IDbDataProvider"/>.</param>
		public DbOperationScope(IDbDataProviderFactory factory)
			: this(CreateProvider(factory))
		{
			_hasOwnedContext = true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Radischevo.Wahha.Data.DbOperationScope"/> class.
		/// </summary>
		/// <param name="provider">The <see cref="Radischevo.Wahha.Data.IDbDataProvider"/> 
		/// used to perform database queries.</param>
		public DbOperationScope(IDbDataProvider provider)
			: base()
		{
			Precondition.Require(provider, () => 
				Error.ArgumentNull("provider"));

			_provider = provider;
			_cache = new ScopedCacheProvider(CacheProvider.Instance);
			_isolationLevel = IsolationLevel.ReadCommitted;
		}
		#endregion

		#region Instance Properties
		/// <summary>
		/// Gets the <see cref="Radischevo.Wahha.Data.IDbDataProvider"/> 
		/// used to perform database queries.
		/// </summary>
		public override IDbDataProvider Provider
		{
			get
			{
				return _provider;
			}
		}

		/// <summary>
		/// Gets the <see cref="Radischevo.Wahha.Data.Caching.ITaggedCacheProvider"/> 
		/// used to access the application cache within a scope.
		/// </summary>
		public override ITaggedCacheProvider Cache
		{
			get
			{
				// cache scope can only be accessed within a transaction.
				EnsureTransaction(); 
				return _cache;
			}
		}

		/// <summary>
		/// Gets or sets the transaction isolation level.
		/// </summary>
		public IsolationLevel IsolationLevel
		{
			get
			{
				return _isolationLevel;
			}
			set
			{
				if (_isolationLevel != value)
				{
					if (_transactionActive)
						throw Error.IsolationLevelCannotBeModified();

					_isolationLevel = value;
					_cache.AllowDirtyReads = (value == IsolationLevel.ReadUncommitted);
				}
			}
		}
		#endregion

		#region Static Methods
		private static IDbDataProvider CreateDefaultProvider()
		{
			return CreateProvider(Configuration.Instance.Database.Factory);
		}

		private static IDbDataProvider CreateProvider(IDbDataProviderFactory factory)
		{
			Precondition.Require(factory, () => Error.ArgumentNull("factory"));
			return factory.CreateProvider();
		}
		#endregion

		#region Instance Methods
		private void EnsureTransaction()
		{
			if (!_transactionActive)
			{
				_provider.BeginTransaction(_isolationLevel);
				_transactionActive = true;
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or
		/// resetting unmanaged resources.
		/// </summary>
		/// <param name="disposing">A value indicating whether 
		/// the disposal is called explicitly.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_hasOwnedContext)
				{
					IDisposable disposable = (_provider as IDisposable);
					if (disposable != null)
						disposable.Dispose();
				}
			}
		}

		/// <summary>
		/// Executes the specified operation within the scope.
		/// </summary>
		/// <param name="operation">The operation to execute.</param>
		public void Execute(IDbOperation operation)
		{
			Precondition.Require(operation, () =>
				Error.ArgumentNull("operation"));

			try
			{
				EnsureTransaction();
				operation.Execute(this);
			}
			catch (Exception)
			{
				Rollback();
				throw;
			}
		}

		/// <summary>
		/// Executes the specified operation within the scope 
		/// and return its result.
		/// </summary>
		/// <typeparam name="TResult">The type of the operation result.</typeparam>
		/// <param name="operation">The operation to execute.</param>
		public TResult Execute<TResult>(IDbOperation<TResult> operation)
		{
			Precondition.Require(operation, () =>
				Error.ArgumentNull("operation"));

			try
			{
				EnsureTransaction();
				return operation.Execute(this);
			}
			catch (Exception)
			{
				Rollback();
				throw;
			}
		}

		/// <summary>
		/// Accepts all changes that have 
		/// been made since the last commit.
		/// </summary>
		public void Commit()
		{
			if (_transactionActive)
			{
				_provider.Commit();
				_cache.Commit();

				_transactionActive = false;
			}
		}

		/// <summary>
		/// Discards all changes that have 
		/// been made since the last commit.
		/// </summary>
		public void Rollback()
		{
			if (_transactionActive)
			{
				_provider.Rollback();
				_cache.Rollback();

				_transactionActive = false;
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with 
		/// freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
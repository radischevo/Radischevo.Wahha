using System;
using System.Data;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data.Configurations;
using Radischevo.Wahha.Data.Caching;

namespace Radischevo.Wahha.Data
{
	/// <summary>
	/// Represents the controlled scope which can 
	/// be used to execute several database operations 
	/// within a single transaction.
	/// </summary>
	public class DbOperationScope : DbOperationContext, IDisposable
	{
		#region Instance Fields
		private IDbDataProvider _dataProvider;
		private DbOperationScopeCacheProvider _cacheProvider;
		private IsolationLevel _isolationLevel;
		private bool _transactionActive;
		private bool _hasOwnedContext;
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
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Radischevo.Wahha.Data.DbOperationScope"/> class.
		/// </summary>
		/// <param name="dataProvider">The <see cref="Radischevo.Wahha.Data.IDbDataProvider"/> 
		/// used to perform database queries.</param>
		public DbOperationScope(IDbDataProvider dataProvider)
			: base()
		{
			Precondition.Require(dataProvider, () => 
				Error.ArgumentNull("dataProvider"));

			_dataProvider = dataProvider;
			_cacheProvider = new DbOperationScopeCacheProvider(Caching.CacheProvider.Instance);
			_isolationLevel = IsolationLevel.ReadCommitted;
		}
		#endregion

		#region Instance Properties
		/// <summary>
		/// Gets the <see cref="Radischevo.Wahha.Data.IDbDataProvider"/> 
		/// used to perform database queries.
		/// </summary>
		public override IDbDataProvider DataProvider
		{
			get
			{
				return _dataProvider;
			}
		}

		/// <summary>
		/// Gets the <see cref="Radischevo.Wahha.Data.Caching.IScopedCacheProvider"/> 
		/// used to access the application cache within a scope.
		/// </summary>
		public override ITaggedCacheProvider CacheProvider
		{
			get
			{
				return _cacheProvider;
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
						throw Error.CouldNotSetIsolationLevelAfterInitialize();

					_isolationLevel = value;
					_cacheProvider.AllowDirtyWrites = (value == IsolationLevel.ReadUncommitted);
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
				_dataProvider.BeginTransaction(_isolationLevel);
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
			Commit();

			if (disposing && _hasOwnedContext)
			{
				IDisposable disposable = (_dataProvider as IDisposable);
				if (disposable != null)
					disposable.Dispose();
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
		/// Explicitly commits the current transaction.
		/// </summary>
		public void Commit()
		{
			if (_transactionActive)
			{
				_dataProvider.Commit();
				_cacheProvider.Commit();

				_transactionActive = false;
			}
		}

		/// <summary>
		/// Rolls back the current transaction.
		/// </summary>
		public void Rollback()
		{
			if (_transactionActive)
			{
				_dataProvider.Rollback();
				_cacheProvider.Rollback();

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

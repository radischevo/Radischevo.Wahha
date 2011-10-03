using System;
using System.Data;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data.Configurations;

namespace Radischevo.Wahha.Data
{
	/// <summary>
	/// Represents the controlled scope which can 
	/// be used to execute several database operations 
	/// within a single transaction.
	/// </summary>
	public class DbOperationScope : IDisposable
	{
		#region Instance Fields
		private IDbDataProvider _provider;
		private IsolationLevel _isolationLevel;
		private bool _initialized;
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
		/// <param name="provider">The <see cref="Radischevo.Wahha.Data.IDbDataProviderFactory"/> 
		/// used to create an instance of <see cref="Radischevo.Wahha.Data.IDbDataProvider"/>.</param>
		public DbOperationScope(IDbDataProviderFactory factory)
			: this(CreateProvider(factory))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Radischevo.Wahha.Data.DbOperationScope"/> class.
		/// </summary>
		/// <param name="provider">The <see cref="Radischevo.Wahha.Data.IDbDataProvider"/> 
		/// used to perform database queries.</param>
		public DbOperationScope(IDbDataProvider provider)
		{
			Precondition.Require(provider, () => 
				Error.ArgumentNull("provider"));

			_provider = provider;
			_isolationLevel = IsolationLevel.ReadCommitted;
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

		#region Instance Properties
		/// <summary>
		/// Gets the <see cref="Radischevo.Wahha.Data.IDbDataProvider"/> 
		/// used to perform database queries.
		/// </summary>
		protected IDbDataProvider Provider
		{
			get
			{
				return _provider;
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
				if (_initialized)
					throw Error.CouldNotSetIsolationLevelAfterInitialize();

				_isolationLevel = value;
			}
		}
		#endregion

		#region Instance Methods
		private void EnsureInitialized()
		{
			if (!_initialized)
			{
				_initialized = true;
				Initialize();
			}
		}

		/// <summary>
		/// Perform initialization tasks before the first 
		/// operation is executed, i.e. starts new transaction 
		/// if necessary.
		/// </summary>
		protected virtual void Initialize()
		{
			Provider.BeginTransaction(IsolationLevel);
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
				IDisposable disposable = (_provider as IDisposable);
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
				EnsureInitialized();
				operation.Execute(Provider);
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
				EnsureInitialized();
				return operation.Execute(Provider);
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
			Provider.Commit();
		}

		/// <summary>
		/// Rolls back the current transaction.
		/// </summary>
		public void Rollback()
		{
			Provider.Rollback();
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

using System;
using System.Data;

using Radischevo.Wahha.Core;

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
		private bool _useTransaction;
		private bool _hasOwnedContext;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a nes instance of the <see cref="DbOperationScope"/> class.
		/// </summary>
		public DbOperationScope()
			: this(DbDataProvider.Create())
		{
			_hasOwnedContext = true;
		}

		/// <summary>
		/// Initializes a nes instance of the <see cref="DbOperationScope"/> class.
		/// </summary>
		/// <param name="dataProvider">The <see cref="Radischevo.Wahha.Data.IDbDataProvider"/> 
		/// used to perform database queries.</param>
		public DbOperationScope(IDbDataProvider provider)
		{
			Precondition.Require(provider, () => 
				Error.ArgumentNull("provider"));

			_provider = provider;
			_useTransaction = true;
			_isolationLevel = IsolationLevel.ReadCommitted;
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
		/// Gets or sets the value indicating whether 
		/// the transaction will be used.
		/// </summary>
		public bool UseTransaction
		{
			get
			{
				return _useTransaction;
			}
			set
			{
				_useTransaction = value;
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
			if (UseTransaction)
				_provider.BeginTransaction(IsolationLevel);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or
		/// resetting unmanaged resources.
		/// </summary>
		/// <param name="disposing">A value indicating whether 
		/// the disposal is called explicitly.</param>
		protected virtual void Dispose(bool disposing)
		{
			_provider.Commit();

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

			EnsureInitialized();
			operation.Execute(Provider);
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

			EnsureInitialized();
			return operation.Execute(Provider);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or
		/// resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}

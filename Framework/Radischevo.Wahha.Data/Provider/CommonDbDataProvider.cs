using System;
using System.Data;
using System.Data.Common;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Provider
{
	public abstract class CommonDbDataProvider : IDbDataProvider
	{
		#region Instance Fields
		private IDbConnection _connection;
		private IDbTransaction _transaction;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="CommonDbDataProvider"/> class.
		/// </summary>
		/// <param name="connectionString">The database connection string.</param>
		protected CommonDbDataProvider(string connectionString)
        {
			Precondition.Defined(connectionString, () => 
				Error.ConnectionStringNotInitialized());

			_connection = CreateConnection(connectionString);
        }
		#endregion

		#region Instance Properties
		/// <summary>
		/// Gets a value indicating whether 
		/// the underlying database connection 
		/// is open.
		/// </summary>
		protected bool IsOpen
		{
			get
			{
				if (_connection == null)
					return false;

				return (_connection.State
					!= ConnectionState.Closed);
			}
		}

		/// <summary>
		/// Gets a value indicating whether 
		/// the database transaction is active.
		/// </summary>
		protected bool HasTransaction
		{
			get
			{
				return (_transaction != null &&
					_transaction.Connection != null);
			}
		}

		/// <summary>
		/// Gets the underlying database connection.
		/// </summary>
		protected IDbConnection Connection
		{
			get
			{
				return _connection;
			}
		}

		/// <summary>
		/// Gets the underlying database transaction.
		/// </summary>
		protected IDbTransaction Transaction
		{
			get
			{
				return _transaction;
			}
		}

		/// <summary>
		/// Gets the current state of the connection.
		/// </summary>
		public ConnectionState State
		{
			get
			{
				if (_connection == null)
					return ConnectionState.Closed;

				return _connection.State;
			}
		}
		#endregion

		#region Create Methods
		/// <summary>
		/// When overridden in a derived class, creates 
		/// an <see cref="System.Data.IDbConnection"/> instance 
		/// which is supported by the current provider.
		/// </summary>
		/// <param name="connectionString">The database connection string.</param>
		protected abstract IDbConnection CreateConnection(string connectionString);

		/// <summary>
		/// When overridden in a derived class, creates 
		/// an <see cref="System.Data.IDbCommand"/> instance 
		/// which is supported by the current provider.
		/// </summary>
		public virtual IDbCommand CreateCommand()
		{
			IDbCommand command = _connection.CreateCommand();
			command.Connection = _connection;

			return command;
		}

		/// <summary>
		/// When overridden in a derived class, creates 
		/// an <see cref="System.Data.IDbDataAdapter"/> instance 
		/// which is supported by the current provider.
		/// </summary>
		public abstract IDbDataAdapter CreateDataAdapter();
		#endregion

		#region Utility Methods
		/// <summary>
		/// When overridden in a derived class, validates the 
		/// provided <see cref="System.Data.IDbCommand"/>.
		/// </summary>
		/// <param name="command">The <see cref="IDbCommand"/> instance to validate.</param>
		protected virtual bool ValidateCommand(IDbCommand command)
		{
			return true;
		}

		/// <summary>
		/// Returns a value indicating whether the current 
		/// database connection is available.
		/// </summary>
		public virtual bool ValidateConnection()
		{
			try
			{
				_connection.Open();
				_connection.Close();

				return true;
			}
			catch
			{
				return false;
			}
		}
		#endregion

		#region Manipulation Methods
		/// <summary>
		/// When overridden in a derived class opens the underlying 
		/// database connection.
		/// </summary>
		protected virtual void Open()
		{
			if (_connection.State == ConnectionState.Closed)
				_connection.Open();
		}

		/// <summary>
		/// When overridden in a derived class closes the underlying 
		/// database connection and rolls back the current transaction.
		/// </summary>
		protected void CloseOnError()
		{
			if (_connection.State != ConnectionState.Closed)
			{
				Rollback();
				_connection.Close();
			}
		}

		/// <summary>
		/// Closes the underlying database connection discarding 
		/// all changes have been made.
		/// </summary>
		public void Close()
		{
			if (_connection.State != ConnectionState.Closed)
			{
				Rollback();
				_connection.Close();
			}
		}

		/// <summary>
		/// Commits the current transaction.
		/// </summary>
		public void Commit()
		{
			if (HasTransaction)
			{
				_transaction.Commit();
				_transaction.Dispose();
				_transaction = null;
			}
		}

		/// <summary>
		/// Discards changes have been made 
		/// since the last commit.
		/// </summary>
		public void Rollback()
		{
			if (HasTransaction)
			{
				_transaction.Rollback();
				_transaction.Dispose();
				_transaction = null;
			}
		}

		/// <summary>
		/// Starts the database transaction with the specified isolation level.
		/// </summary>
		public virtual void BeginTransaction(IsolationLevel isolation)
		{
			if (!HasTransaction)
			{
				if (_connection.State == ConnectionState.Closed)
					_connection.Open();

				_transaction = _connection.BeginTransaction(isolation);
			}
		}

		/// <summary>
		/// Executes the <paramref name="command"/> against the current data source 
		/// and converts the result using the specified <paramref name="converter"/>.
		/// </summary>
		/// <param name="command">The <see cref="System.Data.IDbCommand"/> 
		/// to execute.</param>
		/// <param name="converter">The action to perform convertion with.</param>
		/// <typeparam name="TR">The type of the returning value.</typeparam>
		public virtual TR Execute<TR>(IDbCommand command, Func<IDbCommand, TR> converter)
		{
			Precondition.Require(command, () => Error.ArgumentNull("command"));
			Precondition.Require(ValidateCommand(command), () => 
				Error.UnsupportedCommandType(command.GetType()));
			Precondition.Require(converter, () => Error.ArgumentNull("converter"));

			TR result = default(TR);
			try
			{
				Open();

				command.Connection = _connection;
				command.Transaction = _transaction;

				result = converter(command);
			}
			catch (DbException)
			{
				CloseOnError();
				throw;
			}
			return result;
		}
		#endregion

		#region Dispose Methods
		/// <summary>
		/// Performs application-defined tasks associated with 
		/// freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
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
				Close();
				_connection.Dispose();
			}
		}
		#endregion
	}
}

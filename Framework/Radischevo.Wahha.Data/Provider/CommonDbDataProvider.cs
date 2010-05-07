using System;
using System.Data;
using System.Data.Common;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Provider
{
	public abstract class CommonDbDataProvider : IDbDataProvider
	{
		#region Instance Fields
		private IDbTransaction _transaction;
		private IDbConnection _connection;
		private bool _useTransaction;
		#endregion

		#region Constructors
		protected CommonDbDataProvider()
        {
        }
		#endregion

		#region Instance Properties
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

		protected bool HasTransaction
		{
			get
			{
				return (_transaction != null &&
					_transaction.Connection != null);
			}
		}

		protected IDbConnection Connection
		{
			get
			{
				return _connection;
			}
		}

		protected IDbTransaction Transaction
		{
			get
			{
				return _transaction;
			}
		}

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
		protected abstract IDbConnection CreateConnection(string connectionString);

		public virtual IDbCommand CreateCommand()
		{
			IDbCommand command = _connection.CreateCommand();
			command.Connection = _connection;

			return command;
		}

		public abstract IDbDataAdapter CreateDataAdapter();
		#endregion

		#region Utility Methods
		protected virtual bool ValidateCommand(IDbCommand command)
		{
			return true;
		}

		public virtual void Initialize(string connectionString, bool useTransaction)
		{
			if (String.IsNullOrEmpty(connectionString))
				throw Error.ConnectionStringNotInitialized();

			_connection = CreateConnection(connectionString);
			_useTransaction = useTransaction;
		}
		#endregion

		#region Manipulation Methods
		protected virtual void Open()
		{
			if (_connection.State == ConnectionState.Closed)
			{
				_connection.Open();

				if (_useTransaction && !HasTransaction)
					_transaction = _connection.BeginTransaction();
			}
		}

		protected void CloseOnError()
		{
			if (_connection.State != ConnectionState.Closed)
			{
				Rollback();
				_connection.Close();
			}
		}

		public void Close()
		{
			if (_connection.State != ConnectionState.Closed)
			{
				Commit();
				_connection.Close();
			}
		}

		public void Commit()
		{
			if (HasTransaction)
				_transaction.Commit();

			_transaction = null;
		}

		public void Rollback()
		{
			if (HasTransaction)
				_transaction.Rollback();

			_transaction = null;
		}

		public virtual void BeginTransaction(IsolationLevel isolation)
		{
			if (!HasTransaction)
			{
				_useTransaction = true;
				if (_connection.State == ConnectionState.Closed)
					_connection.Open();

				_transaction = _connection.BeginTransaction(isolation);
			}
		}

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
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			Close();

			if (_transaction != null)
				_transaction.Dispose();

			_connection.Dispose();
		}
		#endregion
	}
}

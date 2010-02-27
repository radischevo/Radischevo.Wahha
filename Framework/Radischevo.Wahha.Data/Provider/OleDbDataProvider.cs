using System;
using System.Data;
using System.Data.OleDb;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Provider
{
    /// <summary>
    /// Provides methods to access 
    /// Microsoft Jet databases
    /// </summary>
    public class OleDbDataProvider : IDbDataProvider
    {
        #region Instance Fields
        private OleDbTransaction _transaction;
        private OleDbConnection _connection;
        private bool _useTransaction;
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets or sets the value indicating 
        /// whether the transaction will be used
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
        /// Gets the current connection state
        /// </summary>
        public ConnectionState State
        {
            get
            {
                return _connection.State;
            }
        }

        /// <summary>
        /// Gets the value indicating 
        /// whether the current connection 
        /// is opened
        /// </summary>
        private bool IsOpened
        {
            get
            {
                if (_connection == null)
                    return false;

                return (_connection.State
                    != ConnectionState.Closed);
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of 
        /// the <see cref="OleDbDataProvider"/> class
        /// </summary>
        public OleDbDataProvider()
        {
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Initializes an instance of 
        /// the <see cref="OleDbDataProvider"/> class
        /// </summary>
        /// <param name="connectionString">The database connection string</param>
        /// <param name="useTransaction">True to use a transaction</param>
        public virtual void Initialize(string connectionString, bool useTransaction)
        {
            if (String.IsNullOrEmpty(connectionString))
                throw Error.ConnectionStringNotInitialized();

            _connection = new OleDbConnection(connectionString);
            _useTransaction = useTransaction;
        }

        /// <summary>
        /// Starts the database transaction with the specified isolation level
        /// </summary>
        public virtual void BeginTransaction(IsolationLevel isolation)
        {
            if (_transaction == null)
                _transaction = _connection.BeginTransaction(isolation);
        }

        /// <summary>
        /// Commits the current transaction (if present)
        /// </summary>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.Exception"></exception>
        public virtual void Commit()
        {
            if (_transaction != null)
                _transaction.Commit();

			_transaction = null;
        }

        /// <summary>
        /// Rolls back the current transaction
        /// </summary>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        public virtual void Rollback()
        {
            if (_transaction != null)
                _transaction.Rollback();

			_transaction = null;
        }

        /// <summary>
        /// Closes the underlying database connection.
        /// </summary>
        public virtual void Close()
        {
            if (_connection.State != ConnectionState.Closed)
                Commit();

            _connection.Close();
        }

        /// <summary>
        /// Ensures the current database connection is opened
        /// </summary>
        /// <exception cref="System.Data.OleDb.OleDbException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        private void Open()
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();

                if (_useTransaction)
                    _transaction = _connection.BeginTransaction();
            }
        }

        private void CloseOnError()
        {
            if (_connection.State != ConnectionState.Closed)
                Rollback();

            _connection.Close();
        }

        /// <summary>
        /// Executes the <paramref name="command"/> against the current 
        /// <see cref="Radischevo.Wahha.Data.IDataProvider"/> and converts the 
        /// result using the specified <paramref name="converter"/>.
        /// </summary>
        /// <param name="command">The <see cref="System.Data.IDbCommand"/> 
        /// command to execute.</param>
        /// <param name="converter">The action to perform convertion with.</param>
        /// <typeparam name="TR">The type of the returning value.</typeparam>
        public virtual TR Execute<TR>(IDbCommand command, Func<IDbCommand, TR> converter)
        {
            Precondition.Require(command, () => Error.ArgumentNull("command"));
            Precondition.Require(command is OleDbCommand,
				() => Error.UnsupportedCommandType(typeof(OleDbCommand), command.GetType()));
            Precondition.Require(converter, () => Error.ArgumentNull("converter"));

            TR result = default(TR);
            try
            {
                Open();

                command.Connection = _connection;
                command.Transaction = _transaction;

                result = converter(command);
            }
            catch (OleDbException)
            {
                CloseOnError();
                throw;
            }            
            return result;
        }

        /// <summary>
        /// Creates a <see cref="System.Data.IDbDataAdapter"/> instance, 
        /// which is supported by the current provider.
        /// </summary>
        public IDbDataAdapter CreateDataAdapter()
        {
            return new OleDbDataAdapter();
        }

        /// <summary>
        /// Creates an <see cref="IDbCommand"/> instance, 
        /// which is supported by the current provider
        /// </summary>
        public IDbCommand CreateCommand()
        {
            return new OleDbCommand() { Connection = _connection };
        }

        /// <summary>
        /// Performs tasks associated 
        /// with freeing, releasing or 
        /// resetting unmanaged resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs tasks associated 
        /// with freeing, releasing or 
        /// resetting unmanaged resources
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            Close();
            
            if(_transaction != null)
                _transaction.Dispose();

            _connection.Dispose();
        }
        #endregion

        #region IDataProvider Members
        /// <summary>
        /// Generates the <see cref="System.NotSupportedException"/> exception
        /// </summary>
        /// <exception cref="System.NotSupportedException"></exception>
        void IDbDataProvider.Rollback(string savePointName)
        {
            throw Error.NotSupported();
        }

        /// <summary>
        /// Generates the <see cref="System.NotSupportedException"/> exception
        /// </summary>
        /// <exception cref="System.NotSupportedException"></exception>
        void IDbDataProvider.CreateSavePoint(string savePointName)
        {
            throw Error.NotSupported();
        }
        #endregion
    }
}

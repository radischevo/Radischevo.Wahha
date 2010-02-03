using System;
using System.Data;

namespace Radischevo.Wahha.Data
{
    /// <summary>
    /// Defines the database connection 
    /// adapter interface.
    /// </summary>
    public interface IDbDataProvider : IDisposable
    {
        #region Instance Properties
        /// <summary>
        /// Gets or sets the value indicating 
        /// whether the transaction will be used
        /// </summary>
        bool UseTransaction
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the current connection state
        /// </summary>
        ConnectionState State
        {
            get;
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Initializes an instance of 
        /// the <see cref="IDataProvider" />
        /// </summary>
        /// <param name="connectionString">The database connection string</param>
        /// <param name="useTransaction">True to use a transaction</param>
        void Initialize(string connectionString, bool useTransaction);

        /// <summary>
        /// Starts the database transaction with the specified isolation level
        /// </summary>
        void BeginTransaction(IsolationLevel isolation);

        /// <summary>
        /// Commits the current transaction (if present)
        /// </summary>
        void Commit();

        /// <summary>
        /// Rolls back the current transaction
        /// </summary>
        void Rollback();

        /// <summary>
        /// Closes the underlying database connection.
        /// </summary>
        void Close();

        /// <summary>
        /// Restores the current transaction to 
        /// the <paramref name="savePointName"/> savepoint
        /// </summary>
        /// <param name="savePointName">The savepoint name</param>
        void Rollback(string savePointName);

        /// <summary>
        /// Creates a savepoint with name <paramref name="savePointName"/>
        /// </summary>
        /// <param name="savePointName">The savepoint name</param>
        void CreateSavePoint(string savePointName);

		/// <summary>
		/// Executes the <paramref name="command"/> against the current data source 
		/// and converts the result using the specified <paramref name="converter"/>.
		/// </summary>
		/// <param name="command">The <see cref="System.Data.IDbCommand"/> 
		/// to execute.</param>
		/// <param name="converter">The action to perform convertion with.</param>
		/// <typeparam name="TR">The type of the returning value.</typeparam>
		TR Execute<TR>(IDbCommand command, Func<IDbCommand, TR> converter);

        /// <summary>
        /// Creates a <see cref="System.Data.IDbDataAdapter"/> instance, 
        /// which is supported by the current provider.
        /// </summary>
        IDbDataAdapter CreateDataAdapter();

		/// <summary>
		/// Creates an <see cref="IDbCommand"/> instance, 
		/// which is supported by the current provider.
		/// </summary>
		IDbCommand CreateCommand();
        #endregion
    }
}

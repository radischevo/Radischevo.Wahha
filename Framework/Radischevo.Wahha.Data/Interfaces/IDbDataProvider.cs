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
        /// Gets the current connection state
        /// </summary>
        ConnectionState State
        {
            get;
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Starts the database transaction with the specified isolation level.
        /// </summary>
        void BeginTransaction(IsolationLevel isolation);

        /// <summary>
        /// Accepts all changes have been made 
		/// within the current transaction.
        /// </summary>
        void Commit();

		/// <summary>
		/// Discards all changes have been made 
		/// within the current transaction.
		/// </summary>
        void Rollback();

		/// <summary>
		/// Closes the underlying database connection discarding 
		/// all changes have been made.
		/// </summary>
        void Close();

		/// <summary>
		/// Returns a value indicating whether 
		/// the current database connection is available.
		/// </summary>
		bool ValidateConnection();

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

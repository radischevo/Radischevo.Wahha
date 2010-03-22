using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data.Configurations;

namespace Radischevo.Wahha.Data
{
    /// <summary>
    /// Defines the generic database 
    /// connection provider.
    /// </summary>
    public sealed class DbDataProvider : IDbDataProvider
    {
        #region Instance Fields
        private IDbDataProvider _provider;
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
                return _provider.UseTransaction;
            }
            set
            {
                _provider.UseTransaction = value;
            }
        }

        /// <summary>
        /// Gets the current connection state
        /// </summary>
        public ConnectionState State
        {
            get
            {
                return _provider.State;
            }
        }
        #endregion

        #region Constructors
        private DbDataProvider(IDbDataProvider provider, 
            string connectionString, bool useTransaction)
        {
            Precondition.Require(provider, () => Error.ArgumentNull("provider"));
            Precondition.Defined(connectionString,
				() => Error.ConnectionStringNotInitialized());

            _provider = provider;
            _provider.Initialize(connectionString, useTransaction);
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Creates a new instance of 
        /// the <see cref="IDbDataProvider"/> class, which is registered 
        /// in the provider name-to-type mapping table with a specified name.
        /// </summary>
        /// <param name="providerName">The mapped name of the provider.</param>
        public static DbDataProvider Resolve(string providerName)
        {
            return Resolve(providerName, false);
        }

        /// <summary>
        /// Creates a new instance of 
        /// the <see cref="IDbDataProvider"/> class, which is registered 
        /// in the provider name-to-type mapping table with a specified name.
        /// </summary>
        /// <param name="providerName">The mapped name of the provider.</param>
        /// <param name="useTransaction">True to use a transaction.</param>
        public static DbDataProvider Resolve(string providerName, bool useTransaction)
        {
            if (Configuration.Instance.ConnectionStrings.Count < 1)
                throw Error.ConnectionStringNotConfigured();

            return Resolve(providerName, Configuration.Instance.ConnectionStrings[0]);
        }

        /// <summary>
        /// Creates a new instance of 
        /// the <see cref="IDbDataProvider"/> class, which is registered 
        /// in the provider name-to-type mapping table with a specified name.
        /// </summary>
        /// <param name="providerName">The mapped name of the provider.</param>
        /// <param name="connectionString">The database connection string.</param>
        public static DbDataProvider Resolve(string providerName,
            string connectionString)
        {
            return Resolve(providerName, connectionString, false);
        }

        /// <summary>
        /// Creates a new instance of 
        /// the <see cref="IDbDataProvider"/> class, which is registered 
        /// in the provider name-to-type mapping table with a specified name.
        /// </summary>
        /// <param name="providerName">The mapped name of the provider.</param>
        /// <param name="connectionString">The database connection string.</param>
        /// <param name="useTransaction">True to use a transaction.</param>
        public static DbDataProvider Resolve(string providerName, 
            string connectionString, bool useTransaction)
        {
            IDbDataProvider provider = Configuration.Instance
                .Providers.Factory.CreateProvider(providerName);
            return new DbDataProvider(provider, connectionString, useTransaction);
        }

        /// <summary>
        /// Creates a new instance of 
        /// the <see cref="IDbDataProvider"/> class.
        /// </summary>
        public static DbDataProvider Create()
        {
            return Create(false);
        }

        /// <summary>
        /// Creates a new instance of 
        /// the <see cref="IDbDataProvider"/> class.
        /// </summary>
        /// <param name="useTransaction">True to use a transaction.</param>
        public static DbDataProvider Create(bool useTransaction)
        {
            if (Configuration.Instance.ConnectionStrings.Count < 1)
                throw Error.ConnectionStringNotConfigured();

            return Create(Configuration.Instance.ConnectionStrings[0], false);
        }

        /// <summary>
        /// Creates a new instance of 
        /// the <see cref="IDbDataProvider"/> class.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        public static DbDataProvider Create(string connectionString)
        {
            return Create(connectionString, false);
        }

        /// <summary>
        /// Creates a new instance of 
        /// the <see cref="IDbDataProvider"/> class.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        /// <param name="useTransaction">True to use a transaction.</param>
        public static DbDataProvider Create(string connectionString, bool useTransaction)
        {
            Type type = Configuration.Instance
                .Providers.Mappings.Default;
            Precondition.Require(type, () => Error.ProviderNotConfigured());

            IDbDataProvider provider = Configuration.Instance
                .Providers.Factory.CreateProvider(type);
            return new DbDataProvider(provider, connectionString, useTransaction);
        }

        /// <summary>
        /// Creates a new instance of 
        /// the specified <see cref="IDbDataProvider"/> class.
        /// </summary>
        public static DbDataProvider Create<TProvider>()
            where TProvider : IDbDataProvider
        {
            return Create<TProvider>(false);
        }

        /// <summary>
        /// Creates a new instance of 
        /// the specified <see cref="IDbDataProvider"/> class.
        /// </summary>
        /// <param name="useTransaction">True to use a transaction.</param>
        public static DbDataProvider Create<TProvider>(bool useTransaction)
            where TProvider : IDbDataProvider
        {
            if (Configuration.Instance.ConnectionStrings.Count < 1)
                throw Error.ConnectionStringNotConfigured();

            return Create<TProvider>(
                Configuration.Instance.ConnectionStrings[0], useTransaction);
        }

        /// <summary>
        /// Creates a new instance of 
        /// the specified <see cref="IDbDataProvider"/> class.
        /// </summary>
        /// <param name="connectionString">The database connection string</param>
        public static DbDataProvider Create<TProvider>(string connectionString)
            where TProvider : IDbDataProvider
        {
            return Create<TProvider>(connectionString, false);
        }

        /// <summary>
        /// Creates a new instance of 
        /// the specified <see cref="IDbDataProvider"/> class.
        /// </summary>
        /// <param name="connectionString">The database connection string</param>
        /// <param name="useTransaction">True to use a transaction</param>
        public static DbDataProvider Create<TProvider>(
            string connectionString, bool useTransaction)
            where TProvider : IDbDataProvider
        {
            IDbDataProvider provider = Configuration.Instance
                .Providers.Factory.CreateProvider(typeof(TProvider));

            return new DbDataProvider(provider, connectionString, useTransaction);
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Starts the database transaction
        /// </summary>
        public void BeginTransaction()
        {
            _provider.BeginTransaction(IsolationLevel.Unspecified);
        }

        /// <summary>
        /// Starts the database transaction with the specified isolation level
        /// </summary>
        public void BeginTransaction(IsolationLevel isolation)
        {
            _provider.BeginTransaction(isolation);
        }

        /// <summary>
        /// Commits the current transaction (if present)
        /// </summary>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.Exception"></exception>
        public void Commit()
        {
            _provider.Commit();
        }

        /// <summary>
        /// Rolls back the current transaction
        /// </summary>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        public void Rollback()
        {
            _provider.Rollback();
        }

        /// <summary>
        /// Closes the underlying database connection.
        /// </summary>
        public void Close()
        {
            _provider.Close();
        }

        /// <summary>
        /// Restores the current transaction to 
        /// the <paramref name="savePointName"/> savepoint
        /// </summary>
        /// <param name="savePointName">The savepoint name</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.NotSupportedException"></exception>
        public void Rollback(string savePointName)
        {
            _provider.Rollback(savePointName);
        }

        /// <summary>
        /// Creates a savepoint with name <paramref name="savePointName"/>
        /// </summary>
        /// <param name="savePointName">The savepoint name</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.NotSupportedException"></exception>
        public void CreateSavePoint(string savePointName)
        {
            _provider.CreateSavePoint(savePointName);
        }

        /// <summary>
        /// Creates a <see cref="System.Data.IDbDataAdapter"/> instance, 
        /// which is supported by the current provider.
        /// </summary>
        public IDbDataAdapter CreateDataAdapter()
        {
            return _provider.CreateDataAdapter();
        }

        /// <summary>
        /// Creates an <see cref="IDbCommand"/> instance, 
        /// which is supported by the current provider
        /// </summary>
        public IDbCommand CreateCommand()
        {
            return _provider.CreateCommand();
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

        private void Dispose(bool disposing)
        {
            Configuration.Instance
                .Providers.Factory.DisposeProvider(_provider);
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Checks whether the <paramref name="connection"/> 
        /// is available
        /// </summary>
        /// <param name="connection">A database connection object instance</param>
        public static bool CheckConnection(IDbConnection connection)
        {
            if (connection == null)
                return false;

            bool exists = false;
            try
            {
                connection.Open();
                connection.Close();
                exists = true;
            }
            catch
            {
                exists = false;
            }
            return exists;
        }
        #endregion

        #region IDataProvider Members
        void IDbDataProvider.Initialize(string connectionString, bool useTransaction)
        {
            if (_provider == null)
                return;

            _provider.Initialize(connectionString, useTransaction);
        }

        TR IDbDataProvider.Execute<TR>(IDbCommand command, Func<IDbCommand, TR> converter)
        {
			return _provider.Execute<TR>(command, converter);
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
    /// <summary>
    /// Acts as a wrapper for the 
    /// <see cref="System.Data.IDbCommand"/> object 
    /// for executing various queries.
    /// </summary>
    public class DbCommandResult : IHideObjectMembers
    {
        #region Instance Fields
		private IDbDataProvider _provider;
        private DbCommandDescriptor _command;
		private CommandBehavior _behavior;
        #endregion

        #region Constructors
		/// <summary>
        /// Initializes a new instance of the 
		/// <see cref="Radischevo.Wahha.Data.DbCommandResult"/> class.
        /// </summary>
        /// <param name="provider">The <see cref="Radischevo.Wahha.Data.IDbDataProvider"/> 
        /// instance used to access the database.</param>
		public DbCommandResult(IDbDataProvider provider)
			: this(provider, null)
		{
		}

        /// <summary>
        /// Initializes a new instance of the 
		/// <see cref="Radischevo.Wahha.Data.DbCommandResult"/> class.
        /// </summary>
        /// <param name="provider">The <see cref="Radischevo.Wahha.Data.IDbDataProvider"/> 
        /// instance used to access the database.</param>
		/// <param name="command">The <see cref="Radischevo.Wahha.Data.DbCommandDescriptor"/>
		/// describing the command to execute.</param>
        public DbCommandResult(IDbDataProvider provider, DbCommandDescriptor command)
        {
            Precondition.Require(provider, () => Error.ArgumentNull("provider"));
            _provider = provider;
			_command = command;
			_behavior = CommandBehavior.Default;
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets the <see cref="Radischevo.Wahha.Data.IDbDataProvider"/> 
        /// instance used to access the database.
        /// </summary>
        public IDbDataProvider Provider
        {
            get
            {
                return _provider;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Radischevo.Wahha.Data.DbCommandDescriptor"/> 
		/// describing the command to execute.
        /// </summary>
        public DbCommandDescriptor Command
        {
            get
            {
                return _command;
            }
            set
            {
                Precondition.Require(value, () => Error.ArgumentNull("value"));
                _command = value;
            }
        }

		/// <summary>
		/// Gets the command behaviour.
		/// </summary>
		public CommandBehavior Behavior
		{
			get
			{
				return _behavior;
			}
			set
			{
				_behavior = value;
			}
		}
        #endregion

		#region Instance Methods
		/// <summary>
		/// Creates a new <see cref="System.Data.IDbCommand"/> to 
		/// use with the current data provider and initializes 
		/// its text, timeouts and parameters.
		/// </summary>
		protected virtual IDbCommand CreateCommand()
		{
			IDbCommand command = _provider.CreateCommand();
			if (_command == null)
				return command;

			command.CommandText = _command.Text;
			command.CommandTimeout = _command.Timeout;
			command.CommandType = _command.Type;

			foreach (DbParameterDescriptor pd in _command.Parameters)
			{
				IDbDataParameter parameter = command.CreateParameter();
				parameter.ParameterName = pd.Name;
				parameter.Direction = pd.Direction;
				parameter.Precision = pd.Precision;
				parameter.Scale = pd.Scale;
				parameter.Value = pd.Value;

				if (pd.Size.HasValue)
					parameter.Size = pd.Size.GetValueOrDefault();

				command.Parameters.Add(parameter);
			}
			return command;
		}

		/// <summary>
		/// Executes the command against the specified data source 
		/// and immediately closes the underlying connection.
		/// </summary>
		/// <typeparam name="TResult">The type of the execution result.</typeparam>
		/// <param name="converter">The action to perform conversion with.</param>
		protected TResult ExecuteOnce<TResult>(Func<IDbCommand, TResult> converter)
		{
			try
			{
				return Execute<TResult>(converter);
			}
			finally
			{
				if ((Behavior & CommandBehavior.CloseConnection) ==
					CommandBehavior.CloseConnection)
					Provider.Close();
			}
		}

		/// <summary>
		/// Executes the command against the specified data 
		/// source and transforms its results using the specified converter.
		/// </summary>
		/// <typeparam name="TResult">The type of the execution result.</typeparam>
		/// <param name="converter">The action to perform conversion with.</param>
		protected virtual TResult Execute<TResult>(Func<IDbCommand, TResult> converter)
		{
			using (IDbCommand command = CreateCommand())
			{
				return _provider.Execute(command, converter);
			}
		}

		/// <summary>
        /// Sets the command type property of the wrapper 
        /// command to CommandType.StoredProcedure and returns 
        /// the same object.
        /// </summary>
        public DbCommandResult ToProcedure()
        {
            Precondition.Require(_command, () => Error.CommandIsNotInitialized());
            _command.Type = CommandType.StoredProcedure;
			
            return this;
        }

		/// <summary>
		/// Sets the command behaviour, providing a description 
		/// of the results of the query and its effect on the database.
		/// </summary>
		/// <param name="behavior">The target behaviour to set.</param>
		public DbCommandResult Using(CommandBehavior behavior)
		{
			Behavior = behavior;
			return this;
		}

        /// <summary>
        /// Executes an SQL statement against the current 
        /// provider and returns a number of rows affected.
        /// </summary>
        public int AsNonQuery()
        {
            Precondition.Require(_command, () => Error.CommandIsNotInitialized());
            return ExecuteOnce<int>(c => c.ExecuteNonQuery());
        }

        /// <summary>
        /// Executes the query and returns the first column of the first row 
        /// in the resultset returned by the query. Extra columns and rows are ignored.
        /// </summary>
        public object AsScalar()
        {
            Precondition.Require(_command, () => Error.CommandIsNotInitialized());
			return ExecuteOnce<object>(c => c.ExecuteScalar());
        }

        /// <summary>
        /// Executes the query and converts the first column of the first row 
        /// in the resultset returned by the query to the 
        /// <typeparamref name="T"/> type. If no rows returned or field type 
        /// does not support the required conversion, a default value is returned.
        /// </summary>
        public T AsScalar<T>()
        {
            Precondition.Require(_command, () => Error.CommandIsNotInitialized());
			return ExecuteOnce<T>(c => Converter.ChangeType<T>(c.ExecuteScalar()));
        }

        /// <summary>
        /// Executes an SQL statement against the current provider 
        /// and builds a <see cref="System.Data.IDataReader"/>.
        /// </summary>
        public IDbDataReader AsDataReader()
        {
            Precondition.Require(_command, () => Error.CommandIsNotInitialized());
            return Execute<IDbDataReader>(DataReaderConverter);
        }

		/// <summary>
		/// Executes an SQL statement against the current provider 
		/// and builds a <see cref="System.Data.IDataReader"/>.
		/// </summary>
		public TResult AsDataReader<TResult>(Func<IDbDataReader, TResult> action)
		{
			Precondition.Require(_command, () => Error.CommandIsNotInitialized());
			return Execute<TResult>((command) => DataReaderConverter(command, action));
		}

        /// <summary>
        /// Executes an SQL statement against the current provider 
        /// and creates a <see cref="System.Data.DataSet"/> containing 
        /// the results of the query.
        /// </summary>
        public DataSet AsDataSet()
        {
            Precondition.Require(_command, () => Error.CommandIsNotInitialized());
			return ExecuteOnce<DataSet>(DataSetConverter);
        }

		/// <summary>
        /// Executes an SQL statement against the current provider 
        /// and builds a typed collection of elements using the 
        /// specified <paramref name="converter"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of a collection element.</typeparam>
        /// <param name="converter">A function used to convert the data row to 
        /// an instance of the specified type.</param>
        public IEnumerable<TEntity> AsEntitySet<TEntity>(Func<IDbDataRecord, TEntity> converter)
        {
            Precondition.Require(_command, () => Error.CommandIsNotInitialized());
			return ExecuteOnce((command) => DataReaderConverter(command, reader => 
				new ObjectReader<TEntity>(new DbQueryResultReader(reader), converter)));
        }

		/// <summary>
		/// Executes an SQL statement against the current provider 
		/// and builds a typed collection of elements using the 
		/// specified <paramref name="materializer"/>.
		/// </summary>
		/// <typeparam name="TEntity">The type of a collection element.</typeparam>
		/// <param name="materializer">A materialization service used to convert the data row to 
		/// an instance of the specified type.</param>
		public IEnumerable<TEntity> AsEntitySet<TEntity>(IDbMaterializer<TEntity> materializer)
		{
			Precondition.Require(_command, () => Error.CommandIsNotInitialized());
			Precondition.Require(materializer, () => Error.ArgumentNull("materializer"));

			return ExecuteOnce((command) => DataReaderConverter(command, reader =>
				new ObjectReader<TEntity>(new DbQueryResultReader(reader), materializer.Materialize)));
		}
        #endregion

        #region Cached Delegates
        private IDbDataReader DataReaderConverter(IDbCommand command)
        {
            return new DbDataReader(command.ExecuteReader(_behavior));
        }

		private TResult DataReaderConverter<TResult>(IDbCommand command, 
			Func<IDbDataReader, TResult> action)
		{
			using (IDbDataReader reader = new DbDataReader(command.ExecuteReader(_behavior)))
			{
				return action(reader);
			}
		}

        private DataSet DataSetConverter(IDbCommand command)
        {
            IDbDataAdapter adapter = _provider.CreateDataAdapter();
            try
            {
                DataSet ds = new DataSet();
                adapter.SelectCommand = command;
                adapter.Fill(ds);

                return ds;
            }
            finally
            {
                IDisposable d = (adapter as IDisposable);
                if (d != null)
                    d.Dispose();
            }
        }       
        #endregion
    }
}

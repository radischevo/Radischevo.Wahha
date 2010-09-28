using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public abstract class KeyedDbRepository<TEntity, TKey> :
		DbRepository<TEntity, TKey>
		where TEntity : class
	{
		#region Instance Fields
		private IDbMaterializer<TEntity> _materializer;
		private IDbDataProvider _dataProvider;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of the 
		/// <see cref="Radischevo.Wahha.Data.KeyedDbRepository{TKey,TEntity}"/> class.
		/// </summary>
		/// <param name="materializer">The <see cref="Radischevo.Wahha.Data.IDbMaterializer{TEntity}"/>
		/// used to transform database query results into objects.</param>
		protected KeyedDbRepository(IDbMaterializer<TEntity> materializer)
			: this(null, materializer)
		{
		}

		/// <summary>
		/// Creates a new instance of the 
		/// <see cref="Radischevo.Wahha.Data.KeyedDbRepository{TKey,TEntity}"/> class.
		/// </summary>
		/// <param name="dataProvider">The <see cref="Radischevo.Wahha.Data.IDbDataProvider"/> 
		/// used to perform database queries.</param>
		/// <param name="materializer">The <see cref="Radischevo.Wahha.Data.IDbMaterializer{TEntity}"/>
		/// used to transform database query results into objects.</param>
		protected KeyedDbRepository(IDbDataProvider dataProvider,
			IDbMaterializer<TEntity> materializer)
		{
			Precondition.Require(materializer, () => Error.ArgumentNull("materializer"));

			Dictionary<string, object> d = new Dictionary<string, object>();

			_dataProvider = dataProvider;
			_materializer = materializer;
		}
		#endregion

		#region Instance Properties
		/// <summary>
		/// Gets the initial size of the entity collections.
		/// </summary>
		protected virtual int SelectBufferSize
		{
			get
			{
				return 15;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="Radischevo.Wahha.Data.IDbMaterializer{TEntity}"/>
		/// used to transform database query results into objects.
		/// </summary>
		public IDbMaterializer<TEntity> Materializer
		{
			get
			{
				return _materializer;
			}
			set
			{
				Precondition.Require(value, () => Error.ArgumentNull("value"));
				_materializer = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="Radischevo.Wahha.Data.IDbDataProvider"/> 
		/// used to perform database queries. If no data provider is set, 
		/// the default instance will be used.
		/// </summary>
		public IDbDataProvider DataProvider
		{
			get
			{
				if (_dataProvider == null)
					_dataProvider = DbDataProvider.Create();

				return _dataProvider;
			}
			set
			{
				_dataProvider = value;
			}
		}
		#endregion

		#region Instance Methods
		/// <summary>
		/// When overridden in a derived class, creates a 
		/// <see cref="Radischevo.Wahha.Data.DbCommandDescriptor"/> used to 
		/// select a single entity by its unique key.
		/// </summary>
		/// <param name="key">The unique key of the entity.</param>
		protected abstract DbCommandDescriptor CreateEntitySelector(TKey key);

		/// <summary>
		/// When overridden in a derived class, gets the unique key 
		/// value of the provided entity.
		/// </summary>
		/// <param name="entity">The entity to extract key from.</param>
		protected abstract TKey ExtractKey(TEntity entity);

		/// <summary>
		/// Creates and materializes a new entity object using the 
		/// provided <paramref name="values">data source</paramref>.
		/// </summary>
		/// <param name="values">An <see cref="Radischevo.Wahha.Core.IValueSet"/> 
		/// containing the data for entity materialization.</param>
		protected virtual TEntity Materialize(IValueSet values)
		{
			return _materializer.Materialize(values);
		}

		/// <summary>
		/// Executes the select query and updates the 
		/// data properties of the provided entity.
		/// </summary>
		/// <param name="entity">The entity to update.</param>
		public TEntity Load(TEntity entity)
		{
			Precondition.Require(entity, () => Error.ArgumentNull("entity"));
			return ExecuteLoad(entity, CreateEntitySelector(ExtractKey(entity)));
		}

		/// <summary>
		/// Executes the select query described by the provided <paramref name="command"/> 
		/// and transforms the query result into the collection of entities.
		/// </summary>
		/// <param name="command">The unique key of the entity.</param>
		protected override IEnumerable<TEntity> ExecuteSelect(DbCommandDescriptor command)
		{
			return DataProvider.Execute(command)
				.Using(CommandBehavior.CloseConnection)
				.AsDataReader(reader => {
					List<TEntity> list = new List<TEntity>(SelectBufferSize);
					int count = -1;

					while (reader.Read())
						list.Add(Materialize(reader));

					if (reader.NextResult() && reader.Read()
						&& reader.Keys.Count() == 1)
						count = reader.GetValue<int>(0);

					return new Subset<TEntity>(list, count);
				});
		}

		/// <summary>
		/// Selects a single entity using the provided unique key.
		/// </summary>
		/// <param name="key">An object describing the command to execute.</param>
		protected override TEntity ExecuteSingle(TKey key)
		{
			return ExecuteSingle(CreateEntitySelector(key));
		}

		/// <summary>
		/// Executes the select query and updates the data properties of the provided 
		/// entity using the first element of the result set.
		/// </summary>
		/// <param name="entity">The entity to update.</param>
		/// <param name="command">An object describing the command to execute.</param>
		protected override TEntity ExecuteLoad(TEntity entity, DbCommandDescriptor command)
		{
			return DataProvider.Execute(command).Using(CommandBehavior.CloseConnection)
				.AsDataReader(reader => {
					IDbDataRecord record = reader.SingleOrDefault();

					if (record != null)
						return Materializer.Materialize(entity, record);

					return null;
				});
		}

		/// <summary>
		/// Executes the select query described by the provided <paramref name="command"/> 
		/// and transforms the first element of the resultset into entity.
		/// </summary>
		/// <param name="command">An object describing the command to execute.</param>
		protected override TEntity ExecuteSingle(DbCommandDescriptor command)
		{
			return DataProvider.Execute(command).Using(CommandBehavior.CloseConnection)
				.AsDataReader(reader => {
					if(reader.Read())
						return Materialize(reader);
					
					return null;
				});
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or
		/// resetting unmanaged resources.
		/// </summary>
		/// <param name="disposing">A value indicating whether 
		/// the disposal is called explicitly.</param>
		protected override void Dispose(bool disposing)
		{
			if (_dataProvider != null)
				_dataProvider.Dispose();

			IDisposable dm = (_materializer as IDisposable);
			if (dm != null)
				dm.Dispose();
		}
		#endregion
	}
}

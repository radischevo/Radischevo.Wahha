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
		#region Nested Types
		private class CachedDbEntry<TEntity>
		{
			#region Instance Fields
			private TEntity _entity;
			private bool _complete;
			#endregion

			#region Constructors
			public CachedDbEntry(TEntity entity, bool complete)
			{
				_entity = entity;
				_complete = complete;
			}
			#endregion

			#region Instance Properties
			public TEntity Entity
			{
				get
				{
					return _entity;
				}
			}

			public bool Complete
			{
				get
				{
					return _complete;
				}
			}
			#endregion
		}
		#endregion

		#region Instance Fields
		private IDbMaterializer<TEntity> _materializer;
		private IDbDataProvider _dataProvider;
		#endregion

		#region Constructors
		protected KeyedDbRepository(IDbMaterializer<TEntity> materializer)
			: this(null, materializer)
		{
		}

		protected KeyedDbRepository(IDbDataProvider dataProvider,
			IDbMaterializer<TEntity> materializer)
		{
			Precondition.Require(materializer, Error.ArgumentNull("materializer"));

			_dataProvider = dataProvider;
			_materializer = materializer;
		}
		#endregion

		#region Instance Properties
		public IDbMaterializer<TEntity> Materializer
		{
			get
			{
				return _materializer;
			}
			set
			{
				Precondition.Require(value, Error.ArgumentNull("value"));
				_materializer = value;
			}
		}

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
		protected abstract DbCommandDescriptor CreateEntitySelector(TKey key);

		protected abstract TKey ExtractKey(IValueSet values);

		protected virtual TEntity Materialize(IValueSet values, bool isComplete)
		{
			if (EnableCaching)
			{
				TKey key = ExtractKey(values);

				CachedDbEntry<TEntity> instance = Cache.Get(CreateCacheKey(key),
					() => new CachedDbEntry<TEntity>(_materializer.Materialize(values), isComplete),
					DateTime.Now.Add(ExpirationTimeout), Tags.ToArray());

				if (isComplete && !instance.Complete)
					_materializer.Materialize(instance.Entity, values);

				return instance.Entity;
			}
			return _materializer.Materialize(values);
		}

		protected override IEnumerable<TEntity> ExecuteSelect(DbCommandDescriptor command)
		{
			Precondition.Require(command, Error.ArgumentNull("command"));

			return DataProvider.Execute(command)
				.Using(CommandBehavior.CloseConnection)
				.AsDataReader(reader => {
					List<TEntity> list = new List<TEntity>();
					int count = -1;

					while (reader.Read())
						list.Add(Materialize(reader, false));

					if (reader.NextResult() && reader.Read()
						&& reader.Keys.Count() == 1)
						count = reader.GetValue<int>(0);

					return new Subset<TEntity>(list, count);
				});
		}

		protected override TEntity ExecuteSingle(TKey key)
		{
			return ExecuteSingle(CreateEntitySelector(key), true);
		}

		protected override TEntity ExecuteSingle(DbCommandDescriptor command)
		{
			return ExecuteSingle(command, false);
		}

		protected virtual TEntity ExecuteSingle(DbCommandDescriptor command, bool isComplete)
		{
			Precondition.Require(command, Error.ArgumentNull("command"));

			return DataProvider.Execute(command).Using(CommandBehavior.CloseConnection)
				.AsDataReader(reader => {
					IDbDataRecord record = reader.SingleOrDefault();

					if (record != null)
						return Materialize(record, isComplete);

					return null;
				});
		}

		public override TEntity Single(TKey key, params string[] tags)
		{
			return ExecuteSingle(key);
		}

		protected override void Dispose(bool disposing)
		{
			if (_dataProvider != null)
				_dataProvider.Dispose();

			_dataProvider = null;
		}
		#endregion
	}
}

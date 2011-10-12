using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Radischevo.Wahha.Data;
using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data.Caching;
using System.Threading;

namespace ConsoleTester
{
	public class City
	{
		public City()
		{
			Capital = new Link<City>();
			Neighbours = new EnumerableLink<City>();
		}

		public long Id
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public long RegionId
		{
			get;
			set;
		}

		public Link<City> Capital
		{
			get;
			private set;
		}

		public EnumerableLink<City> Neighbours
		{
			get;
			private set;
		}
	}

	public interface ICityOperations
	{
		IDbOperation<City> Single(long id);

		IDbOperation<IEnumerable<City>> Select();
	}

	public class CityOperations : ICityOperations
	{
		#region Instance Methods
		public IDbOperation<City> Single(long id)
		{
			return new SingleCityCommand(id);
		}

		public IDbOperation<IEnumerable<City>> Select()
		{
			return new SelectCityCommand();
		}
		#endregion
	}

	public class CityMaterializer : ObjectMaterializer<City>
	{
		protected override City CreateInstance(IDbValueSet source)
		{
			return new City();
		}

		protected override City Execute(City entity, IDbValueSet source)
		{
			var sub = source.Subset(k => k.StartsWith("region.")).Transform(k => k.Substring("region.".Length));

			entity.Id = source.GetValue<long>("id");
			entity.Title = source.GetValue<string>("title");
			entity.RegionId = sub.GetValue<long>("id");

			long client = (entity.Id == 38708) ? 0 : 38708;

			
			bool b = sub.AccessedKeys.Any();

			//Associate(entity.Capital).With<CityOperations>(b => b.Single(client)).Apply();
			Associate(entity.Capital).With(() => new SingleCityCommand(client)).Apply();

			//Associate(entity.Neighbours).With<CityOperations>(b => b.Select()).Apply();

			return entity;
		}
	}

	public class InsertCityCommand : DbCommandOperation<City>
	{
		private City _city;

		public InsertCityCommand(City city)
			: base()
		{
			_city = city;
		}

		protected override DbCommandDescriptor CreateCommand()
		{
			return new DbCommandDescriptor(SqlQueries.ScalarInsertQuery, new {
				title = _city.Title,
				regionId = _city.RegionId
			});
		}

		protected override City ExecuteCommand(IDbDataProvider provider, DbCommandDescriptor command)
		{
			_city.Id = provider.Execute(command).AsScalar<long>();
			return _city;
		}
	}

	public class UpdateCityCommand : DbCommandOperation<City>
	{
		private City _city;

		public UpdateCityCommand(City city)
			: base()
		{
			_city = city;
		}

		protected override DbCommandDescriptor CreateCommand()
		{
			return new DbCommandDescriptor(SqlQueries.UpdateQuery, new {
				id = _city.Id,
				title = _city.Title,
				regionId = _city.RegionId
			});
		}

		protected override City ExecuteCommand(IDbDataProvider provider, DbCommandDescriptor command)
		{
			provider.Execute(command).AsNonQuery();
			return _city;
		}
	}

	public class DeleteCityCommand : DbCommandOperation<City>
	{
		private City _city;

		public DeleteCityCommand(City city)
			: base()
		{
			_city = city;
		}

		protected override DbCommandDescriptor CreateCommand()
		{
			return new DbCommandDescriptor(SqlQueries.DeleteQuery, new {
				id = _city.Id
			});
		}

		protected override City ExecuteCommand(IDbDataProvider provider, DbCommandDescriptor command)
		{
			provider.Execute(command).AsNonQuery();
			_city.Id = 0;
			return _city;
		}
	}

	public class SingleCityCommand : DbSingleOperation<City>
	{
		private long _id;

		public SingleCityCommand(long id)
			: base(new CityMaterializer())
		{
			_id = id;
		}

		protected override DbCommandDescriptor CreateCommand()
		{
			return new DbCommandDescriptor(SqlQueries.SingleQuery, new {
				id = _id
			});
		}
	}

	public class SelectCityCommand : DbSelectOperation<City>
	{
		public SelectCityCommand()
			: base(new CityMaterializer())
		{
		}

		protected override DbCommandDescriptor CreateCommand()
		{
			return new DbCommandDescriptor(SqlQueries.SelectQuery);
		}
	}

	public class SelectCitySubsetCommand : CachedDbCommandOperation<IEnumerable<City>>
	{
		private int _skip;
		private int _take;

		public SelectCitySubsetCommand(ITaggedCacheProvider cache, int skip, int take)
		{
			Cache = cache;
			ExpirationTimeout = TimeSpan.FromHours(1);

			Tags.Add("global");
			Tags.Add("cities");
			Tags.Add("regions");

			_skip = skip;
			_take = take;
		}

		protected override DbCommandDescriptor CreateCommand()
		{
			return new DbCommandDescriptor(SqlQueries.SelectQueryPaged, new {
				skip = _skip,
				take = _take
			});
		}

		protected override IEnumerable<City> ExecuteCommand(IDbDataProvider provider, 
			DbCommandDescriptor command)
		{
			return provider.Execute(command).AsEntitySet<City>(new CityMaterializer()).ToList();
		}
	}

	public class ExecuteQueryCommand : DbCommandOperation<DbQueryResult>
	{
		private int _skip;
		private int _take;

		public ExecuteQueryCommand(int skip, int take)
		{
			_skip = skip;
			_take = take;
		}

		protected override DbCommandDescriptor CreateCommand()
		{
			return new DbCommandDescriptor(SqlQueries.SelectQueryPaged, new {
				skip = _skip,
				take = _take
			});
		}

		protected override DbQueryResult ExecuteCommand(IDbDataProvider provider, 
			DbCommandDescriptor command)
		{
			return provider.Execute(command).AsDataReader(reader => new DbQueryResult(reader));
		}
	}
}

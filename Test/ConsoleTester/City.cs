using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Radischevo.Wahha.Data;
using Radischevo.Wahha.Core;

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
		protected override City CreateInstance(IValueSet source)
		{
			return new City();
		}

		protected override City Execute(City entity, IValueSet source)
		{
			entity.Id = source.GetValue<long>("id");
			entity.Title = source.GetValue<string>("title");
			entity.RegionId = source.GetValue<long>("region.id");

			long client = (entity.Id == 38708) ? 0 : 38708;
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
			Command = new DbCommandDescriptor(SqlQueries.ScalarInsertQuery, new {
				title = city.Title,
				regionId = city.RegionId
			});
		}

		protected override City ExecuteInternal(IDbDataProvider provider)
		{
			_city.Id = provider.Execute(Command).AsScalar<long>();
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
			Command = new DbCommandDescriptor(SqlQueries.UpdateQuery, new {
				id = city.Id,
				title = city.Title,
				regionId = city.RegionId
			});
		}

		protected override City ExecuteInternal(IDbDataProvider provider)
		{
			provider.Execute(Command).AsNonQuery();
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
			Command = new DbCommandDescriptor(SqlQueries.DeleteQuery, new {
				id = city.Id
			});
		}

		protected override City ExecuteInternal(IDbDataProvider provider)
		{
			provider.Execute(Command).AsNonQuery();
			_city.Id = 0;
			return _city;
		}
	}

	public class SingleCityCommand : DbSingleOperation<City>
	{
		public SingleCityCommand(long id)
			: base(new CityMaterializer())
		{
			Command = new DbCommandDescriptor(SqlQueries.SingleQuery, new {
				id = id
			});
		}
	}

	public class SelectCityCommand : DbSelectOperation<City>
	{
		public SelectCityCommand()
			: base(new CityMaterializer())
		{
			Command = new DbCommandDescriptor(SqlQueries.SelectQuery);
		}
	}

	public class SelectCitySubsetCommand : DbSubsetOperation<City>
	{
		public SelectCitySubsetCommand(int skip, int take)
			: base(new CityMaterializer())
		{
			Command = new DbCommandDescriptor(SqlQueries.SelectQueryPaged, new {
				skip = skip, take = take
			});
		}
	}
}

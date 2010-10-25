using System;
using System.Collections.Generic;

using Radischevo.Wahha.Data;

namespace ConsoleTester
{
	public interface IItemRepository : IRepository<Item, long>
	{
		IEnumerable<Item> Select(int skip, int take);
	}

	public interface IItemDataRepository : IRepository<ItemData, long>
	{
		IEnumerable<ItemData> Select(Item item);
	}

	public class ItemRepository : KeyedDbRepository<Item, long>, IItemRepository
	{
		public ItemRepository(IDbMaterializer<Item> materializer)
			: base(materializer)
		{
		}

		protected override DbCommandDescriptor CreateEntitySelector(long key)
		{
			return new DbCommandDescriptor(SqlQueries.SingleQuery, new {
				id = key
			});
		}

		protected override long ExtractKey(Item entity)
		{
			return entity.Id;
		}

		public IEnumerable<Item> Select(int skip, int take)
		{
			return Select(new DbCommandDescriptor(SqlQueries.SelectQuery, 
				new { skip = skip, take = take }));
		}

		protected override Item ExecuteSave(Item entity)
		{
			return entity;
		}

		protected override Item ExecuteDelete(Item entity)
		{
			return entity;
		}
	}

	public class ItemDataRepository : KeyedDbRepository<ItemData, long>, IItemDataRepository
	{
		public ItemDataRepository(IDbMaterializer<ItemData> materializer)
			: base(materializer)
		{
		}

		protected override DbCommandDescriptor CreateEntitySelector(long key)
		{
			return new DbCommandDescriptor(SqlQueries.DataQuery, new {
				id = key
			});
		}

		public IEnumerable<ItemData> Select(Item item)
		{
			return Select(new DbCommandDescriptor(SqlQueries.CollectionDataQuery, new {
				id = item.Id
			}));
		}

		protected override long ExtractKey(ItemData entity)
		{
			return entity.Item;
		}

		protected override ItemData ExecuteSave(ItemData entity)
		{
			return entity;
		}

		protected override ItemData ExecuteDelete(ItemData entity)
		{
			return entity;
		}
	}
}

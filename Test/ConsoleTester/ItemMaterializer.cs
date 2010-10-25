using System;
using System.Collections.Generic;
using System.Linq;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data;

using Microsoft.Practices.Unity;
using System.Linq.Expressions;
using System.Reflection;
using Radischevo.Wahha.Core.Expressions;

namespace ConsoleTester
{
	public class ItemMaterializer : ObjectMaterializer<Item>
	{
		public ItemMaterializer()
			: base()
		{
		}

		protected override Item CreateInstance(IValueSet source)
		{
			return new Item();
		}

		protected override Item Execute(Item entity, IValueSet source)
		{
			entity.Id = source.GetValue<long>("id");
			entity.Name = source.GetValue<string>("name");
			entity.Alias = source.GetValue<string>("alias");
			entity.Description = source.GetValue<string>("description");
			entity.DateCreated = source.GetValue<DateTime>("dateCreated");
			entity.DateLastModified = source.GetValue<DateTime>("dateLastModified");

			Associate(entity.Data)
				.With<IItemDataRepository>(r => r.Single(source.GetValue<long>("id")))
				.Subset("data.").Scheme("field_1_value").Apply(source);

			Associate(entity.Values)
				.With<IItemDataRepository>(r => r.Select(entity)).Apply();

			return entity;
		}
	}

	public class ItemDataMaterializer : ObjectMaterializer<ItemData>
	{
		protected override ItemData CreateInstance(IValueSet source)
		{
			return new ItemData();
		}

		protected override ItemData Execute(ItemData entity, IValueSet source)
		{
			entity.Item = source.GetValue<long>("productId");
			entity.Amount = new Money(
				source.GetValue<decimal>("field_1_value"),
				source.GetValue<string>("field_1_currency"));
			entity.Length = source.GetValue<int>("field_3_value");
			entity.Percent = source.GetValue<float>("field_2_value");
			entity.Comments = source.GetValue<string>("field_5_value");

			return entity;
		}
	}
}

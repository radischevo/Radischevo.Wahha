using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Radischevo.Wahha.Data;

namespace ConsoleTester
{
	public static class FastFoodQueries
	{
		public static TValue Scalar<TValue>(this DbQueryResult result)
		{
			foreach (DbSubQueryResult item in result)
				foreach (DbQueryResultRow row in item)
					return row.GetValue<TValue>(0);

			return default(TValue);
		}

		public static string DropAll
		{
			get {
				return @"DELETE FROM [dbo].[FastFood.Items.Data];
						DELETE FROM [dbo].[FastFood.Items];
						DELETE FROM [dbo].[FastFood.Groups];
						DELETE FROM [dbo].[FastFood.Catalogs];
						DELETE FROM [dbo].[FastFood.Brands];
						DELETE FROM [dbo].[FastFood.Partners];
						DELETE FROM [dbo].[FastFood.Element.Tags];
						DELETE FROM [dbo].[FastFood.Tags]";
			}
		}

		public static string CreateCatalog
		{
			get
			{
				return @"INSERT INTO [dbo].[FastFood.Catalogs] ([alias], [name], [active])
						VALUES (@alias, @name, @active); SELECT SCOPE_IDENTITY();";
			}
		}

		public static string CreateGroup
		{
			get
			{
				return @"INSERT INTO [dbo].[FastFood.Groups] ([alias], [name], [catalog], [parent], [left_key], [right_key], [level])
					VALUES (@alias, @name, @catalog, @parent, @left_key, @right_key, @level); SELECT SCOPE_IDENTITY();";
			}
		}

		public static string CreateItem
		{
			get
			{
				return @"DECLARE @id bigint; 
						INSERT INTO [dbo].[FastFood.Items] ([catalog], [group], [alias], [name], [partner], [brand], [active], [price], [options], [locale])
						VALUES (@catalog, @group, @alias, @name, @partner, @brand, 1, @price, @options, @locale);
						SELECT @id = SCOPE_IDENTITY();
						INSERT INTO [dbo].[FastFood.Items.Data] ([item], [locale], [properties])
						VALUES (@id, @locale, @properties);
						SELECT @id;";
			}
		}

		public static string CreatePartner
		{
			get
			{
				return @"INSERT INTO [dbo].[FastFood.Partners] ([alias], [name], [active]) VALUES (@alias, @name, 1); SELECT SCOPE_IDENTITY();";
			}
		}

		public static string CreateBrand
		{
			get
			{
				return @"INSERT INTO [dbo].[FastFood.Brands] ([partner], [alias], [name]) VALUES (@partner, @alias, @name); SELECT SCOPE_IDENTITY();";
			}
		}

		public static string CreateTag
		{
			get
			{
				return @"INSERT INTO [dbo].[FastFood.Tags] ([value], [category]) VALUES (@value, @category)";
			}
		}

		public static string CreateElementTag
		{
			get
			{
				return @"INSERT INTO [dbo].[FastFood.Element.Tags] ([key], [dependency], [tag]) VALUES (@key, @dependency, @tag)";
			}
		}
	}
}

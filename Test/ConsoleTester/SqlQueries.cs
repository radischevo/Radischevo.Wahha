using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleTester
{
	internal static class SqlQueries
	{
		internal static string SelectQuery = @"DECLARE @results TABLE ([index] bigint, [productId] bigint)
			INSERT INTO @results ([index], [productId])
			SELECT [t].[index], [t].[product]
			FROM (
				SELECT ROW_NUMBER() OVER (ORDER BY [p].[id] ASC) AS [index], [p].[id] AS [product]
				FROM [dbo].[workle.products] AS [p]
					INNER JOIN [dbo].[workle.product.data.1] AS [d]
					ON [p].[id] = [d].[productId]
				) AS [t]
			WHERE [t].[index] BETWEEN (@skip + 1) AND (@skip + @take)

			SELECT [p].*, [d].[field_1_value] AS [data.field_1_value], [d].[field_5_value] as [data.field_5_value]
				FROM [dbo].[workle.products] AS [p]
				INNER JOIN [dbo].[workle.product.data.1] AS [d]
				ON [d].[productId] = [p].[id]
				INNER JOIN @results AS [r]
				ON [r].[productId] = [p].[id]
				ORDER BY [r].[index] ASC";

		internal static string SingleQuery = @"
			SELECT [p].* FROM [dbo].[workle.products] AS [p]
				WHERE [p].[id] = @id";

		internal static string DataQuery = @"
			SELECT [d].* 
			FROM [dbo].[workle.product.data.1] AS [d]
			WHERE [d].[productId] = @id";
	}
}

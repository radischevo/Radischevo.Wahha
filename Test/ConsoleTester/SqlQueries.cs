using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleTester
{
	internal static class SqlQueries
	{
		internal static string SelectQuery = @"SELECT [C].[Id] AS [id], [C].[Title] AS [title], [C].[RegionId] AS [region.id] 
			FROM [dbo].[Workle.Cities] AS [C] ORDER BY [Title] ASC";

		internal static string SelectQueryPaged = @"
			DECLARE @index TABLE ([index] [int] IDENTITY(1, 1), [id] [bigint])

			INSERT INTO @index ([id])
			SELECT [C].[Id]
				FROM [dbo].[Workle.Cities] AS [C] 
				ORDER BY [Title] ASC
			
			SELECT [C].[Id] AS [id], [C].[Title] AS [title], [C].[RegionId] AS [region.id] 
				FROM [dbo].[Workle.Cities] AS [C]
				INNER JOIN @index AS [I]
				ON [C].[Id] = [I].[id]
				WHERE [I].[index] BETWEEN @skip + 1 AND @skip + @take
				ORDER BY [I].[index] ASC

			SELECT COUNT(1) FROM @index";

		internal static string SingleQuery = @"SELECT [C].[Id] AS [id], [C].[Title] AS [title], [C].[RegionId] AS [region.id] 
			FROM [dbo].[Workle.Cities] AS [C] WHERE [C].[Id] = @id";

		internal static string ScalarQuery = @"SELECT COUNT(1) FROM [dbo].[Workle.Cities]";

		internal static string ScalarInsertQuery = @"INSERT INTO [dbo].[Workle.Cities] ([RegionId], [Title], [Enabled]) 
			VALUES (@regionId, @title, 1);
			SELECT SCOPE_IDENTITY()";

		internal static string UpdateQuery = @"UPDATE [dbo].[Workle.Cities] SET 
			[Title] = @title WHERE [Id] = @id";

		internal static string DeleteQuery = @"DELETE FROM [dbo].[Workle.Cities] WHERE [Id] = @id";
	}
}

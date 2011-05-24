using System;

namespace Radischevo.Wahha.Data
{
	internal static class ScopedDbOperationExecutor<TAssociation>
	{
		#region Static Methods
		public static TAssociation Execute(IDbOperation<TAssociation> operation)
		{
			using (DbOperationScope scope = new DbOperationScope())
			{
				return scope.Execute<TAssociation>(operation);
			}
		}
		#endregion
	}
}

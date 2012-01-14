using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public interface IDbSerializer
	{
		#region Instance Methods
		IValueSet Serialize (object entity, DbQueryStatement statement);
		
		object Deserialize (IDbValueSet source);
		#endregion
	}
	
	public interface IDbSerializer<TEntity> : IDbSerializer
	{
		#region Instance Methods
		IValueSet Serialize (TEntity entity, DbQueryStatement statement);
		
		new TEntity Deserialize (IDbValueSet source);
		#endregion
	}
}


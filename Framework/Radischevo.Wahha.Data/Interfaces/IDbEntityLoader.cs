using System;

namespace Radischevo.Wahha.Data
{
	public interface IDbEntityLoader<TEntity>
	{
		TEntity Load(TEntity entity, DbCommandDescriptor command);
	}
}

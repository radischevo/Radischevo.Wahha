using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public interface IDbColumnMapper<TEntity>
	{
		IValueSet Map(TEntity entity);

		IValueSet Map(TEntity entity, DbQueryType type);
	}
}

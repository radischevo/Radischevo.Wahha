using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Data
{
    public interface IDbQueryService<TEntity>
    {
        IEnumerable<TEntity> Select(DbCommandDescriptor command);

        TEntity Single(DbCommandDescriptor command);
    }
}

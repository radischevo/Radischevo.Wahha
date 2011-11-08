using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Data
{
    public interface IRepository<TEntity> : IDisposable
    {
    }

    public interface IRepository<TEntity, TKey> : IRepository<TEntity>
    {
        TEntity Single(TKey key);
    }
}

using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Data
{
    public interface IRepository<TEntity> : IDisposable
    {
        TEntity Save(TEntity entity);

        TEntity Delete(TEntity entity);
    }

    public interface IRepository<TEntity, TKey> : IRepository<TEntity>
    {
        TEntity Single(TKey key);
    }
}

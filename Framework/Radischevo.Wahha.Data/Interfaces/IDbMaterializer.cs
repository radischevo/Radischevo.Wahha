using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
    public interface IDbMaterializer<TEntity>
    {
        TEntity Materialize(IValueSet source);

        TEntity Materialize(TEntity entity, IValueSet source);
    }
}

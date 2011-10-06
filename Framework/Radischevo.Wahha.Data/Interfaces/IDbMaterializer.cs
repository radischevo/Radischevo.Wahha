using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
    public interface IDbMaterializer<TEntity>
    {
        TEntity Materialize(IDbValueSet source);

        TEntity Materialize(TEntity entity, IDbValueSet source);
    }
}

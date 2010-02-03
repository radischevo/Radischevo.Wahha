using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
    public class ObjectReader<TEntity> : IEnumerable, IEnumerable<TEntity>, IDisposable
    {
        #region Instance Fields
        private ObjectEnumerator<TEntity> _enumerator;
        #endregion

        #region Constructors
        public ObjectReader(IDbDataReader reader, Func<IDbDataRecord, TEntity> translator)
        {
            Precondition.Require(reader, Error.ArgumentNull("reader"));
            Precondition.Require(translator, Error.ArgumentNull("translator"));

            _enumerator = new ObjectEnumerator<TEntity>(reader, translator);
        }
        #endregion

        #region Instance Methods
        public IEnumerator<TEntity> GetEnumerator()
        {
            ObjectEnumerator<TEntity> e = _enumerator;
            if (e == null)
                throw Error.CannotEnumerateMoreThanOnce();

            _enumerator = null;
            return e;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_enumerator != null)
                _enumerator.Dispose();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
    internal sealed class ObjectEnumerator<TEntity> : IEnumerator<TEntity>, IEnumerator, IDisposable
    {
        #region Instance Fields
        private TEntity _current;
        private IEnumerator<IDbDataRecord> _enumerator;
        private Func<IDbDataRecord, TEntity> _translator;
        private IDbDataReader _reader;
        #endregion

        #region Constructors
        public ObjectEnumerator(IDbDataReader reader, Func<IDbDataRecord, TEntity> translator)
        {
            Precondition.Require(reader, () => Error.ArgumentNull("reader"));
            Precondition.Require(translator, () => Error.ArgumentNull("translator"));

            _reader = reader;
            _enumerator = _reader.GetEnumerator();
            _translator = translator;
        }

		public ObjectEnumerator(IDbDataReader reader, IDbMaterializer<TEntity> materializer)
		{
			Precondition.Require(reader, () => Error.ArgumentNull("reader"));
			Precondition.Require(materializer, () => Error.ArgumentNull("materializer"));

			_reader = reader;
			_enumerator = _reader.GetEnumerator();
			_translator = materializer.Materialize;
		}
        #endregion

        #region Instance Properties
        public TEntity Current 
        {
            get
            {
                return _current;
            }
        }

        object IEnumerator.Current 
        {
            get
            {
                return Current;
            }
        }
        #endregion

        #region Instance Methods
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) 
        {
			if (disposing)
			{
				if (_reader != null)
					_reader.Dispose();
			}
        }

        public bool MoveNext()
        {
            if (_enumerator.MoveNext())
            {
                _current = _translator(_enumerator.Current);
                return true;
            }
            return false;
        }

        public void Reset() 
        {
        }
        #endregion
    }
}

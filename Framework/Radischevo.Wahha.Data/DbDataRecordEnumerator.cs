using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
    /// <summary>
    /// Acts as a decorator for the 
    /// <see cref="System.Data.Common.DbEnumerator"/> class.
    /// </summary>
    internal sealed class DbDataRecordEnumerator : IEnumerator<IDbDataRecord>
    {
        #region Instance Fields
        private DbEnumerator _enumerator; 
        private DbFieldLookup _lookup;
        private IDbDataReader _reader;
        private IDbDataRecord _current;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="DbDataRecordEnumerator"/> class.
        /// </summary>
        /// <param name="reader">The data reader instance, containing 
        /// the result set.</param>
        /// <param name="lookup">The column name lookup dictionary.</param>
        public DbDataRecordEnumerator(IDbDataReader reader, 
            DbFieldLookup lookup)
        {
            Precondition.Require(reader, () => Error.ArgumentNull("reader"));
            Precondition.Require(lookup, () => Error.ArgumentNull("lookup"));

            _reader = reader;
            _lookup = lookup;
            _enumerator = new DbEnumerator(_reader, false);
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets the current element in 
        /// the collection.
        /// </summary>
        public IDbDataRecord Current
        {
            get 
            {
                return _current;
            }
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Advances the enumerator to the 
        /// next element in the collection.
        /// </summary>
        public bool MoveNext()
        {
            _current = null;
            if (_enumerator.MoveNext())
            {
                _current = new DbDataRecord((IDataRecord)_enumerator.Current, _lookup);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Throws the <see cref="System.NotSupportedException"/>.
        /// </summary>
        public void Reset()
        {
            throw Error.CannotEnumerateMoreThanOnce();
        }

        /// <summary>
        /// Performs tasks associated 
        /// with freeing, releasing or 
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
			GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            IDisposable disp = (_enumerator as IDisposable);
            if (disp != null)
                disp.Dispose();
        }
        #endregion

        #region IEnumerator Members
        object IEnumerator.Current
        {
            get 
            {
                return _current;
            }
        }
        #endregion
    }
}

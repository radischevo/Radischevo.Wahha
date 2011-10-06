using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Radischevo.Wahha.Data
{
    /// <summary>
    /// Acts as a high-level decorator for the 
    /// <see cref="System.Data.IDataReader"/> object, 
    /// extending and adding some useful functionality 
    /// to the decorated instance.
    /// </summary>
    public sealed class DbDataReader : DbDataRecord, IDbDataReader
    {
        #region Instance Fields
        private bool _disposed;
        private IDataReader _reader;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Radischevo.Wahha.Data.DbDataReader"/> class.
        /// </summary>
        /// <param name="reader">The <see cref="System.Data.IDataReader"/> 
        /// to decorate.</param>
        internal DbDataReader(IDataReader reader)
			: base(reader)
        {
            _reader = reader;
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets a value indicating whether 
        /// the data reader is closed.
        /// </summary>
        public bool IsClosed
        {
            get
            {
                return _reader.IsClosed;
            }
        }

		/// <summary>
        /// Gets a value indicating the 
        /// depth of nesting for the current row.
        /// </summary>
        public int Depth
        {
            get
            {
                return _reader.Depth;
            }
        }

        /// <summary>
        /// Gets the number of rows changed, 
        /// inserted, or deleted by execution 
        /// of the SQL statement.
        /// </summary>
        public int RecordsAffected
        {
            get
            {
                return _reader.RecordsAffected;
            }
        }
        #endregion

        #region Dispose Methods
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

        /// <summary>
        /// Performs tasks associated 
        /// with freeing, releasing or 
        /// resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">A value indicating 
        /// that the process was initiated internally.</param>
        private void Dispose(bool disposing)
        {
            if (!_disposed && _reader != null)
            {
                _reader.Close();
                _reader.Dispose();

				_reader = null;
				_disposed = true;
            }
        }
        #endregion

        #region DataReader Methods
        /// <summary>
        /// Closes the <see cref="Radischevo.Wahha.Data.DbDataReader"/> object.
        /// </summary>
        public void Close()
        {
            _reader.Close();
        }

        /// <summary>
        /// Returns a <see cref="System.Data.DataTable"/> that describes 
        /// describes the column metadata of the 
        /// <see cref="Radischevo.Wahha.Data.DbDataReader"/>.
        /// </summary>
        public DataTable GetSchemaTable()
        {
            return _reader.GetSchemaTable();
        }

        /// <summary>
        /// Advances the data reader to the next result, 
        /// when reading the results of batch SQL statements.
        /// </summary>
        public bool NextResult()
        {
			ResetAccessedKeys();
			ResetLookupTable();

            if (_reader.NextResult())
            {
				UpdateLookupTable();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Advances the data reader 
        /// to the next record.
        /// </summary>
        public bool Read()
        {
			ResetAccessedKeys();
            return _reader.Read();
        }

        /// <summary>
        /// Gets the IEnumerator that iterates 
        /// through the records of the current 
        /// result set.
        /// </summary>
        public IEnumerator<IDbDataRecord> GetEnumerator()
        {
            if (_disposed)
                throw Error.ObjectDisposed("DbDataReader");

            return new DbDataRecordEnumerator(this, Lookup);
        }
        #endregion

        #region IEnumerable Members
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
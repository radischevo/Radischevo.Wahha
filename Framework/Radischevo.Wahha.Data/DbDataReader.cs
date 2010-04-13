using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
    /// <summary>
    /// Acts as a high-level decorator for the 
    /// <see cref="System.Data.IDataReader"/> object, 
    /// extending and adding some useful functionality 
    /// to the decorated instance.
    /// </summary>
    public sealed class DbDataReader : IDbDataReader
    {
        #region Instance Fields
        private bool _disposed;
        private IDataReader _reader;
        private DbFieldLookup _lookup;
        private HashSet<string> _accessedFields;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Radischevo.Wahha.Data.DbDataReader"/> class.
        /// </summary>
        /// <param name="reader">The <see cref="System.Data.IDataReader"/> 
        /// to decorate.</param>
        internal DbDataReader(IDataReader reader)
        {
            Precondition.Require(reader, () => Error.ArgumentNull("reader"));
            
            _reader = reader;
            _accessedFields = new HashSet<string>(
                StringComparer.InvariantCultureIgnoreCase);
            _lookup = new DbFieldLookup();

            FillLookupTables();
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets the boxed value of the column 
        /// which has the specified ordinal.
        /// </summary>
        /// <param name="ordinal">The zero-based 
        /// column ordinal.</param>
        public object this[int ordinal]
        {
            get 
            {
                return GetValue(ordinal);
            }
        }

        /// <summary>
        /// Gets the boxed value of the 
        /// column which has the specified name.
        /// </summary>
        /// <param name="name">The name of the 
        /// column to find.</param>
        public object this[string name]
        {
            get
            {
                return GetValue(name);
            }
        }

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
        /// Gets the number of columns 
        /// in the current row.
        /// </summary>
        public int FieldCount
        {
            get 
            {
                return _reader.FieldCount;
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

        /// <summary>
        /// Gets the collection of column names, 
        /// which values were accessed by calling 
        /// value-getter methods of the current row.
        /// </summary>
        public IEnumerable<string> AccessedFields
        {
            get
            {
                return _accessedFields;
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
        private void FillLookupTables()
        {
			for (int i = 0; i < _reader.FieldCount; ++i)
				_lookup.Add(i, _reader.GetName(i));
        }

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
            _accessedFields.Clear();
            _lookup.Clear();

            if (_reader.NextResult())
            {
                FillLookupTables();
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
            _accessedFields.Clear();
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

            return new DbDataRecordEnumerator(this, _lookup);
        }
        #endregion

        #region DataRecord Helper Methods
        private void MarkAccessedField(int ordinal)
        {
            MarkAccessedField(GetName(ordinal));
        }

        private void MarkAccessedField(string name)
        {
            if (!_accessedFields.Contains(name))
                _accessedFields.Add(name);
        }

        /// <summary>
        /// Gets the name of the field to find.
        /// </summary>
        /// <param name="ordinal">The zero-based field 
        /// ordinal to find.</param>
        public string GetName(int ordinal)
        {
            string name;
            if(!_lookup.TryGetName(ordinal, out name))
                throw Error.ColumnOrdinalDoesNotExistInResultSet(ordinal);
            
            return name;
        }

        /// <summary>
        /// Returns the index of the named field.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        public int GetOrdinal(string name)
        {
            int ordinal;
            if (!_lookup.TryGetOrdinal(name, out ordinal))
                throw Error.ColumnNameDoesNotExistInResultSet(name);
            
            return ordinal;
        }

        /// <summary>
        /// Returns a value indicating 
        /// whether the specified field 
        /// is set to null.
        /// </summary>
        /// <param name="ordinal">The zero-based 
        /// column ordinal.</param>
        public bool IsDBNull(int ordinal)
        {
            if (_lookup.Contains(ordinal))
                return _reader.IsDBNull(ordinal);

            return true;
        }

        /// <summary>
        /// Returns a value indicating 
        /// whether the specified field 
        /// is set to null.
        /// </summary>
        /// <param name="name">The name of 
        /// the field to find</param>
        public bool IsDBNull(string name)
        {
            int index;
            if (_lookup.TryGetOrdinal(name, out index))
                return _reader.IsDBNull(index);

            return true;
        }

        /// <summary>
        /// Gets the data type information
        /// for the specified column.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        public string GetDataTypeName(int ordinal)
        {
            return _reader.GetDataTypeName(ordinal);
        }

        /// <summary>
        /// Gets the data type information
        /// for the specified column.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        public string GetDataTypeName(string name)
        {
            return _reader.GetDataTypeName(GetOrdinal(name));
        }

        /// <summary>
        /// Gets the <see cref="System.Type"/>, describing the specified 
        /// column value.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        public Type GetFieldType(int ordinal)
        {
            return _reader.GetFieldType(ordinal);
        }

        /// <summary>
        /// Gets the <see cref="System.Type"/>, describing the specified 
        /// column value.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        public Type GetFieldType(string name)
        {
            return _reader.GetFieldType(GetOrdinal(name));
        }

        /// <summary>
        /// Determines whether the current record 
        /// contains all of the listed fields.
        /// </summary>
        /// <param name="keys">An array, containing 
        /// the names of the fields to find.</param>
        public bool ContainsAll(params string[] keys)
        {
            if (keys == null || keys.Length < 1)
                return true;

            foreach (string key in keys)
                if (!ContainsKey(key))
                    return false;

            return true;
        }

        /// <summary>
        /// Determines whether the current record 
        /// contains at least one of the listed fields.
        /// </summary>
        /// <param name="keys">An array, containing 
        /// the names of the fields to find.</param>
        public bool ContainsAny(params string[] keys)
        {
            if (keys == null || keys.Length < 1)
                return true;

            foreach (string key in keys)
                if (ContainsKey(key))
                    return true;

            return false;
        }

        /// <summary>
        /// Returns a value indicating whether the column 
        /// with the specified name exists in the current 
        /// value set.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        public bool ContainsKey(string name)
        {
            return _lookup.Contains(name);
        }
        #endregion

        #region GetXxx By Column Ordinal Methods
        /// <summary>
        /// Gets the value of the specified column 
        /// as a <see cref="System.Boolean"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        public bool GetBoolean(int ordinal)
        {
            MarkAccessedField(ordinal);
            return _reader.GetBoolean(ordinal);
        }

        /// <summary>
        /// Gets the 8-bit unsigned integer value of the specified column.
        /// </summary>
        /// <param name="index">The zero-based column ordinal.</param>
        public byte GetByte(int ordinal)
        {
            MarkAccessedField(ordinal);
            return _reader.GetByte(ordinal);
        }

        /// <summary>
        /// Reads a stream of bytes from the specified column offset 
        /// into the buffer as an array, starting at the given buffer offset.
        /// </summary>
        /// <param name="index">The zero-based column ordinal.</param>
        /// <param name="fieldOffset">The index within the field from which 
        /// to start the read operation.</param>
        /// <param name="buffer">The buffer infto which to read the stream of bytes.</param>
        /// <param name="bufferOffset">The index for buffer to start the read operation.</param>
        /// <param name="length">The number of bytes to read.</param>
        public long GetBytes(int ordinal, long fieldOffset,
            byte[] buffer, int bufferOffset, int length)
        {
            MarkAccessedField(ordinal);
            return _reader.GetBytes(ordinal, fieldOffset,
                buffer, bufferOffset, length);
        }

        /// <summary>
        /// Gets the characher value of the specified column.
        /// </summary>
        /// <param name="index">The zero-based column ordinal.</param>
        public char GetChar(int ordinal)
        {
            MarkAccessedField(ordinal);
            return _reader.GetChar(ordinal);
        }

        /// <summary>
        /// Reads a stream of characters from the specified column offset 
        /// into the buffer as an array, starting at the given buffer offset.
        /// </summary>
        /// <param name="index">The zero-based column ordinal.</param>
        /// <param name="fieldOffset">The index within the field from which 
        /// to start the read operation.</param>
        /// <param name="buffer">The buffer infto which to read the stream of characters.</param>
        /// <param name="bufferOffset">The index for buffer to start the read operation.</param>
        /// <param name="length">The number of characters to read.</param>
        public long GetChars(int ordinal, long fieldOffset,
            char[] buffer, int bufferOffset, int length)
        {
            MarkAccessedField(ordinal);
            return _reader.GetChars(ordinal, fieldOffset, buffer,
                bufferOffset, length);
        }

        /// <summary>
        /// Gets a <see cref="System.Data.IDataReader"/> 
        /// for the specified column.
        /// </summary>
        /// <param name="index">The zero-based column ordinal.</param>
        public IDataReader GetData(int ordinal)
        {
            MarkAccessedField(ordinal);
            return _reader.GetData(ordinal);
        }

        /// <summary>
        /// Gets the date and time data value of the specified column. 
        /// </summary>
        /// <param name="index">The zero-based column ordinal.</param>
        public DateTime GetDateTime(int ordinal)
        {
            MarkAccessedField(ordinal);
            return _reader.GetDateTime(ordinal);
        }

        /// <summary>
        /// Gets the fixed-position numeric 
        /// value of the specified column. 
        /// </summary>
        /// <param name="index">The zero-based column ordinal.</param>
        public decimal GetDecimal(int ordinal)
        {
            MarkAccessedField(ordinal);
            return _reader.GetDecimal(ordinal);
        }

        /// <summary>
        /// Gets the double-precision floating point number 
        /// value of the specified column. 
        /// </summary>
        /// <param name="index">The zero-based column ordinal.</param>
        public double GetDouble(int ordinal)
        {
            MarkAccessedField(ordinal);
            return _reader.GetDouble(ordinal);
        }

        /// <summary>
        /// Gets the single-precision floating point number 
        /// value of the specified column. 
        /// </summary>
        /// <param name="index">The zero-based column ordinal.</param>
        public float GetFloat(int ordinal)
        {
            MarkAccessedField(ordinal);
            return _reader.GetFloat(ordinal);
        }

        /// <summary>
        /// Gets the GUID value of the specified column. 
        /// </summary>
        /// <param name="index">The zero-based column ordinal.</param>
        public Guid GetGuid(int ordinal)
        {
            MarkAccessedField(ordinal);
            return _reader.GetGuid(ordinal);
        }

        /// <summary>
        /// Gets the 16-bit signed integer 
        /// value of the specified column. 
        /// </summary>
        /// <param name="index">The zero-based column ordinal.</param>
        public short GetInt16(int ordinal)
        {
            MarkAccessedField(ordinal);
            return _reader.GetInt16(ordinal);
        }

        /// <summary>
        /// Gets the 32-bit signed integer 
        /// value of the specified column.
        /// </summary>
        /// <param name="index">The zero-based column ordinal.</param>
        public int GetInt32(int ordinal)
        {
            MarkAccessedField(ordinal);
            return _reader.GetInt32(ordinal);
        }

        /// <summary>
        /// Gets the 64-bit signed integer 
        /// value of the specified column. 
        /// </summary>
        /// <param name="index">The zero-based column ordinal.</param>
        public long GetInt64(int ordinal)
        {
            MarkAccessedField(ordinal);
            return _reader.GetInt64(ordinal);
        }

        /// <summary>
        /// Gets the string  
        /// value of the specified column. 
        /// </summary>
        /// <param name="index">The zero-based column ordinal.</param>
        public string GetString(int ordinal)
        {
            MarkAccessedField(ordinal);
            return _reader.GetString(ordinal);
        }

        /// <summary>
        /// Gets the boxed value of the result 
        /// column which has the specified name.
        /// </summary>
        /// <param name="index">The zero-based column ordinal.</param>
        public object GetValue(int ordinal)
        {
            string name;
            if (_lookup.TryGetName(ordinal, out name))
            {
                MarkAccessedField(name);
                return _reader.GetValue(ordinal);
            }
            return null;
        }

        /// <summary>
        /// Gets the strongly-typed value of the 
        /// specified column.
        /// </summary>
        /// <typeparam name="TValue">A type of column value.</typeparam>
        /// <param name="index">The zero-based column ordinal.</param>
        /// <param name="defaultValue">The default value of the variable.</param>
		/// <param name="provider">An <see cref="IFormatProvider" /> interface implementation that 
		/// supplies culture-specific formatting information.</param>
        public TValue GetValue<TValue>(int ordinal, TValue defaultValue, 
			IFormatProvider provider)
        {
            string name;
            if (_lookup.TryGetName(ordinal, out name))
            {
                MarkAccessedField(name);
                return Converter.ChangeType<TValue>(_reader.GetValue(ordinal), 
					defaultValue, provider);
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets all the attribute fields in the collection 
        /// for the current record.
        /// </summary>
        /// <param name="values">An Array of <see cref="System.Object"/> 
        /// to copy the attribute fields into.</param>
        public int GetValues(object[] values)
        {
            _accessedFields = new HashSet<string>(_lookup.Names,
                StringComparer.InvariantCultureIgnoreCase);

            return _reader.GetValues(values);
        }
        #endregion

        #region GetXxx By Column Name Methods
        /// <summary>
        /// Gets the value of the specified column 
        /// as a <see cref="System.Boolean"/>.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        public bool GetBoolean(string name)
        {
            int ordinal = GetOrdinal(name);
            MarkAccessedField(name);

            return _reader.GetBoolean(ordinal);
        }

        /// <summary>
        /// Gets the 8-bit unsigned integer value of the specified column.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        public byte GetByte(string name)
        {
            int ordinal = GetOrdinal(name);
            MarkAccessedField(name);

            return _reader.GetByte(ordinal);
        }

        /// <summary>
        /// Reads a stream of bytes from the specified column offset 
        /// into the buffer as an array, starting at the given buffer offset.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="fieldOffset">The index within the field from which 
        /// to start the read operation.</param>
        /// <param name="buffer">The buffer infto which to read the stream of bytes.</param>
        /// <param name="bufferOffset">The index for buffer to start the read operation.</param>
        /// <param name="length">The number of bytes to read.</param>        
        public long GetBytes(string name, long fieldOffset,
            byte[] buffer, int bufferOffset, int length)
        {
            int ordinal = GetOrdinal(name);
            MarkAccessedField(name);

            return _reader.GetBytes(ordinal, fieldOffset, 
                buffer, bufferOffset, length);
        }

        /// <summary>
        /// Gets the characher value of the specified column.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        public char GetChar(string name)
        {
            int ordinal = GetOrdinal(name);
            MarkAccessedField(name);

            return _reader.GetChar(ordinal);
        }

        /// <summary>
        /// Reads a stream of characters from the specified column offset 
        /// into the buffer as an array, starting at the given buffer offset.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="fieldOffset">The index within the field from which 
        /// to start the read operation.</param>
        /// <param name="buffer">The buffer infto which to read the stream of characters.</param>
        /// <param name="bufferOffset">The index for buffer to start the read operation.</param>
        /// <param name="length">The number of characters to read.</param>        
        public long GetChars(string name, long fieldOffset, 
            char[] buffer, int bufferOffset, int length)
        {
            int ordinal = GetOrdinal(name);
            MarkAccessedField(name);

            return _reader.GetChars(ordinal, fieldOffset, 
                buffer, bufferOffset, length);
        }

        /// <summary>
        /// Gets a <see cref="System.Data.IDataReader"/> 
        /// for the specified column.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        public IDataReader GetData(string name)
        {
            int ordinal = GetOrdinal(name);
            MarkAccessedField(name);

            return _reader.GetData(ordinal);
        }

        /// <summary>
        /// Gets the date and time data value of the specified column. 
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        public DateTime GetDateTime(string name)
        {
            int ordinal = GetOrdinal(name);
            MarkAccessedField(name);

            return _reader.GetDateTime(ordinal);
        }

        /// <summary>
        /// Gets the fixed-position numeric 
        /// value of the specified column. 
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        public decimal GetDecimal(string name)
        {
            int ordinal = GetOrdinal(name);
            MarkAccessedField(name);

            return _reader.GetDecimal(ordinal);
        }

        /// <summary>
        /// Gets the double-precision floating point number 
        /// value of the specified column. 
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        public double GetDouble(string name)
        {
            int ordinal = GetOrdinal(name);
            MarkAccessedField(name);

            return _reader.GetDouble(ordinal);
        }

        /// <summary>
        /// Gets the single-precision floating point number 
        /// value of the specified column. 
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        public float GetFloat(string name)
        {
            int ordinal = GetOrdinal(name);
            MarkAccessedField(name);

            return _reader.GetFloat(ordinal);
        }

        /// <summary>
        /// Gets the GUID value of the specified column. 
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        public Guid GetGuid(string name)
        {
            int ordinal = GetOrdinal(name);
            MarkAccessedField(name);

            return _reader.GetGuid(ordinal);
        }

        /// <summary>
        /// Gets the 16-bit signed integer 
        /// value of the specified column. 
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        public short GetInt16(string name)
        {
            int ordinal = GetOrdinal(name);
            MarkAccessedField(name);

            return _reader.GetInt16(ordinal);
        }

        /// <summary>
        /// Gets the 32-bit signed integer 
        /// value of the specified column.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        public int GetInt32(string name)
        {
            int ordinal = GetOrdinal(name);
            MarkAccessedField(name);

            return _reader.GetInt32(ordinal);
        }

        /// <summary>
        /// Gets the 64-bit signed integer 
        /// value of the specified column. 
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        public long GetInt64(string name)
        {
            int ordinal = GetOrdinal(name);
            MarkAccessedField(name);

            return _reader.GetInt64(ordinal);
        }

        /// <summary>
        /// Gets the string  
        /// value of the specified column. 
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        public string GetString(string name)
        {
            int ordinal = GetOrdinal(name);
            MarkAccessedField(name);

            return _reader.GetString(ordinal);
        }

        /// <summary>
        /// Gets the boxed value of the result 
        /// column which has the specified name.
        /// </summary>
        /// <param name="key">The name of the column to find.</param>
        public object GetValue(string name)
        {
            int index;
            if (_lookup.TryGetOrdinal(name, out index))
            {
                MarkAccessedField(name);
                return _reader.GetValue(index);
            }
            return null;
        }

        /// <summary>
        /// Gets the strongly-typed value of the result 
        /// column which has the specified name.
        /// </summary>
        /// <typeparam name="TValue">A type of column value.</typeparam>
        /// <param name="key">The name of the column to find.</param>
        /// <param name="defaultValue">The default value of the variable.</param>
		/// <param name="provider">An <see cref="IFormatProvider" /> interface implementation that 
		/// supplies culture-specific formatting information.</param>
        public TValue GetValue<TValue>(string name, TValue defaultValue, 
			IFormatProvider provider)
        {
            int index;
            if (_lookup.TryGetOrdinal(name, out index))
            {
                MarkAccessedField(name);
                return Converter.ChangeType<TValue>(_reader.GetValue(index), 
					defaultValue, provider);
            }
            return defaultValue;
        }
        #endregion

        #region IEnumerable Members
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region IValueSet Members
        IEnumerable<string> IValueSet.Keys
        {
            get 
            {
                return _lookup.Names;
            }
        }
        #endregion
    }
}
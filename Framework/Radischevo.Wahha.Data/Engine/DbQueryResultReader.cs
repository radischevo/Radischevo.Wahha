using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	/// <summary>
	/// Provides a means of reading one or more forward-only streams of detached result sets.
	/// </summary>
	public sealed class DbQueryResultReader : DbDataRecordBase, IDbDataReader
	{
		#region Nested Types
		private sealed class EnumeratorWrapper : IEnumerator<IDbDataRecord>, IEnumerator
		{
			#region Instance Fields
			private IEnumerator<DbQueryResultRow> _enumerator;
			#endregion

			#region Constructors
			public EnumeratorWrapper(IEnumerator<DbQueryResultRow> enumerator)
			{
				Precondition.Require(enumerator, () => 
					Error.ArgumentNull("enumerator"));
				_enumerator = enumerator;
			}
			#endregion

			#region Instance Properties
			public IDbDataRecord Current
			{
				get
				{
					return _enumerator.Current;
				}
			}
			#endregion

			#region Instance Methods
			public bool MoveNext()
			{
				return _enumerator.MoveNext();
			}

			public void Reset()
			{
				_enumerator.Reset();
			}

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			private void Dispose(bool disposing)
			{
				if (disposing)
				{
					IDisposable d = (_enumerator as IDisposable);
					if (d != null)
						d.Dispose();
				}
			}
			#endregion

			#region IEnumerator Members
			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}
			#endregion
		}
		#endregion

		#region Instance Fields
		private int _depth;
		private int _recordsAffected;
		private bool _isClosed;
		private IEnumerator<DbSubQueryResult> _resultIterator;
		private IEnumerator<DbQueryResultRow> _rowIterator;
		#endregion

		#region Constructors
		public DbQueryResultReader(IDataReader reader)
			: this(new DbQueryResult(reader))
		{
			_depth = reader.Depth;
			_recordsAffected = reader.RecordsAffected;
		}

		public DbQueryResultReader(DbQueryResult result)
			: base()
		{
			Precondition.Require(result, () => Error.ArgumentNull("result"));

			_recordsAffected = -1;
			_resultIterator = result.GetEnumerator();

			NextResult();
		}
		#endregion

		#region Instance Properties
		private DbQueryResultRow CurrentRow
		{
			get
			{
				EnsureReaderNotClosed("Read");
				DbQueryResultRow current = (_rowIterator == null)
					? null : _rowIterator.Current;

				Precondition.Require(current, () => Error.ReaderIsEmpty());
				return current;
			}
		}

		/// <summary>
		/// Gets the collection of column names, 
		/// which values were accessed by calling 
		/// value-getter methods of the current row.
		/// </summary>
		public IEnumerable<string> AccessedKeys
		{
			get
			{
				return CurrentRow.AccessedKeys;
			}
		}

		/// <summary>
		/// Gets the collection of column 
		/// names containing in the current row.
		/// </summary>
		public IEnumerable<string> Keys
		{
			get
			{
				return CurrentRow.Keys;
			}
		}

		/// <summary>
		/// Gets the number of columns in the current row.
		/// </summary>
		public int FieldCount
		{
			get
			{
				return CurrentRow.FieldCount;
			}
		}

		/// <summary>
		/// Gets a value indicating the depth 
		/// of nesting for the current row.
		/// </summary>
		public int Depth
		{
			get
			{
				return _depth;
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
				return _isClosed;
			}
		}

		/// <summary>
		/// Gets the number of rows changed, inserted, or deleted 
		/// by execution of the SQL statement.
		/// </summary>
		public int RecordsAffected
		{
			get
			{
				return _recordsAffected;
			}
		}
		#endregion

		#region Utility Methods
		private void EnsureReaderNotClosed(string methodName)
		{
			if (_isClosed)
				throw Error.DataReaderIsClosed(methodName);
		}

		/// <summary>
		/// Advances the data reader to the next result, when 
		/// reading the results of batch SQL statements.
		/// </summary>
		/// <returns></returns>
		public bool NextResult()
		{
			EnsureReaderNotClosed("NextResult");
			if (_resultIterator.MoveNext())
			{
				IEnumerable<DbQueryResultRow> current = _resultIterator.Current;
				if (current != null)
					_rowIterator = current.GetEnumerator();

				return true;
			}
			return false;
		}

		/// <summary>
		/// Closes the <see cref="Radischevo.Wahha.Data.DbQueryResultReader"/> object.
		/// </summary>
		public void Close()
		{
			if (!_isClosed)
			{
				_isClosed = true;

				IDisposable d = (_rowIterator as IDisposable);
				if (d != null)
					d.Dispose();

				d = (_resultIterator as IDisposable);
				if (d != null)
					d.Dispose();

				_rowIterator = null;
				_resultIterator = null;
			}
		}

		/// <summary>
		/// Advances the <see cref="Radischevo.Wahha.Data.DbQueryResultReader"/> to the next record.
		/// </summary>
		public bool Read()
		{
			EnsureReaderNotClosed("Read");
			if (_rowIterator == null)
				return false;

			return _rowIterator.MoveNext();
		}
		#endregion

		#region ContainsXxx Methods
		/// <summary>
		/// Returns a value indicating whether the column 
		/// with the specified name exists in the current 
		/// value set.
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		public bool ContainsKey(string key)
		{
			return CurrentRow.ContainsKey(key);
		}

		/// <summary>
		/// Determines whether the current record 
		/// contains at least one of the listed fields.
		/// </summary>
		/// <param name="keys">An array, containing 
		/// the names of the fields to find.</param>
		public bool ContainsAny(params string[] keys)
		{
			return CurrentRow.ContainsAny(keys);
		}

		/// <summary>
		/// Determines whether the current record 
		/// contains all of the listed fields.
		/// </summary>
		/// <param name="keys">An array, containing 
		/// the names of the fields to find.</param>
		public bool ContainsAll(params string[] keys)
		{
			return CurrentRow.ContainsAll(keys);
		}
		#endregion

		#region IsDBNull Methods
		/// <summary>
		/// Return whether the specified field is set to null.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		public bool IsDBNull(int ordinal)
		{
			return CurrentRow.IsDBNull(ordinal);
		}

		/// <summary>
		/// Return whether the specified field is set to null.
		/// </summary>
		/// <param name="name">The name of the column.</param>
		public bool IsDBNull(string name)
		{
			return CurrentRow.IsDBNull(name);
		}
		#endregion

		#region GetXxx By Ordinal Methods
		/// <summary>
		/// Gets the data type information for the specified field.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		public string GetDataTypeName(int ordinal)
		{
			return CurrentRow.GetDataTypeName(ordinal);
		}

		/// <summary>
		/// Gets the <see cref="System.Type"/> information corresponding to the type of 
		/// <see cref="System.Object"/> that would be returned from the field.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		public Type GetFieldType(int ordinal)
		{
			return CurrentRow.GetFieldType(ordinal);
		}

		/// <summary>
		/// Return the boxed value of the specified field.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		public override object GetValue(int ordinal)
		{
			return CurrentRow.GetValue(ordinal);
		}
		#endregion

		#region GetXxx By Name Methods
		/// <summary>
		/// Gets the data type information for the specified field.
		/// </summary>
		/// <param name="ordinal">The name of the column.</param>
		public string GetDataTypeName(string name)
		{
			return CurrentRow.GetDataTypeName(name);
		}

		/// <summary>
		/// Gets the <see cref="System.Type"/> information corresponding to the type of 
		/// <see cref="System.Object"/> that would be returned from the field.
		/// </summary>
		/// <param name="ordinal">The name of the column.</param>
		public Type GetFieldType(string name)
		{
			return CurrentRow.GetFieldType(name);
		}

		/// <summary>
		/// Return the boxed value of the specified field.
		/// </summary>
		/// <param name="name">The name of the column.</param>
		public override object GetValue(string name)
		{
			return CurrentRow.GetValue(name);
		}
		#endregion

		#region Other Instance Methods
		/// <summary>
		/// Gets the name for the field to find.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		public string GetName(int ordinal)
		{
			return CurrentRow.GetName(ordinal);
		}

		/// <summary>
		/// Return the index of the named field.
		/// </summary>
		/// <param name="name">The name of the column.</param>
		public int GetOrdinal(string name)
		{
			return CurrentRow.GetOrdinal(name);
		}

		/// <summary>
		/// Gets all the attribute fields in the collection 
		/// for the current record.
		/// </summary>
		/// <param name="values">An array of <see cref="System.Object"/> 
		/// to copy the attribute fields into.</param>
		public int GetValues(object[] values)
		{
			return CurrentRow.GetValues(values);
		}

		/// <summary>
		/// Gets the typed value with the specified key.
		/// </summary>
		/// <typeparam name="TValue">The type of value.</typeparam>
		/// <param name="key">The key to find.</param>
		/// <param name="defaultValue">The default value of the variable.</param>
		/// <param name="provider">An <see cref="IFormatProvider" /> interface implementation that 
		/// supplies culture-specific formatting information.</param>
		public TValue GetValue<TValue>(string key, TValue defaultValue, 
			IFormatProvider provider)
		{
			return CurrentRow.GetValue<TValue>(key, defaultValue, provider);
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		public IEnumerator<IDbDataRecord> GetEnumerator()
		{
			IEnumerator<DbQueryResultRow> enumerator = _rowIterator;
			if (enumerator == null)
				throw Error.ReaderIsEmpty();

			return new EnumeratorWrapper(enumerator);
		}
		#endregion

		#region IDisposable Members
		/// <summary>
		/// Performs application-defined tasks associated with 
		/// freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Close();
			GC.SuppressFinalize(this);
		}
		#endregion

		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion

		#region Unsupported Implementations
		DataTable IDataReader.GetSchemaTable()
		{
			throw Error.NotSupported();
		}

		IDataReader IDbDataRecord.GetData(string name)
		{
			throw Error.NotSupported();
		}

		IDataReader IDataRecord.GetData(int ordinal)
		{
			throw Error.NotSupported();
		}
		#endregion
	}
}

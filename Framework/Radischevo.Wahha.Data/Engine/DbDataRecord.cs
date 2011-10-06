using System;
using System.Collections.Generic;
using System.Data;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	/// <summary>
	/// Acts as a high-level decorator for the 
	/// <see cref="System.Data.IDataRecord"/> object, 
	/// extending and adding some useful functionality 
	/// to the decorated instance.
	/// </summary>
	public class DbDataRecord : DbDataRecordBase, IDbDataRecord
	{
		#region Instance Fields
		private IDataRecord _dataRecord;
		private DbFieldLookup _lookup;
		private HashSet<string> _accessedKeys;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance 
		/// of the <see cref="DbDataRecord"/> class.
		/// </summary>
		/// <param name="record">An <see cref="IDataRecord"/> 
		/// instance, which will be used to fill the value set.</param>
		internal DbDataRecord(IDataRecord record)
			: this(record, new DbFieldLookup())
		{
			UpdateLookupTable();
		}

		/// <summary>
		/// Initializes a new instance 
		/// of the <see cref="DbDataRecord"/> class.
		/// </summary>
		/// <param name="record">An <see cref="IDataRecord"/> 
		/// instance, which will be used to fill the value set.</param>
        internal DbDataRecord(IDataRecord record, DbFieldLookup lookup)
        {
			Precondition.Require(record, () => Error.ArgumentNull("record"));
			Precondition.Require(lookup, () => Error.ArgumentNull("lookup"));
			
			_dataRecord = record;
			_lookup = lookup;
			_accessedKeys = new HashSet<string>(
				StringComparer.InvariantCultureIgnoreCase);
        }
        #endregion

		#region Instance Properties
		/// <summary>
		/// Gets the number of columns 
		/// in the current row.
		/// </summary>
		public int FieldCount
		{
			get
			{
				return _dataRecord.FieldCount;
			}
		}

		/// <summary>
		/// Provides a two-way column lookup 
		/// dictionary for the result set.
		/// </summary>
		protected DbFieldLookup Lookup
		{
			get
			{
				return _lookup;
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
				return _accessedKeys;
			}
		}
		#endregion

		#region Static Methods
		private static object ConvertDbNull(object value)
		{
			return (DBNull.Value.Equals(value)) ? null : value;
		}
		#endregion

		#region DataRecord Helper Methods
		/// <summary>
		/// Marks the column with the 
		/// specified <paramref name="ordinal"/> as accessed.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		protected void MarkAccessedKey(int ordinal)
		{
			MarkAccessedKey(GetName(ordinal));
		}

		/// <summary>
		/// Marks the column with the 
		/// specified <paramref name="name"/> as accessed.
		/// </summary>
		/// <param name="name">The name of the column.</param>
		protected void MarkAccessedKey(string name)
		{
			if (!_accessedKeys.Contains(name))
				_accessedKeys.Add(name);
		}	

		/// <summary>
		/// Clears the column lookup table.
		/// </summary>
		protected void ResetLookupTable()
		{
			_lookup.Clear();
		}

		/// <summary>
		/// Clears the collection of accessed fields.
		/// </summary>
		protected void ResetAccessedKeys()
		{
			_accessedKeys.Clear();
		}

		/// <summary>
		/// Refills the column lookup table 
		/// using the current <see cref="IDataRecord"/> 
		/// instance.
		/// </summary>
		protected void UpdateLookupTable()
		{
			_lookup.Clear();

			for (int i = 0; i < _dataRecord.FieldCount; ++i)
				_lookup.Add(i, _dataRecord.GetName(i));
		}

		/// <summary>
		/// Gets the name of the field to find.
		/// </summary>
		/// <param name="ordinal">The zero-based field 
		/// ordinal to find.</param>
		public string GetName(int ordinal)
		{
			string name;
			if (!_lookup.TryGetName(ordinal, out name))
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
				return _dataRecord.IsDBNull(ordinal);

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
				return _dataRecord.IsDBNull(index);

			return true;
		}

		/// <summary>
		/// Gets the data type information
		/// for the specified column.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		public string GetDataTypeName(int ordinal)
		{
			return _dataRecord.GetDataTypeName(ordinal);
		}

		/// <summary>
		/// Gets the data type information
		/// for the specified column.
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		public string GetDataTypeName(string name)
		{
			return _dataRecord.GetDataTypeName(GetOrdinal(name));
		}

		/// <summary>
		/// Gets the <see cref="System.Type"/>, describing the specified 
		/// column value.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		public Type GetFieldType(int ordinal)
		{
			return _dataRecord.GetFieldType(ordinal);
		}

		/// <summary>
		/// Gets the <see cref="System.Type"/>, describing the specified 
		/// column value.
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		public Type GetFieldType(string name)
		{
			return _dataRecord.GetFieldType(GetOrdinal(name));
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
		/// Gets a <see cref="System.Data.IDataReader"/> 
		/// for the specified column.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		public IDataReader GetData(int ordinal)
		{
			MarkAccessedKey(ordinal);
			return _dataRecord.GetData(ordinal);
		}

		/// <summary>
		/// Gets the boxed value of the result column which has the specified ordinal.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		public override object GetValue(int ordinal)
		{
			string name;
			if (_lookup.TryGetName(ordinal, out name))
			{
				MarkAccessedKey(name);
				return _dataRecord.GetValue(ordinal);
			}
			throw Error.ColumnOrdinalDoesNotExistInResultSet(ordinal);
		}

		/// <summary>
		/// Gets the strongly-typed value of the 
		/// specified column.
		/// </summary>
		/// <typeparam name="TValue">A type of column value.</typeparam>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <param name="defaultValue">The default value of the variable.</param>
		/// <param name="provider">An <see cref="IFormatProvider" /> interface implementation that 
		/// supplies culture-specific formatting information.</param>
		public TValue GetValue<TValue>(int ordinal, TValue defaultValue,
			IFormatProvider provider)
		{
			string name;
			if (_lookup.TryGetName(ordinal, out name))
			{
				MarkAccessedKey(name);

				object value = ConvertDbNull(_dataRecord.GetValue(ordinal));
				return Converter.ChangeType<TValue>(value, defaultValue, provider);
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
			_accessedKeys = new HashSet<string>(_lookup.Names,
				StringComparer.InvariantCultureIgnoreCase);

			return _dataRecord.GetValues(values);
		}
		#endregion

		#region GetXxx By Column Name Methods
		/// <summary>
		/// Gets a <see cref="System.Data.IDataReader"/> 
		/// for the specified column.
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		public IDataReader GetData(string name)
		{
			int ordinal = GetOrdinal(name);
			MarkAccessedKey(name);

			return _dataRecord.GetData(ordinal);
		}

		/// <summary>
		/// Gets the boxed value of the result column which has the specified name.
		/// </summary>
		/// <param name="key">The name of the column to find.</param>
		public override object GetValue(string name)
		{
			int index;
			if (_lookup.TryGetOrdinal(name, out index))
			{
				MarkAccessedKey(name);
				return _dataRecord.GetValue(index);
			}
			throw Error.ColumnNameDoesNotExistInResultSet(name);
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
				MarkAccessedKey(name);

				object value = ConvertDbNull(_dataRecord.GetValue(index));
				return Converter.ChangeType<TValue>(value, defaultValue, provider);
			}
			return defaultValue;
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

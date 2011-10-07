using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	/// <summary>
	/// Represents a single row of the database query result.
	/// </summary>
	[Serializable]
	public class DbQueryResultRow : DbDataRecordBase, IDbDataRecord
	{
		#region Instance Fields
		private DbFieldLookup _lookup;
		private HashSet<string> _accessedKeys;
		private string[] _dataTypes;
		private object[] _values;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="Radischevo.Wahha.Data.DbQueryResultRow"/> class.
		/// </summary>
		/// <param name="lookup">The result field lookup helper.</param>
		/// <param name="record">The data record to extract values from.</param>
		public DbQueryResultRow(DbFieldLookup lookup, IDataRecord record) 
			: base()
		{
			Precondition.Require(lookup, () => Error.ArgumentNull("lookup"));
			Precondition.Require(record, () => Error.ArgumentNull("record"));

			int fieldCount = record.FieldCount;

			_lookup = lookup;
			_values = new object[fieldCount];
			_dataTypes = new string[fieldCount];
			_accessedKeys = new HashSet<string>();

			record.GetValues(_values);

			for (int i = 0; i < fieldCount; ++i)
				_dataTypes[i] = record.GetDataTypeName(i);
		}
		#endregion

		#region Instance Properties
		/// <summary>
		/// Gets the number of fields in the current row.
		/// </summary>
		public int FieldCount
		{
			get
			{
				return _values.Length;
			}
		}

		/// <summary>
		/// Gets the collection of field names, which 
		/// values were retrieved.
		/// </summary>
		public IEnumerable<string> AccessedKeys
		{
			get
			{
				return _accessedKeys;
			}
		}

		/// <summary>
		/// Gets the collection of field names contained 
		/// in the current row.
		/// </summary>
		public IEnumerable<string> Keys
		{
			get
			{
				return _lookup.Names;
			}
		}
		#endregion

		#region Static Methods
		private static object ConvertDbNull(object value)
		{
			return (Convert.IsDBNull(value)) ? null : value;
		}
		#endregion

		#region Utility Methods
		protected void MarkAccessedKey(string name)
		{
			if (!_accessedKeys.Contains(name))
				_accessedKeys.Add(name);
		}

		protected void MarkAccessedKey(int ordinal)
		{
			string name;
			if (_lookup.TryGetName(ordinal, out name))
				MarkAccessedKey(name);
		}
		#endregion

		#region ContainsXxx Methods
		/// <summary>
		/// Gets the value indicating whether the 
		/// specified field exists in the row.
		/// </summary>
		/// <param name="key">The name of the field to find.</param>
		public bool ContainsKey(string key)
		{
			return _lookup.Contains(key);
		}

		/// <summary>
		/// Gets the value indicating whether the row 
		/// contains any of the specified fields.
		/// </summary>
		/// <param name="keys">The collection of field names.</param>
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
		/// Gets the value indicating whether the row 
		/// contains all of the specified fields.
		/// </summary>
		/// <param name="keys">The collection of field names.</param>
		public bool ContainsAll(params string[] keys)
		{
			if (keys == null || keys.Length < 1)
				return true;

			foreach (string key in keys)
				if (!ContainsKey(key))
					return false;

			return true;
		}
		#endregion

		#region GetXxx By Name Methods
		public string GetDataTypeName(string name)
		{
			int ordinal;
			if (_lookup.TryGetOrdinal(name, out ordinal))
				return _dataTypes[ordinal];

			throw Error.ColumnNameDoesNotExistInResultSet(name);
		}
		
		public Type GetFieldType(string name)
		{
			object value = GetValue(name);
			return (value == null) ? typeof(object) : value.GetType();
		}

		public override object GetValue(string name)
		{
			int ordinal;
			if (_lookup.TryGetOrdinal(name, out ordinal))
			{
				MarkAccessedKey(name);
				return ConvertDbNull(_values[ordinal]);
			}
			throw Error.ColumnNameDoesNotExistInResultSet(name);
		}
		#endregion

		#region GetXxx By Ordinal Methods
		public string GetDataTypeName(int ordinal)
		{
			Precondition.Require(ordinal > -1, () =>
				Error.ParameterMustBeGreaterThanOrEqual("ordinal", 0, ordinal));

			Precondition.Require(ordinal < _values.Length, () =>
				Error.ColumnOrdinalDoesNotExistInResultSet(ordinal));

			return _dataTypes[ordinal];
		}
		
		public Type GetFieldType(int ordinal)
		{
			object value = GetValue(ordinal);
			return (value == null) ? typeof(object) : value.GetType();
		}
		
		public override object GetValue(int ordinal)
		{
			Precondition.Require(ordinal > -1, () =>
				Error.ParameterMustBeGreaterThanOrEqual("ordinal", 0, ordinal));

			Precondition.Require(ordinal < _values.Length, () =>
				Error.ColumnOrdinalDoesNotExistInResultSet(ordinal));

			MarkAccessedKey(ordinal);
			return ConvertDbNull(_values[ordinal]);
		}
		#endregion

		#region IsDBNull Methods
		public bool IsDBNull(int ordinal)
		{
			if (_lookup.Contains(ordinal))
				return Convert.IsDBNull(GetValue(ordinal));

			return true;
		}

		public bool IsDBNull(string name)
		{
			if (_lookup.Contains(name))
				return Convert.IsDBNull(GetValue(name));

			return true;
		}
		#endregion

		#region Other Instance Methods
		/// <summary>
		/// Gets the name for the field to find.
		/// </summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		public string GetName(int ordinal)
		{
			string name;
			if (_lookup.TryGetName(ordinal, out name))
				return name;

			throw Error.ColumnOrdinalDoesNotExistInResultSet(ordinal);
		}

		/// <summary>
		/// Return the index of the named field.
		/// </summary>
		/// <param name="name">The name of the column.</param>
		public int GetOrdinal(string name)
		{
			int ordinal;
			if (_lookup.TryGetOrdinal(name, out ordinal))
				return ordinal;

			throw Error.ColumnNameDoesNotExistInResultSet(name);
		}

		/// <summary>
		/// Gets all the attribute fields in the collection 
		/// for the current record.
		/// </summary>
		/// <param name="values">An array of <see cref="System.Object"/> 
		/// to copy the attribute fields into.</param>
		public int GetValues(object[] values)
		{
			Precondition.Require(values, () => Error.ArgumentNull("values"));

			_accessedKeys = new HashSet<string>(_lookup.Names,
				StringComparer.InvariantCultureIgnoreCase);

			int count = Math.Min(_values.Length, values.Length);
			Array.Copy(_values, values, count);

			return count;
		}

		/// <summary>
		/// Gets the typed value with the specified key.
		/// </summary>
		/// <typeparam name="TValue">The type of value.</typeparam>
		/// <param name="key">The key to find.</param>
		/// <param name="defaultValue">The default value of the variable.</param>
		/// <param name="provider">An <see cref="IFormatProvider" /> interface implementation that 
		/// supplies culture-specific formatting information.</param>
		public TValue GetValue<TValue>(string key, 
			TValue defaultValue, IFormatProvider provider)
		{
			int ordinal;
			if (_lookup.TryGetOrdinal(key, out ordinal))
			{
				MarkAccessedKey(key);
				object value = ConvertDbNull(_values[ordinal]);

				return Converter.ChangeType<TValue>(value, defaultValue, provider);
			}
			return defaultValue;
		}
		#endregion

		#region Unsupported Implementations
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

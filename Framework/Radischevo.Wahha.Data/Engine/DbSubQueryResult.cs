﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	/// <summary>
	/// Represents a database query result.
	/// </summary>
	[Serializable]
	public class DbSubQueryResult : IEnumerable<DbQueryResultRow>
	{
		#region Instance Fields
		private ICollection<DbQueryResultRow> _rows;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="Radischevo.Wahha.Data.DbSubQueryResult"/> class.
		/// </summary>
		/// <param name="reader">The data reader to extract results from.</param>
		public DbSubQueryResult(IDataReader reader)
		{
			Precondition.Require(reader, () => 
				Error.ArgumentNull("reader"));

			_rows = CreateResultSet(reader);
		}
		#endregion

		#region Instance Properties
		/// <summary>
		/// Gets the number of rows in the 
		/// current result set.
		/// </summary>
		public virtual int RowCount
		{
			get
			{
				return _rows.Count;
			}
		}
		#endregion

		#region Static Methods
		private static DbFieldLookup CreateLookupTable(IDataRecord record)
		{
			DbFieldLookup lookup = new DbFieldLookup();
			for (int i = 0; i < record.FieldCount; ++i)
				lookup.Add(i, record.GetName(i));

			return lookup;
		}

		private static ICollection<DbQueryResultRow> CreateResultSet(IDataReader reader)
		{
			List<DbQueryResultRow> rows = new List<DbQueryResultRow>();
			if (reader.Read())
			{
				DbFieldLookup lookup = CreateLookupTable(reader);
				do
				{
					rows.Add(new DbQueryResultRow(lookup, reader));
				}
				while (reader.Read());
			}
			return rows;
		}
		#endregion

		#region Instance Methods
		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		public virtual IEnumerator<DbQueryResultRow> GetEnumerator()
		{
			return _rows.GetEnumerator();
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	/// <summary>
	/// Represents the combined result set of a database query.
	/// </summary>
	[Serializable]
	public sealed class DbQueryResult : IEnumerable<DbSubQueryResult>
	{
		#region Instance Fields
		private ICollection<DbSubQueryResult> _results;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="Radischevo.Wahha.Data.DbQueryResult"/> class.
		/// </summary>
		/// <param name="reader">The data reader to extract results from.</param>
		public DbQueryResult(IDataReader reader)
		{
			Precondition.Require(reader, () => 
				Error.ArgumentNull("reader"));

			_results = CreateResultSet(reader);
		}
		#endregion

		#region Instance Properties
		/// <summary>
		/// Gets the number of independent subquery 
		/// results in the result set.
		/// </summary>
		public int Count
		{
			get
			{
				return _results.Count;
			}
		}
		#endregion

		#region Static Methods
		private static ICollection<DbSubQueryResult> CreateResultSet(IDataReader reader)
		{
			List<DbSubQueryResult> results = new List<DbSubQueryResult>();
			do
			{
				results.Add(new DbSubQueryResult(reader));
			}
			while (reader.NextResult());
			return results;
		}
		#endregion

		#region Instance Methods
		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		public IEnumerator<DbSubQueryResult> GetEnumerator()
		{
			return _results.GetEnumerator();
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

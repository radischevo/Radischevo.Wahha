using System;
using System.Data;
using System.Data.Odbc;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Provider
{
	/// <summary>
	/// Provides methods to access 
	/// ODBC compliant databases.
	/// </summary>
	public class OdbcDbDataProvider : CommonDbDataProvider
	{
		#region Constructors
		/// <summary>
		/// Creates a new instance of 
		/// the <see cref="T:Radischevo.Wahha.Data.Provider.OdbcDataProvider"/> class.
		/// </summary>
		/// <param name="connectionString">The database connection string.</param>
		public OdbcDbDataProvider(string connectionString)
			: base(connectionString)
		{
		}
		#endregion

		#region Instance Methods
		/// <summary>
		/// Creates an <see cref="T:System.Data.IDbConnection"/> instance 
		/// which is supported by the current provider.
		/// </summary>
		/// <param name="connectionString">The database connection string.</param>
		protected override IDbConnection CreateConnection(string connectionString)
		{
			return new OdbcConnection(connectionString);
		}

		/// <summary>
		/// Creates a <see cref="T:System.Data.IDbDataAdapter"/> instance, 
		/// which is supported by the current provider.
		/// </summary>
		public override IDbDataAdapter CreateDataAdapter()
		{
			return new OdbcDataAdapter();
		}
		#endregion
	}
}
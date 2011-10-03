using System;
using System.Data;
using System.Data.SqlClient;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Provider
{
    /// <summary>
    /// Provides methods to access 
    /// Microsoft SQL Server databases.
    /// </summary>
    public class SqlDbDataProvider : CommonDbDataProvider
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of 
        /// the <see cref="T:Radischevo.Wahha.Data.Provider.SqlDbDataProvider"/> class.
        /// </summary>
		/// <param name="connectionString">The database connection string.</param>
        public SqlDbDataProvider(string connectionString)
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
			return new SqlConnection(connectionString);
		}

        /// <summary>
        /// Creates a <see cref="T:System.Data.IDbDataAdapter"/> instance, 
        /// which is supported by the current provider.
        /// </summary>
        public override IDbDataAdapter CreateDataAdapter()
        {
            return new SqlDataAdapter();
        }
        #endregion
	}
}

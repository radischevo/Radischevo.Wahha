using System;
using System.Data;
using System.Data.OleDb;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Provider
{
    /// <summary>
    /// Provides methods to access 
    /// Microsoft Jet databases.
    /// </summary>
    public class OleDbDataProvider : CommonDbDataProvider
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of 
        /// the <see cref="T:Radischevo.Wahha.Data.Provider.OleDbDataProvider"/> class.
        /// </summary>
		/// <param name="connectionString">The database connection string.</param>
        public OleDbDataProvider(string connectionString) 
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
			return new OleDbConnection(connectionString);
		}

		/// <summary>
        /// Creates a <see cref="T:System.Data.IDbDataAdapter"/> instance, 
        /// which is supported by the current provider.
        /// </summary>
        public override IDbDataAdapter CreateDataAdapter()
        {
            return new OleDbDataAdapter();
        }
        #endregion
	}
}

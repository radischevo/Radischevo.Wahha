using System;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data.Caching;

namespace Radischevo.Wahha.Data
{
	public abstract class DbOperationContext
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="Radischevo.Wahha.Data.DbOperationContext"/> class.
		/// </summary>
		protected DbOperationContext()
		{
		}
		#endregion

		#region Instance Properties
		/// <summary>
		/// When overridden in a derived class, 
		/// gets the <see cref="Radischevo.Wahha.Data.IDbDataProvider"/> 
		/// used to perform database queries.
		/// </summary>
		public abstract IDbDataProvider DataProvider
		{
			get;
		}

		/// <summary>
		/// When overridden in a derived class, 
		/// gets the <see cref="Radischevo.Wahha.Data.Caching.IScopedCacheProvider"/> 
		/// used to access the application cache within a scope.
		/// </summary>
		public abstract ITaggedCacheProvider CacheProvider
		{
			get;
		}
		#endregion
	}
}

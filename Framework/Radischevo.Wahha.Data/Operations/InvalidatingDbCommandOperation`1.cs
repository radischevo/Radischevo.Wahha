using System;
using System.Collections.Generic;

using Radischevo.Wahha.Data.Caching;

namespace Radischevo.Wahha.Data
{
	/// <summary>
	/// Defines the base class for the operation, that executes 
	/// an SQL statement against the database and invalidates 
	/// the result cache storage.
	/// </summary>
	/// <typeparam name="TResult">The type of the result.</typeparam>
	public abstract class InvalidatingDbCommandOperation<TResult> : DbCommandOperation<TResult>
	{
		#region Instance Fields
		private ITaggedCacheProvider _cache;
		private List<string> _tags;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Radischevo.Wahha.Data.InvalidatingDbCommandOperation{TResult}"/> class.
		/// </summary>
		protected InvalidatingDbCommandOperation()
			: base()
		{
			_tags = new List<string>();
		}
		#endregion

		#region Instance Properties
		/// <summary>
		/// Gets or sets the caching provider used 
		/// to store the query results.
		/// </summary>
		protected ITaggedCacheProvider Cache
		{
			get
			{
				if (_cache == null)
					_cache = CacheProvider.Instance;

				return _cache;
			}
			set
			{
				_cache = value;
			}
		}

		/// <summary>
		/// Gets the collection of tags which 
		/// will be invalidated.
		/// </summary>
		public ICollection<string> Tags
		{
			get
			{
				return _tags;
			}
		}
		#endregion

		#region Instance Methods
		/// <summary>
		/// Executes the operation against the provided data source 
		/// and returns the result.
		/// </summary>
		/// <param name="provider">The database communication provider 
		/// using to retrieve or store the data.</param>
		public override TResult Execute(IDbDataProvider provider)
		{
			try
			{
				return base.Execute(provider);
			}
			finally
			{
				Cache.Invalidate(Tags);
			}
		}
		#endregion
	}
}

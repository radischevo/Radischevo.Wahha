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
		/// <param name="context">Provides the current operation context.</param>
		protected override TResult ExecuteInternal(DbOperationContext context)
		{
			try
			{
				return base.ExecuteInternal(context);
			}
			finally
			{
				context.CacheProvider.Invalidate(Tags);
			}
		}
		#endregion
	}
}

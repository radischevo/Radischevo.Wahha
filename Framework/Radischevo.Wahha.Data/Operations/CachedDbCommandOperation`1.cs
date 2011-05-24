using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data.Caching;

namespace Radischevo.Wahha.Data
{
	/// <summary>
	/// Defines the base class for the operation, that queries the database 
	/// converts the result to the specified type and caches it in the cache storage.
	/// </summary>
	/// <typeparam name="TResult">The type of the result.</typeparam>
	public abstract class CachedDbCommandOperation<TResult> : DbCommandOperation<TResult>
	{
		#region Instance Fields
		private ITaggedCacheProvider _cache;
		private TimeSpan _expirationTimeout;
		private List<string> _tags;
		private bool _enableCaching;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Radischevo.Wahha.Data.CachedDbCommandOperation{TResult}"/> class.
		/// </summary>
		protected CachedDbCommandOperation()
			: base()
		{
			_tags = new List<string>();
			_enableCaching = true;
		}
		#endregion

		#region Instance Properties
		/// <summary>
		/// Gets or sets the caching provider used 
		/// to store the query result.
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
		/// is used to mark cache storage entries.
		/// </summary>
		public ICollection<string> Tags
		{
			get
			{
				return _tags;
			}
		}

		/// <summary>
		/// Gets or sets the amount of time after 
		/// which the entry will be removed from cache.
		/// </summary>
		public TimeSpan ExpirationTimeout
		{
			get
			{
				return _expirationTimeout;
			}
			set
			{
				_expirationTimeout = value;
			}
		}

		/// <summary>
		/// Gets or sets the value indicating whether 
		/// the caching of results is enabled.
		/// </summary>
		public bool EnableCaching
		{
			get
			{
				return (_enableCaching && Cache != null);
			}
			set
			{
				_enableCaching = value;
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
			if (EnableCaching)
			{
				string cacheKey = CreateCacheKey(Command);
				return Cache.Get<TResult>(cacheKey,
					() => base.Execute(provider),
					DateTime.Now.Add(_expirationTimeout),
					Tags);
			}
			return base.Execute(provider);
		}

		/// <summary>
		/// Creates the unique cache key based on the text representation 
		/// of the provided command and its parameters values.
		/// </summary>
		/// <param name="command">The command to create fingerprint from.</param>
		protected virtual string CreateCacheKey(DbCommandDescriptor command)
        {
            StringBuilder sb = new StringBuilder();
            MD5 md5 = MD5.Create();
            Encoding encoding = Encoding.UTF8;

			sb.Append(GetType().FullName).Append("::")
				.Append(command.Text).Append("::")
                .Append(command.Type.ToString());

            sb.Append("=>{");

            foreach (DbParameterDescriptor parameter in command.Parameters)
            {
                sb.Append("[").Append(parameter.Name).Append("=")
                    .Append((parameter.Value == DBNull.Value) ? "<NULL>" : 
                        Convert.ToString(parameter.Value, CultureInfo.InvariantCulture))
                    .Append("]");
            }
            sb.Append("}");
            return Converter.ToBase16String(md5.ComputeHash(encoding.GetBytes(sb.ToString())));
        }
		#endregion
	}
}

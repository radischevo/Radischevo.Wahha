using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data.Caching;

namespace Radischevo.Wahha.Data
{
    public abstract class DbRepository<TEntity> 
        : IRepository<TEntity>, IDbQueryService<TEntity>
    {
        #region Instance Fields
        private ITaggedCacheProvider _cache;
        private TimeSpan _expirationTimeout;
        private bool _enableCaching;
        private List<string> _tags;
        #endregion

        #region Constructors
        protected DbRepository(params string[] tags)
        {
            Precondition.Require(tags, Error.ArgumentNull("tags"));
            _tags = new List<string>(tags);
            _enableCaching = true;
        }
        #endregion

        #region Instance Properties
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

        public ICollection<string> Tags
        {
            get
            {
                return _tags;
            }
        }

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

        #region Abstract Methods
        protected abstract IEnumerable<TEntity> ExecuteSelect(DbCommandDescriptor command);

        protected abstract TEntity ExecuteSingle(DbCommandDescriptor command);

        protected abstract TEntity ExecuteSave(TEntity entity);

        protected abstract TEntity ExecuteDelete(TEntity entity);
        #endregion

        #region Helper Methods
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

        #region Instance Methods
        public IEnumerable<TEntity> Select(DbCommandDescriptor command)
        {
            return Select(command, null);
        }

        public virtual IEnumerable<TEntity> Select(DbCommandDescriptor command, 
            params string[] tags)
        {
            Precondition.Require(command, Error.ArgumentNull("command"));
            if (EnableCaching)
            {
                string cacheKey = CreateCacheKey(command);
				IEnumerable<string> concreteTags = _tags.Concat(tags ?? Enumerable.Empty<string>());

                return Cache.Get<IEnumerable<TEntity>>(cacheKey, 
                    () => ExecuteSelect(command), DateTime.Now.Add(_expirationTimeout),
                        concreteTags);
            }
            return ExecuteSelect(command);
        }

        public TEntity Single(DbCommandDescriptor command)
        {
            return Single(command, null);
        }

        public virtual TEntity Single(DbCommandDescriptor command, params string[] tags)
        {
            Precondition.Require(command, Error.ArgumentNull("command"));
            if (EnableCaching)
            {
                string cacheKey = CreateCacheKey(command);
				IEnumerable<string> concreteTags = _tags.Concat(tags ?? Enumerable.Empty<string>());

                return Cache.Get<TEntity>(cacheKey, 
                    () => ExecuteSingle(command), DateTime.Now.Add(_expirationTimeout),
                        concreteTags);
            }
            return ExecuteSingle(command);
        }

        public virtual TEntity Save(TEntity entity)
        {
            try
            {
                return ExecuteSave(entity);
            }
            finally
            {
                if(EnableCaching)
                    Cache.Invalidate(_tags.ToArray());
            }
        }

        public virtual TEntity Delete(TEntity entity)
        {
            try
            {
                return ExecuteDelete(entity);
            }
            finally
            {
                if(EnableCaching)
                    Cache.Invalidate(_tags.ToArray());
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
        #endregion
    }

    public abstract class DbRepository<TEntity, TKey> 
        : DbRepository<TEntity>, IRepository<TEntity, TKey>
    {
        #region Constructors
        protected DbRepository(params string[] tags)
            : base(tags)
        {
        }
        #endregion

        #region Abstract Methods
        protected abstract TEntity ExecuteSingle(TKey key);
        #endregion

        #region Helper Methods
        protected virtual string CreateCacheKey(TKey key)
        {
            StringBuilder sb = new StringBuilder();
            MD5 md5 = MD5.Create();
            Encoding encoding = Encoding.UTF8;

            sb.Append(GetType().FullName).Append("::")
                .Append(typeof(TKey).Name)
				.Append("::Single");

            sb.Append("=>{Key=").Append((Object.ReferenceEquals(key, null)) ? "<NULL>" :
                Convert.ToString(key, CultureInfo.InvariantCulture)).Append("}");

            return Converter.ToBase16String(md5.ComputeHash(encoding.GetBytes(sb.ToString())));
        }
        #endregion

        #region Instance Methods
        public TEntity Single(TKey key)
        {
            return Single(key, null);
        }

        public virtual TEntity Single(TKey key, params string[] tags)
        {
            if (EnableCaching)
            {
                string cacheKey = CreateCacheKey(key);
				IEnumerable<string> concreteTags = Tags.Concat(tags ?? Enumerable.Empty<string>());

                return Cache.Get<TEntity>(cacheKey, 
                    () => ExecuteSingle(key), DateTime.Now.Add(ExpirationTimeout),
                        concreteTags);
            }
            return ExecuteSingle(key);
        }
        #endregion
    }
}

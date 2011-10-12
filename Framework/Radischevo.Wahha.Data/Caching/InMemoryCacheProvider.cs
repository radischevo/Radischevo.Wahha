using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Caching
{
	public class InMemoryCacheProvider : ITaggedCacheProvider, IDisposable
	{
		#region Nested Types
		private sealed class CacheTag : IEquatable<CacheTag>
		{
			#region Instance Fields
			private string _value;
			private long _version;
			#endregion

			#region Constructors
			public CacheTag(string value)
				: this(value, 0)
			{
			}

			public CacheTag(string value, long version)
			{
				Precondition.Defined(value, () =>
					Error.ArgumentNull("value"));

				_value = value;
				_version = version;
			}
			#endregion

			#region Instance Properties
			public string Value
			{
				get
				{
					return _value;
				}
			}

			public long Version
			{
				get
				{
					return _version;
				}
				set
				{
					_version = value;
				}
			}
			#endregion

			#region Instance Methods
			public override int GetHashCode()
			{
				int h1 = _value.GetHashCode();
				int h2 = _version.GetHashCode();

				return (((h1 << 5) + h1) ^ h2);
			}

			public override bool Equals(object obj)
			{
				return Equals(obj as CacheTag);
			}
			
			public bool Equals(CacheTag other)
			{
				if (!String.Equals(_value, other._value, 
					StringComparison.Ordinal))
					return false;

				return _version.Equals(other._version);
			}

			public override string ToString()
			{
				return String.Concat("tag:", _value);
			}
			#endregion
		}

		private sealed class CacheEntry : IEquatable<CacheEntry>
		{
			#region Instance Fields
			private string _key;
			private int _hashCode;
			private object _value;
			private IEnumerable<CacheTag> _tags;
			private DateTime _expires;
			#endregion

			#region Constructors
			public CacheEntry(string key, object value, DateTime expiration, 
				IEnumerable<CacheTag> tags)
			{
				Precondition.Defined(key, () => Error.ArgumentNull("key"));
				_key = key;
				_hashCode = _key.GetHashCode();
				_value = value;
				_tags = tags ?? Enumerable.Empty<CacheTag>();
				_expires = expiration;
			}
			#endregion

			#region Instance Properties
			public string Key
			{
				get
				{
					return _key;
				}
			}

			public object Value
			{
				get
				{
					return _value;
				}
			}

			public IEnumerable<CacheTag> Tags
			{
				get
				{
					return _tags;
				}
			}

			public DateTime Expires
			{
				get
				{
					return _expires;
				}
			}
			#endregion

			#region Instance Methods
			public override bool Equals(object obj)
			{
				return Equals(obj as CacheEntry);
			}

			public bool Equals(CacheEntry other)
			{
				if (other == null)
					return false;
				
				if (_hashCode != other._hashCode)
					return false;

				return String.Equals(_key, other.Key, StringComparison.Ordinal);
			}

			public override int GetHashCode()
			{
				return _hashCode;
			}
			#endregion
		}
		#endregion

		#region Instance Fields
		private Dictionary<string, CacheTag> _tags;
		private Dictionary<string, CacheEntry> _entries;
		private ReaderWriterLockSlim _entriesLock;
		private ReaderWriterLockSlim _tagsLock;
		private Func<DateTime> _dateAccessor;
		#endregion

		#region Constructors
		public InMemoryCacheProvider()
		{
			_entries = new Dictionary<string, CacheEntry>();
			_tags = new Dictionary<string, CacheTag>();
			_entriesLock = new ReaderWriterLockSlim();
			_tagsLock = new ReaderWriterLockSlim();
			_dateAccessor = () => DateTime.Now;
		}
		#endregion

		#region Instance Properties
		public virtual int Count
		{
			get
			{
				try
				{
					_entriesLock.EnterReadLock();
					return _entries.Count;
				}
				finally
				{
					_entriesLock.ExitReadLock();
				}
			}
		}
		#endregion

		#region Instance Methods
		private IEnumerable<CacheTag> CreateEntryTags(IEnumerable<string> values)
		{
			List<CacheTag> tags = new List<CacheTag>();
			if (values == null)
				return tags;

			_tagsLock.EnterUpgradeableReadLock();
			try
			{
				DateTime time = _dateAccessor();
				foreach (string value in values)
				{
					CacheTag tag;
					if (!_tags.TryGetValue(value, out tag))
					{
						tag = new CacheTag(value);
						tag.Version = time.ToFileTimeUtc();

						_tagsLock.EnterWriteLock();
						try
						{
							_tags.Add(value, tag);
						}
						finally
						{
							_tagsLock.ExitWriteLock();
						}
					}
					tags.Add(tag);
				}
			}
			finally
			{
				_tagsLock.ExitUpgradeableReadLock();
			}
			return tags;
		}

		private bool ValidateEntryTags(CacheEntry entry)
		{
			_tagsLock.EnterReadLock();
			try
			{
				foreach (CacheTag tag in entry.Tags)
				{
					CacheTag current;
					if (_tags.TryGetValue(tag.Value, out current))
					{
						if (current.Version > tag.Version)
							return false;
					}
				}
			}
			finally
			{
				_tagsLock.ExitReadLock();
			}
			return true;
		}

		private bool ValidateEntryExpiration(CacheEntry entry)
		{
			DateTime time = _dateAccessor();
			if (entry.Expires <= time)
				return false;

			return ValidateEntryTags(entry);
		}

		private bool TryGetEntryAndValidate(string key, out CacheEntry entry)
		{
			if (_entries.TryGetValue(key, out entry))
			{
				if (ValidateEntryExpiration(entry))
					return true;
			}
			return false;
		}

		protected virtual void Init(IValueSet settings)
		{
		}

		public virtual T Get<T>(string key)
		{
			Precondition.Defined(key, () => Error.ArgumentNull("key"));

			_entriesLock.EnterReadLock();
			try
			{
				CacheEntry entry;
				if (_entries.TryGetValue(key, out entry))
					return Converter.ChangeType<T>(entry.Value);
				
				return default(T);
			}
			finally
			{
				_entriesLock.ExitReadLock();
			}
		}

		public T Get<T>(string key, CacheItemSelector<T> selector, DateTime expiration)
		{
			return Get(key, selector, expiration, null);
		}

		public virtual T Get<T>(string key, CacheItemSelector<T> selector, 
			DateTime expiration, IEnumerable<string> tags)
		{
			Precondition.Defined(key, () => Error.ArgumentNull("key"));
			Precondition.Require(selector, () => Error.ArgumentNull("selector"));

			_entriesLock.EnterUpgradeableReadLock();
			try
			{
				CacheEntry entry;
				if (!TryGetEntryAndValidate(key, out entry))
				{
					_entriesLock.EnterWriteLock();
					try
					{
						if (!TryGetEntryAndValidate(key, out entry))
							_entries[key] = entry = new CacheEntry(key, selector(),
								expiration, CreateEntryTags(tags));
					}
					finally
					{
						_entriesLock.ExitWriteLock();
					}
				}
				return Converter.ChangeType<T>(entry.Value);
			}
			finally
			{
				_entriesLock.ExitUpgradeableReadLock();
			}
		}

		public bool Add<T>(string key, T value, DateTime expiration)
		{
			return Add(key, value, expiration, null);
		}

		public virtual bool Add<T>(string key, T value, DateTime expiration, IEnumerable<string> tags)
		{
			Precondition.Defined(key, () => Error.ArgumentNull("key"));
			_entriesLock.EnterWriteLock();
			try
			{
				CacheEntry entry;
				if (!TryGetEntryAndValidate(key, out entry))
				{
					_entries[key] = entry = new CacheEntry(key, value,
						expiration, CreateEntryTags(tags));

					return true;
				}
				return false;
			}
			finally
			{
				_entriesLock.ExitWriteLock();
			}
		}

		public void Insert<T>(string key, T value, DateTime expiration)
		{
			Insert(key, value, expiration, null);
		}

		public virtual void Insert<T>(string key, T value, DateTime expiration, IEnumerable<string> tags)
		{
			Precondition.Defined(key, () => Error.ArgumentNull("key"));
			_entriesLock.EnterWriteLock();
			try
			{
				_entries[key] = new CacheEntry(key, value, 
					expiration, CreateEntryTags(tags));
			}
			finally
			{
				_entriesLock.ExitWriteLock();
			}
		}

		public virtual void Remove(string key)
		{
			Precondition.Defined(key, () => Error.ArgumentNull("key"));
			_entriesLock.EnterWriteLock();
			try
			{
				_entries.Remove(key);
			}
			finally
			{
				_entriesLock.ExitWriteLock();
			}
		}

		public virtual void Invalidate(IEnumerable<string> tags)
		{
			Precondition.Require(tags, () => Error.ArgumentNull("tags"));
			_tagsLock.EnterWriteLock();

			try
			{
				DateTime time = _dateAccessor();
				foreach (string value in tags)
					_tags[value] = new CacheTag(value, time.ToFileTimeUtc());
			}
			finally
			{
				_tagsLock.ExitWriteLock();
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_entriesLock.Dispose();
				_tagsLock.Dispose();
			}
		}
		#endregion

		#region ICacheProvider Members
		void ICacheProvider.Init(IValueSet settings)
		{
			Init(settings);
		}
		#endregion
	}
}

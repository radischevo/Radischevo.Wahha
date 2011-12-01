using System;
using System.Web.Hosting;
using System.Threading;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Templates
{
    /// <summary>
    /// This class caches the result of VirtualPathProvider.FileExists for a short
    /// period of time, and recomputes it if necessary.
    /// 
    /// The default MapPathBasedVirtualPathProvider caches the result of
    /// the FileExists call with the appropriate dependencies, so it is less
    /// expensive on subsequent calls, but it still needs to do MapPath which can 
    /// take quite some time.
    /// </summary>
    public sealed class FileExistenceCache
	{
		#region Nested Types
		private sealed class CachedStore : ReaderWriterCache<string, bool>
		{
			#region Constructors
			public CachedStore()
				: base(StringComparer.OrdinalIgnoreCase)
			{
			}
			#endregion

			#region Instance Methods
			public bool FileExists(VirtualPathProvider provider, string virtualPath)
			{
				return base.GetOrCreate(virtualPath, () => provider.FileExists(virtualPath));
			}
			#endregion
		}
		#endregion

		#region Constants
		private const int TickPerMiliseconds = 10000;
		#endregion

		#region Static Fields
		private static readonly FileExistenceCache _instance = new FileExistenceCache();
		#endregion

		#region Instance Fields
		private long _creationTick;
		private int _ticksBeforeReset;
		private VirtualPathProvider _provider;
		private CachedStore _cache;
		#endregion

		#region Constructors
		private FileExistenceCache()
		{
			Duration = 1000;
			Reset();
		}
		#endregion

		#region Static Properties
		public static FileExistenceCache Instance
		{
			get
			{
				return _instance;
			}
		}
		#endregion

		#region Instance Properties
		public VirtualPathProvider Provider
		{
			get
			{
				return _provider ?? HostingEnvironment.VirtualPathProvider;
			}
			set
			{
				_provider = value;
			}
		}

		public int Duration
		{
			get
			{
				return _ticksBeforeReset / TickPerMiliseconds;
			}
			set
			{
				_ticksBeforeReset = value * TickPerMiliseconds;
			}
		}

		public bool TimeExceeded
		{
			get
			{
				return ((DateTime.UtcNow.Ticks - Interlocked
					.Read(ref _creationTick)) > _ticksBeforeReset);
			}
		}
		#endregion

		#region Instance Methods
		public void Reset()
		{
			_cache = new CachedStore();
			long tick = DateTime.UtcNow.Ticks;
			Interlocked.Exchange(ref _creationTick, tick);
		}

		public bool FileExists(string virtualPath)
		{
			if (TimeExceeded)
				Reset();

			return _cache.FileExists(Provider, virtualPath);
		}
		#endregion
    }
}
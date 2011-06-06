using System;
using System.Collections.Concurrent;
using System.Web.Hosting;
using System.Threading;

namespace Radischevo.Wahha.Web.Mvc.Html.Razor
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
		private ConcurrentDictionary<string, bool> _cache;
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
			_cache = new ConcurrentDictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
			long tick = DateTime.UtcNow.Ticks;
			Interlocked.Exchange(ref _creationTick, tick);
		}

		public bool FileExists(string virtualPath)
		{
			if (TimeExceeded)
				Reset();

			bool exists;
			if (!_cache.TryGetValue(virtualPath, out exists))
				_cache.TryAdd(virtualPath, exists = Provider.FileExists(virtualPath));

			return exists;
		}
		#endregion
    }
}
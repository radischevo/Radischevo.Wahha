using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Compilation;
using System.Web.Hosting;

namespace Radischevo.Wahha.Web.Mvc.Html.Razor 
{
    public class VirtualPathFactoryManager
	{
		#region Static Fields
		private static VirtualPathFactoryManager _instance;
		private static object _lock = new object();
		#endregion

		#region Instance Fields
		private VirtualPathProvider _provider;
		private Func<string, Type, object> _defaultFactory;
		private List<IVirtualPathFactory> _factories;
		#endregion

		#region Constructors
		public VirtualPathFactoryManager()
			: this(null, null)
		{
		}

		public VirtualPathFactoryManager(VirtualPathProvider provider)
			: this(provider, null)
		{
		}

		public VirtualPathFactoryManager(Func<string, Type, object> factory)
			: this(null, factory)
		{
		}

		public VirtualPathFactoryManager(VirtualPathProvider provider, Func<string, Type, object> factory)
		{
			_provider = provider;
			_defaultFactory = factory ?? BuildManager.CreateInstanceFromVirtualPath;
			_factories = new List<IVirtualPathFactory>();
		}
		#endregion

		#region Static Properties
		public static VirtualPathFactoryManager Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_lock)
					{
						if (_instance == null)
							_instance = new VirtualPathFactoryManager();
					}
				}
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
		}

		public ICollection<IVirtualPathFactory> Factories
		{
			get
			{
				return _factories;
			}
		}
		#endregion

		#region Static Methods
		public static void RegisterFactory(IVirtualPathFactory factory)
		{
			Instance.Factories.Add(factory);
		}
		#endregion

		#region Instance Methods
		public TResource CreateInstance<TResource>(string virtualPath)
		{
			IVirtualPathFactory factory = _factories.FirstOrDefault(f => f.Exists(virtualPath));
			if (factory != null)
				return (TResource)factory.CreateInstance(virtualPath);

			return (TResource)_defaultFactory(virtualPath, typeof(TResource));
		}

		public bool PageExists(string virtualPath, bool useCache)
		{
			if (_factories.Any(factory => factory.Exists(virtualPath)))
				return true;

			return PageExistsInProvider(virtualPath, useCache);
		}

		private bool PageExistsInProvider(string virtualPath, bool useCache)
		{
			var cache = FileExistenceCache.Instance;
			if (useCache && Provider == cache.Provider)
				return cache.FileExists(virtualPath);
			
			return Provider.FileExists(virtualPath);
		}
		#endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    internal sealed class ControllerTypeCache
    {
        #region Static Fields
        private static object _lock = new object();
        #endregion

        #region Instance Fields
        private Dictionary<string, Type> _cache;
        #endregion

        #region Constructors
        public ControllerTypeCache()
        {
        }
        #endregion

        #region Instance Properties
        public int Count
        {
            get
            {
                return _cache.Count;
            }
        }
        #endregion

        #region Static Methods
        private static bool IsController(Type t)
        {
            if (t == null)
                return false;

            return (!t.IsAbstract &&
                !t.IsInterface && 
                typeof(IController).IsAssignableFrom(t));
        }

        private static IEnumerable<Type> DiscoverControllerTypes(IBuildManager buildManager)
        {
            List<Type> types = new List<Type>();
            foreach (Assembly assembly in buildManager.GetReferencedAssemblies())
            {
                Type[] typesInAsm;
                try
                {
                    typesInAsm = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    typesInAsm = ex.Types;
                }
                types.AddRange(typesInAsm.Where(IsController));
            }
            return types;
        }

		private static Dictionary<string, Type> BuildTypeCache(IBuildManager buildManager)
		{
			Dictionary<string, Type> cache = new Dictionary<string, Type>(
				StringComparer.OrdinalIgnoreCase);

			foreach (Type type in DiscoverControllerTypes(buildManager))
			{
				if (cache.ContainsKey(type.Name))
					throw Error.DuplicateControllerName(type.Name);

				cache.Add(type.Name, type);
			}
			return cache;
		}
        #endregion

        #region Instance Methods
        public bool TryGetController(string controllerName, out Type type)
        {
            return _cache.TryGetValue(controllerName, out type);
        }

        public void EnsureInitialized(IBuildManager buildManager)
        {
            Precondition.Require(buildManager, () => Error.ArgumentNull("buildManager"));
            if (_cache == null)
            {
                lock (_lock)
                {
					if (_cache == null)
						_cache = BuildTypeCache(buildManager);
                }
            }
        }
        #endregion
    }
}

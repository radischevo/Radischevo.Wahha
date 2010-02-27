using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Hosting
{
    /// <summary>
    /// A <see cref="System.Web.Hosting.VirtualPathProvider"/> that allows serving
    /// pages from embedded resources.
    /// </summary>
    public class AssemblyResourcePathProvider : VirtualPathProvider
    {
        #region Instance Fields
        private Assembly _assembly;
        private string _baseNamespace;
        private string _virtualPath;
        #endregion

        #region Constructors
        public AssemblyResourcePathProvider()
            : this((Assembly)null, null)
        {
        }

        public AssemblyResourcePathProvider(string assemblyName, string baseNamespace)
            : this(LoadAssemblyFromFile(assemblyName), baseNamespace)
        {
        }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Radischevo.Wahha.Web.Hosting.AssemblyResourcePathProvider" /> class.
        /// </summary>
        public AssemblyResourcePathProvider(Assembly assembly, string baseNamespace) 
            : base() 
        {
            _assembly = assembly;
            _baseNamespace = baseNamespace;
            _virtualPath = "~/";
        }
        #endregion

        #region Instance Properties
        public Assembly Assembly
        {
            get
            {
                return _assembly;
            }
            set
            {
                _assembly = value;
            }
        }

        public string AssemblyName
        {
            get
            {
                return (_assembly == null) ? null : 
                    Path.GetFileNameWithoutExtension(_assembly.Location);
            }
            set
            {
                _assembly = LoadAssemblyFromFile(value);
            }
        }

        public string BaseNamespace
        {
            get
            {
                return _baseNamespace ?? String.Empty;
            }
            set
            {
                _baseNamespace = value;
            }
        }

        public string VirtualPath
        {
            get
            {
                return _virtualPath ?? String.Empty;
            }
            set
            {
                _virtualPath = value;
            }
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Loads an assembly from a DLL file with the specified name, 
        /// located in the bin directory of a web application.
        /// </summary>
        /// <param name="fileName">The name of the assembly file.</param>
        public static Assembly LoadAssemblyFromFile(string fileName)
        {
            Precondition.Defined(fileName, () => Error.ArgumentNull("fileName"));

            string binDir = Path.GetDirectoryName(HttpRuntime.BinDirectory);
            string assemblyPath = Path.Combine(binDir, Path.GetFileName(fileName));

            if (!assemblyPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                assemblyPath += ".dll";

            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFile(assemblyPath);
            }
            catch
            {
                return null;
            }
            return assembly;
        }
        #endregion

        #region Instance Methods
        private bool IsAssemblyResource(string virtualPath)
        {
            string name;
            return IsAssemblyResource(virtualPath, out name);
        }

        protected virtual bool IsAssemblyResource(string virtualPath, out string resourceName)
        {
            Precondition.Defined(virtualPath, () => Error.ArgumentNull("virtualPath"));
            virtualPath = VirtualPathUtility.ToAppRelative(virtualPath);
            resourceName = null;

            if (!virtualPath.StartsWith(VirtualPath, StringComparison.InvariantCultureIgnoreCase))
                return false;

            if (Assembly != null)
            {
                string[] resources = Assembly.GetManifestResourceNames();
                resourceName = ConstructAssemblyResourceName(virtualPath);

                if (resources != null && resources.Contains(resourceName,
                    StringComparer.InvariantCultureIgnoreCase))
                    return true;
            }
            resourceName = null;
            return false;
        }

        protected virtual string ConstructAssemblyResourceName(string virtualPath)
        {
            string resourceName = virtualPath.Remove(0, VirtualPath.Length).Trim('/');

            if (BaseNamespace.Length > 0)
                resourceName = BaseNamespace + '/' + resourceName;
            
            return resourceName.Replace('/', '.');
        }

        /// <summary>
        /// Gets a value that indicates whether a file exists in the virtual file system.
        /// </summary>
        /// <param name="virtualPath">The path to the virtual file.</param>
        /// <exception cref="System.ArgumentNullException" />
        public override bool FileExists(string virtualPath)
        {
            return IsAssemblyResource(virtualPath) || Previous.FileExists(virtualPath);
        }

        /// <summary>
        /// Creates a cache dependency based on the specified virtual paths.
        /// </summary>
        /// <param name="virtualPath">The path to the primary virtual resource.</param>
        /// <param name="virtualPathDependencies">An array of paths to other resources required by the primary virtual resource.</param>
        /// <param name="utcStart">The UTC time at which the virtual resources were read.</param>
        /// <returns>A <see cref="System.Web.Caching.CacheDependency"/> object for the specified virtual resources.</returns>
        /// <exception cref="System.ArgumentNullException" />
        public override CacheDependency GetCacheDependency(string virtualPath, 
            IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            if (!IsAssemblyResource(virtualPath))
                return Previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);

            AggregateCacheDependency dependency = null;
            if (virtualPathDependencies != null)
            {
                foreach (string virtualPathDependency in virtualPathDependencies)
                {
                    CacheDependency dependencyToAdd = GetCacheDependency(virtualPathDependency, null, utcStart);
                    if (dependencyToAdd == null)
                        continue;

                    if (dependency == null)
                        dependency = new AggregateCacheDependency();
                    
                    dependency.Add(dependencyToAdd);
                }
            }

            CacheDependency primaryDependency = new CacheDependency(Assembly.Location, utcStart);
            if (dependency == null)
                dependency = new AggregateCacheDependency();
            
            dependency.Add(primaryDependency);
            return dependency;
        }

        /// <summary>
        /// Gets a virtual file from the virtual file system.
        /// </summary>
        /// <param name="virtualPath">The path to the virtual file.</param>
        /// <returns>A descendent of the <see cref="System.Web.Hosting.VirtualFile"/> 
        /// class that represents a file in the virtual file system.</returns>
        /// <exception cref="System.ArgumentNullException" />
        public override VirtualFile GetFile(string virtualPath)
        {
            string resourceName;
            if (IsAssemblyResource(virtualPath, out resourceName))
                return new AssemblyResourceVirtualFile(virtualPath, Assembly, resourceName);

            return Previous.GetFile(virtualPath);
        }
        #endregion
    }
}

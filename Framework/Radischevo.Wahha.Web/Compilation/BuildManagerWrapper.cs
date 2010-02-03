using System;
using System.Collections;
using System.Web.Compilation;
using System.Reflection;
using System.Web;

namespace Radischevo.Wahha.Web
{
    /// <summary>
    /// Provides a set of methods to help manage the 
    /// compilation of an ASP.NET application.
    /// </summary>
    public class BuildManagerWrapper : IBuildManager
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Radischevo.Wahha.Web.BuildManagerWrapper"/> class.
        /// </summary>
        public BuildManagerWrapper()
        { }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Gets a list of assemblies built from the App_Code directory.
        /// </summary>
        IList IBuildManager.CodeAssemblies 
        {
            get
            {
                return BuildManager.CodeAssemblies;
            }
        }

        /// <summary>
        /// Processes a file, given its virtual path, and creates an instance of the result.
        /// </summary>
        /// <param name="virtualPath">The virtual path of the file to create an instance of.</param>
        /// <param name="requiredBaseType">The base type that defines the object to be created.</param>
        object IBuildManager.CreateInstanceFromVirtualPath(string virtualPath, Type requiredBaseType)
        {
            return BuildManager.CreateInstanceFromVirtualPath(virtualPath, requiredBaseType);
        }

        /// <summary>
        /// Returns a build dependency set for a virtual path if the path is located
        /// in the ASP.NET cache.
        /// </summary>
        /// <param name="context">The context of the request.</param>
        /// <param name="virtualPath">The virtual path from which to determine the build dependency set.</param>
        BuildDependencySet IBuildManager.GetCachedBuildDependencySet(HttpContext context, string virtualPath)
        {
            return BuildManager.GetCachedBuildDependencySet(context, virtualPath);
        }

        /// <summary>
        /// Compiles a file into an assembly given its virtual path.
        /// </summary>
        /// <param name="virtualPath">The virtual path to build into an assembly.</param>
        Assembly IBuildManager.GetCompiledAssembly(string virtualPath)
        {
            return BuildManager.GetCompiledAssembly(virtualPath);
        }

        /// <summary>
        /// Compiles a file, given its virtual path, and returns a custom string that
        /// the build provider persists in cache.
        /// </summary>
        /// <param name="virtualPath">The virtual path of the file to build.</param>
        string IBuildManager.GetCompiledCustomString(string virtualPath)
        {
            return BuildManager.GetCompiledCustomString(virtualPath);
        }

        /// <summary>
        /// Compiles a file, given its virtual path, and returns the compiled type.
        /// </summary>
        /// <param name="virtualPath">The virtual path to build into a type.</param>
        Type IBuildManager.GetCompiledType(string virtualPath)
        {
            return BuildManager.GetCompiledType(virtualPath);
        }

        /// <summary>
        /// Returns a list of assembly references that 
        /// all page compilations must reference.
        /// </summary>
        ICollection IBuildManager.GetReferencedAssemblies()
        {
            return BuildManager.GetReferencedAssemblies();
        }

        /// <summary>
        /// Finds a type in the top-level assemblies, or in assemblies that are defined
        /// in configuration, by using a case-insensitive search and optionally throwing
        /// an exception on failure.
        /// </summary>
        /// <param name="typeName">The name of the type.</param>
        /// <param name="throwOnError">true to throw an exception if 
        /// a <see cref="System.Type"/> cannot be generated for the type
        /// name; otherwise, false.</param>
        /// <param name="ignoreCase">true if typeName is 
        /// case-sensitive; otherwise, false.</param>
        Type IBuildManager.GetType(string typeName, bool throwOnError, bool ignoreCase)
        {
            return BuildManager.GetType(typeName, throwOnError, ignoreCase);
        }

        /// <summary>
        /// Provides a collection of virtual-path dependencies for a 
        /// specified virtual path.
        /// </summary>
        /// <param name="virtualPath">The virtual path used to 
        /// determine the dependencies.</param>
        ICollection IBuildManager.GetVirtualPathDependencies(string virtualPath)
        {
            return BuildManager.GetVirtualPathDependencies(virtualPath);
        }
        #endregion
    }
}

using System;
using System.IO;
using System.Web.Hosting;
using System.Reflection;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Hosting
{
    /// <summary>
    /// Represents a file object in embedded resource space.
    /// </summary>
    public class AssemblyResourceVirtualFile : VirtualFile
    {
        #region Instance Fields
        private Assembly _assembly;
        private string _resourcePath;
	    #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Radischevo.Wahha.Web.Hosting.AssemblyResourceVirtualFile" /> class.
        /// </summary>
        /// <param name="virtualPath">The virtual path to the resource represented by this instance.</param>
        /// <param name="assembly">The <see cref="System.Reflection.Assembly"/> containing 
        /// the resource represented by this instance.</param>
        /// <param name="resourcePath">The path to the embedded resource in the <paramref name="assembly" />.</param>
        /// <exception cref="System.ArgumentNullException" />
        public AssemblyResourceVirtualFile(string virtualPath, 
            Assembly assembly, string resourcePath)
            : base(virtualPath)
        {
            Precondition.Require(assembly, () => Error.ArgumentNull("assembly"));
            Precondition.Defined(resourcePath, () => Error.ArgumentNull("resourcePath"));

            _assembly = assembly;
            _resourcePath = resourcePath;
        }
	    #endregion

        #region Instance Properties
        /// <summary>
        /// Gets a reference to the assembly containing the virtual file.
        /// </summary>
        public Assembly Assembly
        {
            get
            {
                return _assembly;
            }
        }

        /// <summary>
        /// Gets the path to the embedded resource in the containing assembly.
        /// </summary>
        public string ResourcePath
        {
            get
            {
                return _resourcePath;
            }
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Returns a read-only stream to the virtual resource.
        /// </summary>
        /// <returns>A read-only stream to the virtual file.</returns>
        public override Stream Open()
        {
            return _assembly.GetManifestResourceStream(_resourcePath);
        }
        #endregion
    }
}

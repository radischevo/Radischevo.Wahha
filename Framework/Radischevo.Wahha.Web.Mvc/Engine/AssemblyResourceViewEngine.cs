using System;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Hosting;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Hosting;

namespace Radischevo.Wahha.Web.Mvc
{
    /// <summary>
    /// Provides a view engine for serving pages or user controls 
    /// from embedded resources, using the 
    /// <see cref="Radischevo.Wahha.Web.Hosting.AssemblyResourcePathProvider"/>.
    /// </summary>
    public class AssemblyResourceViewEngine : WebFormViewEngine
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Radischevo.Wahha.Web.Mvc.AssemblyResourceViewEngine"/> class.
        /// </summary>
        public AssemblyResourceViewEngine()
            : base()
        {
            VirtualPathProvider = new AssemblyResourcePathProvider();
        }
        #endregion

        #region Instance Properties
        private AssemblyResourcePathProvider Provider
        {
            get
            {
                return (AssemblyResourcePathProvider)VirtualPathProvider;
            }
        }

        public Assembly Assembly
        {
            get
            {
                return Provider.Assembly;
            }
            set
            {
                Provider.Assembly = value;
            }
        }

        public string BaseNamespace
        {
            get
            {
                return Provider.BaseNamespace;
            }
            set
            {
                Provider.BaseNamespace = value;
            }
        }

        public string VirtualPath
        {
            get
            {
                return Provider.VirtualPath;
            }
            set
            {
                Provider.VirtualPath = value;
            }
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Initializes the current 
        /// <see cref="Radischevo.Wahha.Web.Mvc.AssemblyResourceViewEngine"/> instance 
        /// using the specified <paramref name="settings"/>.
        /// </summary>
        /// <param name="settings">A settings list for the current instance.</param>
        protected override void Init(IValueSet settings)
        {
            Precondition.Require(settings, () => Error.ArgumentNull("settings"));
            base.Init(settings);

            Provider.AssemblyName = settings.GetValue<string>("assembly");
            Provider.BaseNamespace = settings.GetValue<string>("baseNamespace");
            Provider.VirtualPath = settings.GetValue<string>("virtualPath");

            HostingEnvironment.RegisterVirtualPathProvider(Provider);
        }
        #endregion
    }
}

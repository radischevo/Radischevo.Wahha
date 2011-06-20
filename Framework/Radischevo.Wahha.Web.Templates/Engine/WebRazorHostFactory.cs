using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Compilation;
using System.Web.Configuration;
using System.Web.Hosting;

using Radischevo.Wahha.Web.Templates.Configurations;
using System.Web;
using System.Collections.Concurrent;

namespace Radischevo.Wahha.Web.Templates {
    public class WebRazorHostFactory {
        private static ConcurrentDictionary<string, Func<WebRazorHostFactory>> _factories = 
            new ConcurrentDictionary<string, Func<WebRazorHostFactory>>(StringComparer.OrdinalIgnoreCase);

        internal static Func<string, Type> TypeFactory = DefaultTypeFactory;

        public static WebPageRazorHost CreateDefaultHost(string virtualPath) {
            return CreateDefaultHost(virtualPath, null);
        }

        public static WebPageRazorHost CreateDefaultHost(string virtualPath, string physicalPath) {
            return CreateHostFromConfigCore(null, virtualPath, physicalPath);
        }

        public static WebPageRazorHost CreateHostFromConfig(string virtualPath) {
            return CreateHostFromConfig(virtualPath, null);
        }

        public static WebPageRazorHost CreateHostFromConfig(string virtualPath, string physicalPath) {
            if (String.IsNullOrEmpty(virtualPath)) { throw new ArgumentException(String.Format("CommonResources.Argument_Cannot_Be_Null_Or_Empty {0}", "virtualPath"), "virtualPath"); }

            return CreateHostFromConfigCore(GetRazorSection(virtualPath), virtualPath, physicalPath);
        }

        public static WebPageRazorHost CreateHostFromConfig(RazorWebSectionGroup config, string virtualPath) {
            return CreateHostFromConfig(config, virtualPath, null);
        }

        public static WebPageRazorHost CreateHostFromConfig(RazorWebSectionGroup config, string virtualPath, string physicalPath) {
            if (config == null) { throw new ArgumentNullException("config"); }
            if (String.IsNullOrEmpty(virtualPath)) { throw new ArgumentException(String.Format("CommonResources.Argument_Cannot_Be_Null_Or_Empty {0}", "virtualPath"), "virtualPath"); }

            return CreateHostFromConfigCore(config, virtualPath, physicalPath);
        }

        internal static WebPageRazorHost CreateHostFromConfigCore(RazorWebSectionGroup config, string virtualPath) {
            return CreateHostFromConfigCore(config, virtualPath, null);
        }

        internal static WebPageRazorHost CreateHostFromConfigCore(RazorWebSectionGroup config, string virtualPath, string physicalPath) {
            // Use the virtual path to select a host environment for the generated code
            // Do this check here because the App_Code host can't be overridden.

            // Make the path app relative
            virtualPath = EnsureAppRelative(virtualPath);

            if (virtualPath.StartsWith("~/App_Code", StringComparison.OrdinalIgnoreCase)) { // change to app-code
                // Under App_Code => It's a Web Code file
                return new WebCodeRazorHost(virtualPath, physicalPath);
            }

            WebRazorHostFactory factory = null;
            if (config != null && config.Host != null && !String.IsNullOrEmpty(config.Host.FactoryType)) {
                Func<WebRazorHostFactory> factoryCreator = _factories.GetOrAdd(config.Host.FactoryType, CreateFactory);
                Debug.Assert(factoryCreator != null); // CreateFactory should throw if there's an error creating the factory
                factory = factoryCreator();
            }

            WebPageRazorHost host = (factory ?? new WebRazorHostFactory()).CreateHost(virtualPath, physicalPath);

            if (config != null && config.Pages != null) {
                ApplyConfigurationToHost(config.Pages, host);
            }

            return host;
        }

        private static Func<WebRazorHostFactory> CreateFactory(string typeName) {
            Type factoryType = TypeFactory(typeName);
            if (factoryType == null) {
                throw new InvalidOperationException(String.Format("RazorWebResources.Could_Not_Locate_FactoryType {0}", typeName));
            }
            return Expression.Lambda<Func<WebRazorHostFactory>>(Expression.New(factoryType))
                             .Compile();
        }

        public static void ApplyConfigurationToHost(RazorPagesSection config, WebPageRazorHost host) {
            host.DefaultPageBaseClass = config.BaseType;

            // Add imports
            foreach (string import in config.Namespaces.OfType<NamespaceInfo>().Select(ns => ns.Namespace)) {
                host.NamespaceImports.Add(import);
            }
        }

        public virtual WebPageRazorHost CreateHost(string virtualPath, string physicalPath) {
            return new WebPageRazorHost(virtualPath, physicalPath);
        }

        internal static RazorWebSectionGroup GetRazorSection(string virtualPath) {
            // Get the individual sections (we can only use GetSection in medium trust) and then reconstruct the section group
            return new RazorWebSectionGroup() {
                Host = (HostSection)WebConfigurationManager.GetSection(HostSection.SectionName, virtualPath),
                Pages = (RazorPagesSection)WebConfigurationManager.GetSection(RazorPagesSection.SectionName, virtualPath)
            };
        }

        private static string EnsureAppRelative(string virtualPath) {
            if (HostingEnvironment.IsHosted) {
                virtualPath = VirtualPathUtility.ToAppRelative(virtualPath);
            }
            else {
                if (virtualPath.StartsWith("/", StringComparison.Ordinal)) {
                    virtualPath = "~" + virtualPath;
                }
                else if (!virtualPath.StartsWith("~/", StringComparison.Ordinal)) {
                    virtualPath = "~/" + virtualPath;
                }
            }
            return virtualPath;
        }

        private static Type DefaultTypeFactory(string typeName) {
            return BuildManager.GetType(typeName, false, false);
        }
    }
}

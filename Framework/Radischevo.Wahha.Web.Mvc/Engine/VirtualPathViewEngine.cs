using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Hosting;
using System.Web.Security;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Mvc.UI;

namespace Radischevo.Wahha.Web.Mvc
{
    public abstract class VirtualPathViewEngine : IViewEngine
    {
        #region Static Fields
        private static IViewLocationCache _staticLocationCache =
            new ViewLocationCache();
        #endregion

        #region Instance Fields
        private List<string> _locationFormats;
        private VirtualPathProvider _provider;
        private IViewLocationCache _locationCache;
        #endregion

        #region Instance Properties
        protected IEnumerable<string> LocationFormats
        {
            get
            {
                return _locationFormats;
            }
        }

        protected VirtualPathProvider VirtualPathProvider
        {
            get
            {
                if (_provider == null)
                    _provider = HostingEnvironment.VirtualPathProvider;

                return _provider;
            }
            set
            {
                _provider = value;
            }
        }

        protected IViewLocationCache LocationCache
        {
            get
            {
                if (HttpContext.Current == null ||
                    HttpContext.Current.IsDebuggingEnabled)
                    return NullViewLocationCache.Instance;

                if (_locationCache == null)
                    _locationCache = _staticLocationCache;

                return _locationCache;
            }
            set
            {
                _locationCache = value;
            }
        }
        #endregion

        #region Constructors
        protected VirtualPathViewEngine()
        {
            _locationFormats = new List<string>();
        }

        protected VirtualPathViewEngine(params string[] locationFormats)
        {
            _locationFormats = new List<string>(locationFormats);
        }
        #endregion

        #region Static Methods
        private static bool IsSpecificPath(string name)
        {
            char c = name[0];
            return (c == '~' || c == '/');
        }
        #endregion

        #region Instance Methods
        private string CreateCacheKey(string name, string controllerName)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(
                String.Format(CultureInfo.InvariantCulture, "ViewCacheEntry:{0}:{1}:{2}",
                base.GetType().AssemblyQualifiedName, name, controllerName), "MD5");
        }

        protected virtual void Init(IValueSet settings)
        {
        }

        protected abstract ViewEngineResult CreateView(ControllerContext context, string viewName);

        protected string GetViewPath(ControllerContext context, string viewName)
        {
            if (String.IsNullOrEmpty(viewName))
                return String.Empty;

            string controllerName = context.RouteData.GetRequiredValue<string>("controller");
            bool isSpecific = IsSpecificPath(viewName);
            string key = CreateCacheKey(viewName, (isSpecific) ? String.Empty : controllerName);

            string result = LocationCache.GetVirtualPath(key);
            if (result != null)
                return result;

            if (_locationFormats.Count < 1)
                throw Error.ViewLocationFormatsAreEmpty();

            result = (isSpecific) ? GetViewPathFromSpecificName(viewName) : 
                GetViewPathFromGeneralName(controllerName, viewName);

            LocationCache.SetVirtualPath(key, result);
            return result;
        }

        private string GetViewPathFromGeneralName(string controllerName, string viewName)
        {
            foreach (string format in _locationFormats)
            {
                string virtualPath = String.Format(CultureInfo.InvariantCulture, format, viewName, controllerName);
                if (VirtualPathProvider.FileExists(virtualPath))
                    return virtualPath;
            }
            return String.Empty;
        }

        private string GetViewPathFromSpecificName(string viewName)
        {
            if (!VirtualPathProvider.FileExists(viewName))
                return String.Empty;

            return viewName;
        }

        public virtual void ReleaseView(ControllerContext context, IView view)
        {
            IDisposable disposable = (view as IDisposable);
            if (disposable != null)
                disposable.Dispose();
        }
	    #endregion

        #region IViewEngine Members
        ViewEngineResult IViewEngine.CreateView(ControllerContext context, string viewName)
        {
            return CreateView(context, viewName);
        }

        void IViewEngine.Init(IValueSet settings)
        {
            Init(settings);
        }
        #endregion
    }
}

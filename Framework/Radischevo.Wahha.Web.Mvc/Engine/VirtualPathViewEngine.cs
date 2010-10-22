using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Hosting;
using System.Web.Security;

using Radischevo.Wahha.Core;

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

		protected virtual bool FileExists(ControllerContext context, string virtualPath)
		{
			return VirtualPathProvider.FileExists(virtualPath);
		}

		protected virtual bool? IsValidPath(ControllerContext context, string virtualPath)
		{
			return null;
		}

		protected virtual ViewEngineResult FindView(ControllerContext context, string viewName)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));
			Precondition.Defined(viewName, () => Error.ArgumentNull("viewName"));

			string viewPath = GetViewPath(context, viewName);
			if (String.IsNullOrEmpty(viewPath))
				return null;

			return new ViewEngineResult(CreateView(context, viewPath), this);
		}

		protected abstract IView CreateView(ControllerContext context, string virtualPath);

		protected virtual string MakeCanonicalControllerName(string controllerName)
		{
			if (String.IsNullOrEmpty(controllerName))
				return String.Empty;

			int index = controllerName.LastIndexOf("controller", 
				StringComparison.OrdinalIgnoreCase);

			if (index > -1)
				controllerName = controllerName.Substring(0, index);

			return controllerName;
		}

        private string GetViewPath(ControllerContext context, string viewName)
        {
            if (String.IsNullOrEmpty(viewName))
                return String.Empty;

            string controllerName = MakeCanonicalControllerName(
				context.RouteData.GetRequiredValue<string>("controller"));
			
            bool isSpecific = IsSpecificPath(viewName);
            string key = CreateCacheKey(viewName, (isSpecific) ? String.Empty : controllerName);

            string result = LocationCache.GetVirtualPath(key);
            if (result != null)
                return result;

            if (_locationFormats.Count < 1)
                throw Error.ViewLocationFormatsAreEmpty();

            result = (isSpecific) ? GetViewPathFromSpecificName(context, viewName, true) : 
                GetViewPathFromGeneralName(context, controllerName, viewName);

            LocationCache.SetVirtualPath(key, result);
            return result;
        }

        private string GetViewPathFromGeneralName(ControllerContext context, string controllerName, string viewName)
        {
            foreach (string format in _locationFormats)
            {
                string virtualPath = String.Format(CultureInfo.InvariantCulture, format, viewName, controllerName);
                if (FileExists(context, virtualPath))
                    return virtualPath;
            }
            return String.Empty;
        }

        private string GetViewPathFromSpecificName(ControllerContext context, string viewName, bool checkValidity)
        {
			if (FileExists(context, viewName))
			{
				if (checkValidity)
				{
					bool? isValid = IsValidPath(context, viewName);
					return (isValid.GetValueOrDefault()) ? viewName : String.Empty;
				}
				return viewName;
			}
            return String.Empty;
        }

        public virtual void ReleaseView(ControllerContext context, IView view)
        {
            IDisposable disposable = (view as IDisposable);
            if (disposable != null)
                disposable.Dispose();
        }
	    #endregion

        #region IViewEngine Members
        ViewEngineResult IViewEngine.FindView(ControllerContext context, string viewName)
        {
            return FindView(context, viewName);
        }

        void IViewEngine.Init(IValueSet settings)
        {
            Init(settings);
        }
        #endregion
    }
}

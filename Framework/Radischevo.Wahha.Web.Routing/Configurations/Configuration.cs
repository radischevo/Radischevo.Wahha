using System;
using System.Collections.Generic;
using System.Configuration;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Routing.Configurations
{
    /// <summary>
    /// Stores the routing configuration
    /// </summary>
    public sealed class Configuration
    {
        #region Static Fields
        private static Configuration _instance;
        private static object _lock = new object();
        #endregion

        #region Instance Fields
        private IRouteTableProvider _provider;
        private List<RouteConfigurationElement> _routes;
		private NameValueConfigurationCollection _variables;
        #endregion

        #region Constructors
        private Configuration()
        {
            _routes = new List<RouteConfigurationElement>();
			_variables = new NameValueConfigurationCollection();

            SettingsSection section =
                    ConfigurationManager.GetSection("radischevo.wahha/web/routing")
                    as SettingsSection;

            if (section != null)
            {
                InitRouteTableProvider(section.Provider);
				InitVariables(section.Variables);
                LoadRouteTable(section.Routes);
            }
        }
        #endregion

        #region Static Properties
        /// <summary>
        /// Gets the current routing configuration
        /// </summary>
        public static Configuration Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if(_instance == null)
                            _instance = new Configuration();
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets the configured route 
        /// persistence provider
        /// </summary>
        public IRouteTableProvider Provider
        {
            get
            {
                return _provider;
            }
            set
            {
                _provider = value;
            }
        }

		public NameValueConfigurationCollection Variables
		{
			get
			{
				return _variables;
			}
		}

        public ICollection<RouteConfigurationElement> Routes
        {
            get
            {
                return _routes;
            }
        }
        #endregion

        #region Instance Methods
        private void InitRouteTableProvider(
            RouteTableProviderConfigurationElement element)
        {
            Type providerType = Type.GetType(element.Type, false, true);
            if (providerType == null)
                return;

            if (!typeof(IRouteTableProvider).IsAssignableFrom(providerType))
                throw Error.IncompatibleRouteTableProvider(providerType);

            _provider = (IRouteTableProvider)ServiceLocator.Instance.GetService(providerType);

            ValueDictionary settings = new ValueDictionary();
            foreach (string key in element.Settings.AllKeys)
                settings.Add(key, element.Settings[key].Value);

            _provider.Init(settings);
        }

		private void InitVariables(NameValueConfigurationCollection collection)
		{
			if (collection == null)
				return;

			foreach (NameValueConfigurationElement element in collection)
				_variables.Add(element);
		}

        private void LoadRouteTable(RouteConfigurationElementCollection routes)
        {
            if (routes == null)
                return;

            foreach (RouteConfigurationElement element in routes)
                _routes.Add(element);
        }
        #endregion
    }
}

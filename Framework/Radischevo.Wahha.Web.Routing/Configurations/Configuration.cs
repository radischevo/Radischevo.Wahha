using System;
using System.Collections.Generic;
using System.Configuration;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Routing.Configurations
{
    /// <summary>
    /// Stores the routing configuration
    /// </summary>
    public sealed class Configuration : Singleton<Configuration>
    {
        #region Instance Fields
        private IRouteTableProvider _provider;
        private List<RouteConfigurationElement> _routes;
		private Type _defaultHandlerType;
		private NameValueCollection<string> _variables;
        #endregion

        #region Constructors
        private Configuration()
        {
            _routes = new List<RouteConfigurationElement>();
			_variables = new NameValueCollection<string>();

			try
            {
				SettingsSection section = ConfigurationManager.GetSection("radischevo.wahha/web/routing") as SettingsSection;
	            if (section == null)
					return;
					
                section.Configure(this);
            }
			catch(ConfigurationErrorsException ex)
			{
				throw Error.UnableToLoadConfiguration(ex);
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

		public NameValueCollection<string> Variables
		{
			get
			{
				return _variables;
			}
		}

		public Type DefaultHandlerType
		{
			get
			{
				return _defaultHandlerType;
			}
			set
			{
				_defaultHandlerType = value;
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
    }
}

using System;
using System.Collections.Specialized;
using System.Configuration;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data;

namespace Radischevo.Wahha.Data.Configurations
{
    /// <summary>
    /// Represents module configuration 
    /// settings.
    /// </summary>
    public sealed class Configuration
    {
        #region Static Fields
        private static Configuration _instance;
        private static object _lock = new object();
        #endregion

        #region Instance Fields
        private CacheSettings _caching;
        private DbDataProviderSettings _providers;
        private NameValueCollection _connectionStrings;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Configuration"/> class
        /// </summary>
        private Configuration()
        {
            _connectionStrings = new NameValueCollection();
            _caching = new CacheSettings();
            _providers = new DbDataProviderSettings();

            try
            {
                SettingsSection section =
                    ConfigurationManager.GetSection("radischevo.wahha/data") 
                    as SettingsSection;

                if (section == null)
                    return;

                _caching.Init(section.Cache);
                _providers.Init(section.Providers);

                foreach (ConnectionStringSettings cs in
                    section.ConnectionStrings)
                    _connectionStrings.Add(cs.Name, cs.ConnectionString);
            }
            catch (ConfigurationErrorsException ex)
            {
                throw Error.UnableToLoadConfiguration(ex);
            }
        }
        #endregion

        #region Static Properties
        /// <summary>
        /// Gets the current module configuration
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
		public DbDataProviderSettings Providers
		{
			get
			{
				return _providers;
			}
		}

		/// <summary>
		/// Gets the collection of connection strings, 
		/// declared within the configuration file
		/// </summary>
		public NameValueCollection ConnectionStrings
		{
			get
			{
				return _connectionStrings;
			}
		}

		public CacheSettings Caching
		{
			get
			{
				return _caching;
			}
		}
		#endregion
    }
}

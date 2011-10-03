using System;
using System.Configuration;

namespace Radischevo.Wahha.Data.Configurations
{
    /// <summary>
    /// Represents the module configuration settings.
    /// </summary>
    public sealed class Configuration
    {
        #region Static Fields
        private static Configuration _instance;
        private static object _lock = new object();
        #endregion

        #region Instance Fields
        private CacheSettings _caching;
        private DatabaseConfigurationSettings _database;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Configuration"/> class
        /// </summary>
        private Configuration()
        {
            _caching = new CacheSettings();
            _database = new DatabaseConfigurationSettings();

            try
            {
                SettingsSection section =
                    ConfigurationManager.GetSection("radischevo.wahha/data") 
                    as SettingsSection;

                if (section == null)
                    return;

                _caching.Init(section.Cache);
                _database.Init(section.Database);
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
                        if (_instance == null)
                            _instance = new Configuration();
                    }
                }
                return _instance;
            }
        }
        #endregion

		#region Instance Properties
		public DatabaseConfigurationSettings Database
		{
			get
			{
				return _database;
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

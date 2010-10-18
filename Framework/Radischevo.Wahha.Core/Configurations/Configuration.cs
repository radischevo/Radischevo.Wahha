using System;
using System.Configuration;

namespace Radischevo.Wahha.Core.Configurations
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
		private ServiceLocationSettings _serviceLocation;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Configuration"/> class.
        /// </summary>
        private Configuration()
        {
			_serviceLocation = new ServiceLocationSettings();
			Init();
        }
        #endregion

        #region Static Properties
        /// <summary>
        /// Gets the current module configuration.
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
		public ServiceLocationSettings ServiceLocation
		{
			get
			{
				return _serviceLocation;
			}
		}
		#endregion

		#region Instance Methods
		private void Init()
		{
			try
			{
				SettingsSection section = ConfigurationManager.GetSection("radischevo.wahha/core")
					as SettingsSection;

				if (section == null)
					return;

				_serviceLocation.Init(section.ServiceLocation);
			}
			catch (ConfigurationErrorsException ex)
			{
				throw Error.UnableToLoadConfiguration(ex);
			}
		}
		#endregion
	}
}

using System;
using System.Configuration;

namespace Radischevo.Wahha.Core.Configurations
{
    /// <summary>
    /// Represents module configuration settings.
    /// </summary>
    public sealed class Configuration : Singleton<Configuration>
    {
        #region Instance Fields
		private ServiceLocationSettings _serviceLocation;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Radischevo.Wahha.Core.Configurations.Configuration"/> class.
        /// </summary>
        private Configuration()
        {
			_serviceLocation = new ServiceLocationSettings();
			Initialize();
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
		private void Initialize()
		{
			try
			{
				SettingsSection section = ConfigurationManager.GetSection("radischevo.wahha/core") as SettingsSection;
				if (section == null)
					return;

				section.Configure(this);
			}
			catch (ConfigurationErrorsException ex)
			{
				throw Error.UnableToLoadConfiguration(ex);
			}
		}
		#endregion
	}
}

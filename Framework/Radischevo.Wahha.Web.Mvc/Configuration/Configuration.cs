using System;
using System.Collections.Generic;
using System.Configuration;

namespace Radischevo.Wahha.Web.Mvc.Configuration
{
    public sealed class Configuration
    {
        #region Static Fields
        private static Configuration _instance;
        private static object _lock = new object();
        #endregion

        #region Instance Fields
        private ControllerConfigurationSettings _controllers;
        private ViewConfigurationSettings _views;
        private ModelConfigurationSettings _models;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Configuration"/> class
        /// </summary>
        public Configuration()
        {
            _controllers = new ControllerConfigurationSettings();
            _views = new ViewConfigurationSettings();
            _models = new ModelConfigurationSettings();

            Init();
        }
        #endregion

        #region Static Properties
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
        public ControllerConfigurationSettings Controllers
        {
            get
            {
                return _controllers;
            }
        }

        public ViewConfigurationSettings Views
        {
            get
            {
                return _views;
            }
        }

        public ModelConfigurationSettings Models
        {
            get
            {
                return _models;
            }
        }
        #endregion

        #region Instance Methods
        private void Init()
        {            
            try
            {
                SettingsSection section = ConfigurationManager.GetSection("radischevo.wahha/web/mvc")
                   as SettingsSection;

                if (section == null)
                    return;

                if(section.Controllers != null)
                    _controllers.Init(section.Controllers);

                if (section.Views != null)
                    _views.Init(section.Views);

                if (section.Models != null)
                    _models.Init(section.Models);
            }
            catch (ConfigurationErrorsException ex)
            {
                throw Error.UnableToLoadConfiguration(ex);
            }           
        }
        #endregion
    }
}

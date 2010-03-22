using System;
using System.Configuration;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Configurations
{
    internal sealed class ControllerConfigurationElement : ConfigurationElement
    {
        #region Instance Fields
        private ValueDictionary _parameters;
        #endregion

        #region Constructors
        public ControllerConfigurationElement()
            : base()
        {
            _parameters = new ValueDictionary();
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets the string representation 
        /// of the type of controller factory which 
        /// will be used by default
        /// </summary>
        [ConfigurationProperty("factory", IsRequired = false, DefaultValue = "")]
        public string FactoryType
        {
            get
            {
                return base["factory"].ToString();
            }
        }

        /// <summary>
        /// Gets the predefined controller name-to-type mappings
        /// </summary>
        [ConfigurationProperty("mappings", IsDefaultCollection = true, IsRequired = false)]
        public ControllerMappingConfigurationElementCollection Mappings
        {
            get
            {
                return (ControllerMappingConfigurationElementCollection)base["mappings"];
            }
        }

        public IValueSet Parameters
        {
            get
            {
                return _parameters;
            }
        }
        #endregion

        #region Instance Methods
        protected sealed override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            _parameters[name] = value;
            return true;
        }
        #endregion
    }
}

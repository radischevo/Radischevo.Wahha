using System;
using System.Configuration;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Configurations
{
    internal sealed class ViewConfigurationElement : ConfigurationElement
    {
        #region Instance Fields
        private ValueDictionary _parameters;
        #endregion

        #region Constructors
        public ViewConfigurationElement()
            : base()
        {
            _parameters = new ValueDictionary();
        }
        #endregion

        #region Instance Properties
        [ConfigurationProperty("engine", IsRequired = true)]
        public string Type
        {
            get
            {
                return base["engine"].ToString();
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
        protected sealed override bool OnDeserializeUnrecognizedAttribute (string name, string value)
        {
            _parameters[name] = value;
            return true;
        }
        #endregion
    }
}

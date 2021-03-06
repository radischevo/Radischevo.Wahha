﻿using System;
using System.Configuration;

namespace Radischevo.Wahha.Web.Mvc.Configurations
{
    internal sealed class ControllerMappingConfigurationElement : ConfigurationElement
    {
		#region Instance Properties
		/// <summary>
        /// Gets the name assigned to the specified controller
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true, DefaultValue = "")]
        public string Name
        {
            get
            {
                return base["name"].ToString();
            }
        }

        /// <summary>
        /// Gets the string representation 
        /// of the type of controller
        /// </summary>
        [ConfigurationProperty("type", IsRequired = true, DefaultValue = "")]
        public string ControllerType
        {
            get
            {
                return base["type"].ToString();
            }
        }
		#endregion
    }
}

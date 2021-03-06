﻿using System;
using System.Configuration;

namespace Radischevo.Wahha.Web.Routing.Configurations
{
    /// <summary>
    /// Describes the route provider 
    /// configuration element.
    /// </summary>
    internal sealed class RouteTableProviderConfigurationElement : ConfigurationElement
    {
		#region Instance Properties
		/// <summary>
        /// Gets the string representation 
        /// of the type of route persistence provider.
        /// </summary>
        [ConfigurationProperty("type", IsRequired = false,
            DefaultValue = "")]
        public string Type
        {
            get
            {
                return base["type"].ToString();
            }
        }

        [ConfigurationProperty("settings")]
        public NameValueConfigurationCollection Settings
        {
            get
            {
                return (NameValueConfigurationCollection)base["settings"];
            }
        }
		#endregion
    }
}

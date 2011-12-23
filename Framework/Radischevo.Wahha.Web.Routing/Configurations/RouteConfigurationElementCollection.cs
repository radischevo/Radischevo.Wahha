using System;
using System.Configuration;

namespace Radischevo.Wahha.Web.Routing.Configurations
{
	[ConfigurationCollection(typeof(RouteConfigurationElement), AddItemName="route")]
    internal sealed class RouteConfigurationElementCollection : ConfigurationElementCollection
    {
		#region Instance Properties
		/// <summary>
		/// Gets the type name for the default route handler.
		/// </summary>
		[ConfigurationProperty("defaultHandler", IsRequired = false, DefaultValue = "")]
		public string DefaultHandlerType
		{
			get
			{
				return base["defaultHandler"].ToString();
			}
		}
		
		public RouteConfigurationElement this[int index]
        {
            get
            {
                return (RouteConfigurationElement)BaseGet(index);
            }
        }
		#endregion
		
		#region Instance Methods
		protected override ConfigurationElement CreateNewElement()
        {
            return new RouteConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RouteConfigurationElement)element).Name;
        }
		#endregion
    }
}


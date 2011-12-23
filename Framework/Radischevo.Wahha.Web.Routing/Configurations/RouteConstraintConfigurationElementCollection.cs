using System;
using System.Configuration;

namespace Radischevo.Wahha.Web.Routing.Configurations
{
	[ConfigurationCollection(typeof(RouteConstraintConfigurationElement), 
        CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public sealed class RouteConstraintConfigurationElementCollection : ConfigurationElementCollection
    {
		#region Instance Properties
		public RouteConstraintConfigurationElement this[int index]
        {
            get
            {
                return (RouteConstraintConfigurationElement)BaseGet(index);
            }
        }
		#endregion
		
		#region Instance Methods
		protected override ConfigurationElement CreateNewElement()
        {
            return new RouteConstraintConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return new object();
        }
		#endregion
    }
}


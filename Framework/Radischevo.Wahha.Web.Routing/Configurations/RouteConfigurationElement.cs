using System;
using System.Configuration;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Routing.Configurations
{
    public class RouteConfigurationElement : ConfigurationElement
    {
        #region Instance Fields
        private ValueDictionary _attributes;
        #endregion

        #region Constructors
        public RouteConfigurationElement()
            : base()
        {
            _attributes = new ValueDictionary();
        }
        #endregion

        #region Instance Properties
        [ConfigurationProperty("name", IsRequired = true,
            DefaultValue = "")]
        public string Name
        {
            get
            {
                return base["name"].ToString();
            }
        }

		[ConfigurationProperty("type", IsRequired = false,
			DefaultValue = "")]
		public string Type
		{
			get
			{
				return base["type"].ToString();
			}
		}

        [ConfigurationProperty("url", IsRequired = true,
            DefaultValue = "")]
        public string Url
        {
            get
            {
                return base["url"].ToString();
            }
        }

        [ConfigurationProperty("handler", IsRequired = false,
            DefaultValue = "")]
        public string HandlerType
        {
            get
            {
                return base["handler"].ToString();
            }
        }

        [ConfigurationProperty("defaults")]
        public NameValueConfigurationCollection Defaults
        {
            get
            {
                return (NameValueConfigurationCollection)base["defaults"];
            }
        }

        [ConfigurationProperty("tokens")]
        public NameValueConfigurationCollection Tokens
        {
            get
            {
                return (NameValueConfigurationCollection)base["tokens"];
            }
        }

        [ConfigurationProperty("constraints")]
        public RouteConstraintConfigurationElementCollection Constraints
        {
            get
            {
                return (RouteConstraintConfigurationElementCollection)base["constraints"];
            }
        }

		public IValueSet Attributes
		{
			get
			{
				return _attributes;
			}
		}
        #endregion

        #region Instance Methods
        protected sealed override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            _attributes[name] = value;
            return true;
        }
        #endregion
    }

    [ConfigurationCollection(typeof(RouteConfigurationElement), AddItemName="route")]
    internal sealed class RouteConfigurationElementCollection : ConfigurationElementCollection
    {
		/// <summary>
		/// Gets the type name for the default route handler.
		/// </summary>
		[ConfigurationProperty("defaultHandler", IsRequired = false,
			DefaultValue = "")]
		public string DefaultHandlerType
		{
			get
			{
				return base["defaultHandler"].ToString();
			}
		}

        protected override ConfigurationElement CreateNewElement()
        {
            return new RouteConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RouteConfigurationElement)element).Name;
        }

        public RouteConfigurationElement this[int index]
        {
            get
            {
                return (RouteConfigurationElement)BaseGet(index);
            }
        }
    }
}

﻿using System.Configuration;

namespace Radischevo.Wahha.Web.Templates.Configurations
{
	public class HostSection : ConfigurationSection
	{
		public static readonly string SectionName = RazorWebSectionGroup.GroupName + "/host";

		private static readonly ConfigurationProperty _typeProperty =
			new ConfigurationProperty("factoryType", typeof(string), null,
				ConfigurationPropertyOptions.IsRequired);

		private bool _factoryTypeSet = false;
		private string _factoryType;

		[ConfigurationProperty("factoryType", IsRequired = true, DefaultValue = null)]
		public string FactoryType
		{
			get
			{
				return _factoryTypeSet ? _factoryType : (string)this[_typeProperty];
			}
			set
			{
				_factoryType = value;
				_factoryTypeSet = true;
			}
		}
	}
}

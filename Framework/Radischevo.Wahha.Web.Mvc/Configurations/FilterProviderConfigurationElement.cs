using System;
using System.Configuration;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Configurations
{
	internal sealed class FilterProviderConfigurationElement : ConfigurationElement
	{
		#region Instance Fields
		private ValueDictionary _parameters;
		#endregion

		#region Constructors
		public FilterProviderConfigurationElement()
			: base()
		{
			_parameters = new ValueDictionary();
		}
		#endregion

		#region Instance Properties
		[ConfigurationProperty("type", IsRequired = true)]
		public string ProviderType
		{
			get
			{
				return base["type"].ToString();
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

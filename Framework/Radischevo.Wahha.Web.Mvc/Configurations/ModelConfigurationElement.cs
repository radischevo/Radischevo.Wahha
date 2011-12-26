using System;
using System.Configuration;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Configurations
{
	internal sealed class ModelConfigurationElement : ConfigurationElement, IConfigurator<ModelConfigurationSettings>
	{
		#region Instance Properties
		[ConfigurationProperty("binders", IsRequired = false)]
		public ModelBinderConfigurationElementCollection Binders
		{
			get
			{
				return (ModelBinderConfigurationElementCollection)base["binders"];
			}
		}

		[ConfigurationProperty("valueProviders", IsRequired = false)]
		public ValueProviderConfigurationElementCollection ValueProviders
		{
			get
			{
				return (ValueProviderConfigurationElementCollection)base["valueProviders"];
			}
		}
		
		[ConfigurationProperty("validatorProviders", IsRequired = false)]
		public ModelValidatorProviderConfigurationElementCollection ValidatorProviders
		{
			get
			{
				return (ModelValidatorProviderConfigurationElementCollection)base["validatorProviders"];
			}
		}
		#endregion
		
		#region Static Methods
        private static IModelBinder CreateModelBinder(Type type)
        {
            Precondition.Require(type, () => Error.ArgumentNull("type"));
            if (!typeof(IModelBinder).IsAssignableFrom(type))
                throw Error.IncompatibleModelBinderType(type);

			return (IModelBinder)ServiceLocator.Instance.GetService(type);
        }

		private static IValueProviderFactory CreateValueProviderFactory(Type type)
		{
			Precondition.Require(type, () => Error.ArgumentNull("type"));
			if (!typeof(IValueProviderFactory).IsAssignableFrom(type))
				throw Error.IncompatibleValueProviderFactoryType(type);

			return (IValueProviderFactory)ServiceLocator.Instance.GetService(type);
		}

		private static IModelValidatorProvider CreateValidatorProvider(Type type)
		{
			Precondition.Require(type, () => Error.ArgumentNull("type"));
			if (!typeof(IModelValidatorProvider).IsAssignableFrom(type))
				throw Error.IncompatibleModelValidatorProviderType(type);

			return (IModelValidatorProvider)ServiceLocator.Instance.GetService(type);
		}
		#endregion
		
		#region Instance Methods
		public void Configure (ModelConfigurationSettings module)
		{
			if (Binders != null)
			{
				if (!String.IsNullOrEmpty(Binders.DefaultType))
					module.Binders.DefaultBinder = CreateModelBinder(
						Type.GetType(Binders.DefaultType, true, true));

				foreach (ModelBinderConfigurationElement elem in Binders)
				{
					Type modelType = Type.GetType(elem.ModelType, true, true);
	
					if (!String.IsNullOrEmpty(elem.BinderType))
						module.Binders.Add(modelType, CreateModelBinder(Type.GetType(elem.BinderType, true, true)));
				}
			}
			
			if (ValueProviders != null)
			{
				foreach (ValueProviderConfigurationElement elem in ValueProviders)
				{
					module.ValueProviders.Add(elem.Name, CreateValueProviderFactory(
						Type.GetType(elem.FactoryType, true, true)));
				}
			}
			
			if (ValidatorProviders != null) 
			{
				foreach (ModelValidatorProviderConfigurationElement elem in ValidatorProviders)
				{
					module.ValidatorProviders.Add(CreateValidatorProvider(
						Type.GetType(elem.Type, true, true)));
				}
			}
		}
		#endregion
	}
}

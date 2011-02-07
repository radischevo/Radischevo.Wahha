using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Configurations
{
    public sealed class ControllerConfigurationSettings
    {
        #region Instance Fields
        private IControllerFactory _factory;
        private Dictionary<string, Type> _mappings;
		private FilterProviderCollection _filterProviders;
        #endregion

        #region Constructors
        internal ControllerConfigurationSettings()
        {
            _mappings = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
			_filterProviders = new FilterProviderCollection();

			InitDefaultFilterProviders();
        }
        #endregion

        #region Instance Properties
        public IControllerFactory Factory
        {
            get
            {
				if (_factory == null)
					_factory = new DefaultControllerFactory();

                return _factory;
            }
            set
            {
                _factory = value;
            }
        }

        public IDictionary<string, Type> Mappings
        {
            get
            {
                return _mappings;
            }
        }

		public FilterProviderCollection FilterProviders
		{
			get
			{
				return _filterProviders;
			}
		}
        #endregion

		#region Static Methods
		private static IFilterProvider CreateFilterProvider(Type type)
		{
			Precondition.Require(type, () => Error.ArgumentNull("type"));
			if (!typeof(IFilterProvider).IsAssignableFrom(type))
				throw Error.IncompatibleFilterProviderType(type);

			return (IFilterProvider)ServiceLocator.Instance.GetService(type);
		}

		private static IControllerFactory CreateControllerFactory(Type type)
		{
			Precondition.Require(type, () => Error.ArgumentNull("type"));
			if (!typeof(IControllerFactory).IsAssignableFrom(type))
				throw Error.IncompatibleControllerFactoryType(type);

			IControllerFactory factory = (IControllerFactory)ServiceLocator.Instance.GetService(type);
			return factory;
		}
		#endregion

		#region Instance Methods
		internal void Init(ControllerConfigurationElement element)
        {
            Precondition.Require(element, () => Error.ArgumentNull("element"));

			InitFactory(element.Factory);
			InitFilterProviders(element.FilterProviders);
			InitStaticMappings(element.Mappings);
        }

		private void InitDefaultFilterProviders()
		{
			_filterProviders.Add(new AttributedFilterProvider(true));
			_filterProviders.Add(new ControllerFilterProvider());
		}

		private void InitFilterProviders(FilterProviderConfigurationElementCollection element)
		{
			if (element == null)
				return;

			foreach (FilterProviderConfigurationElement elem in element)
			{
				IFilterProvider provider = CreateFilterProvider(
					Type.GetType(elem.ProviderType, true, true));

				provider.Init(elem.Parameters);
				_filterProviders.Add(provider);
			}
		}

		private void InitStaticMappings(ControllerMappingConfigurationElementCollection element)
		{
			if (element == null)
				return;

			foreach (ControllerMappingConfigurationElement map in element)
				_mappings.Add(map.Name, Type.GetType(map.ControllerType, true, true));
		}

		private void InitFactory(ControllerFactoryConfigurationElement element)
		{
			Type type = String.IsNullOrEmpty(element.FactoryType) ? 
				typeof(DefaultControllerFactory) : 
				Type.GetType(element.FactoryType, true, true);

			_factory = CreateControllerFactory(type);
			_factory.Init(element.Parameters);
		}
        #endregion
    }
}

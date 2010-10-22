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
        #endregion

        #region Constructors
        internal ControllerConfigurationSettings()
        {
            _mappings = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
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
        #endregion

        #region Instance Methods
        internal void Init(ControllerConfigurationElement element)
        {
            Precondition.Require(element, () => Error.ArgumentNull("element"));
			CreateFactory(element);

            foreach (ControllerMappingConfigurationElement nm in element.Mappings)
                _mappings.Add(nm.Name, Type.GetType(nm.ControllerType, true, true));
        }

		private void CreateFactory(ControllerConfigurationElement element)
		{
			Type type = Type.GetType(element.FactoryType, false, true);
			if (type != null)
			{
				if (!typeof(IControllerFactory).IsAssignableFrom(type))
					throw Error.IncompatibleControllerFactoryType(type);

				_factory = (IControllerFactory)ServiceLocator.Instance.GetService(type);
				_factory.Init(element.Parameters);
			}
			else
				_factory = new DefaultControllerFactory();
		}
        #endregion
    }
}

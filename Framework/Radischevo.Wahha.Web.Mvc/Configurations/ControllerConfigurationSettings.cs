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
            _factory = new DefaultControllerFactory();
            _mappings = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        }
        #endregion

        #region Instance Properties
        public IControllerFactory Factory
        {
            get
            {
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

            Type type = Type.GetType(element.FactoryType, false, true);
            if (type != null)
            {
                if (!typeof(IControllerFactory).IsAssignableFrom(type))
                    throw Error.IncompatibleControllerFactoryType(type);

				_factory = (IControllerFactory)ServiceLocator.Instance.GetService(type);
            }
            else
            {
                _factory = new DefaultControllerFactory();
            }

            _factory.Init(element.Parameters);

            foreach (ControllerMappingConfigurationElement nm in element.Mappings)
                _mappings.Add(nm.Name, Type.GetType(nm.ControllerType, true, true));
        }
        #endregion
    }
}

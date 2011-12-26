using System;
using System.Collections.Generic;

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

		#region Instance Methods
		private void InitDefaultFilterProviders()
		{
			_filterProviders.Add(new AttributedFilterProvider(true));
			_filterProviders.Add(new ControllerFilterProvider());
		}
        #endregion
    }
}

using System;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data.Caching;

namespace Radischevo.Wahha.Data.Configurations
{
    public sealed class CacheSettings
    {
        #region Instance Fields
        private Type _providerType;
        private ValueDictionary _settings;
        #endregion

        #region Constructors
        internal CacheSettings()
        {
            _settings = new ValueDictionary();
        }
        #endregion

        #region Instance Properties
        public Type ProviderType
        {
            get
            {
                if (_providerType == null)
                    _providerType = typeof(NullCacheProvider);

                return _providerType;
            }
            set
            {
                _providerType = value;
            }
        }

        public ValueDictionary Settings
        {
            get
            {
                return _settings;
            }
        }
        #endregion
    }
}

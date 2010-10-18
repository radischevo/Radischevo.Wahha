using System;
using System.Collections.Generic;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Configurations
{
    public sealed class DbDataProviderSettings
    {
        #region Instance Fields
        private int _commandTimeout;
        private IDbDataProviderFactory _factory;
        private DbDataProviderMappingDictionary _mappings;
        #endregion

        #region Constructors
        internal DbDataProviderSettings()
        {
            _commandTimeout = 10;
            _mappings = new DbDataProviderMappingDictionary();
        }
        #endregion

        #region Instance Properties
        public int CommandTimeout
        {
            get
            {
                return _commandTimeout;
            }
            set
            {
                _commandTimeout = value;
            }
        }

        public IDbDataProviderFactory Factory
        {
            get
            {
                if (_factory == null)
                    _factory = new DefaultDbDataProviderFactory();

                return _factory;
            }
            set
            {
                Precondition.Require(value, () => Error.ArgumentNull("value"));
                _factory = value;
            }
        }

        public DbDataProviderMappingDictionary Mappings
        {
            get
            {
                return _mappings;
            }
        }
        #endregion

        #region Instance Fields
        internal void Init(DbDataProviderFactoryConfigurationElement element)
        {
            if (element == null)
                return;

            bool hasDefault = false;
            string defaultName = null;
            _commandTimeout = element.CommandTimeout;

            Type type = Type.GetType(element.FactoryType, false, true);
            if (type != null)
            {
                if (!typeof(IDbDataProviderFactory).IsAssignableFrom(type))
                    throw Error.IncompatibleDataProviderFactoryType(type);

                _factory = (IDbDataProviderFactory)ServiceLocator.Instance.GetService(type);
            }
            else
                _factory = new DefaultDbDataProviderFactory();

            foreach(DbDataProviderConfigurationElement p in element)
            {
                Type t = Type.GetType(p.Type, true, true);
                _mappings.Add(p.Name, t);

                if (p.IsDefault)
                {
                    if (hasDefault)
                        throw Error.CannotAddMoreThanOneDefaultProvider(defaultName, p.Name);

                    hasDefault = true;
                    _mappings.Default = t;
                }
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Specialized;
using System.Configuration;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Configurations
{
    public sealed class DatabaseConfigurationSettings
    {
        #region Instance Fields
        private int _commandTimeout;
        private IDbDataProviderFactory _factory;
		private NameValueCollection _connectionStrings;
        #endregion

        #region Constructors
        internal DatabaseConfigurationSettings()
        {
            _commandTimeout = 10;
			_connectionStrings = new NameValueCollection();
        }
        #endregion

        #region Instance Properties
		/// <summary>
		/// Gets or sets the default database command 
		/// execution timeout.
		/// </summary>
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

		/// <summary>
		/// Gets or sets the IDbdataProvider factory 
		/// to be used for communicating with the database.
		/// </summary>
        public IDbDataProviderFactory Factory
        {
            get
            {
				if (_factory == null)
					_factory = ServiceLocator.Instance.GetService<IDbDataProviderFactory>();

                return _factory;
            }
            set
            {
                Precondition.Require(value, () => Error.ArgumentNull("value"));
                _factory = value;
            }
        }

		/// <summary>
		/// Gets the collection of connection strings, 
		/// declared within the configuration file
		/// </summary>
		public NameValueCollection ConnectionStrings
		{
			get
			{
				return _connectionStrings;
			}
		}
        #endregion

        #region Instance Methods
        internal void Init(DatabaseConfigurationElement element)
        {
            if (element == null)
                return;

            _commandTimeout = element.CommandTimeout;

			if (element.ConnectionStrings != null)
			{
				foreach (ConnectionStringSettings cs in element.ConnectionStrings)
					_connectionStrings.Add(cs.Name, cs.ConnectionString);
			}
			if (element.Provider != null) 
			{
				Type type = Type.GetType(element.Provider.FactoryType, false, true);
				if (!typeof(IDbDataProviderFactory).IsAssignableFrom(type))
					throw Error.IncompatibleDataProviderFactoryType(type);

				_factory = (IDbDataProviderFactory)ServiceLocator.Instance.GetService(type);
			}
        }
        #endregion
    }
}

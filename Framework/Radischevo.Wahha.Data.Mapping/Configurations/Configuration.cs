using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Mapping.Configurations
{
    /// <summary>
    /// Represents the module configuration settings.
    /// </summary>
    public sealed class Configuration : Singleton<Configuration>
    {
        #region Instance Fields
        private IMetaMappingFactory _factory;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Configuration"/> class
        /// </summary>
        private Configuration()
        {
            _factory = new AttributedMetaMappingFactory();
        }
        #endregion

		#region Instance Properties
		public IMetaMappingFactory Factory
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
		#endregion
    }
}

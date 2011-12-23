using System;
using System.Collections.Generic;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Provider
{
    public abstract class CommonDbDataProviderFactory : IDbDataProviderFactory
    {
		#region Instance Fields
		private string _connectionString;
		#endregion
		
        #region Constructors
        protected CommonDbDataProviderFactory()
        {
		}
        #endregion
		
        #region Instance Methods
		public void Init (DbDataProviderFactorySettings settings)
		{
			if (settings.ConnectionStrings.Count < 1)
				throw Error.ConnectionStringNotConfigured();
			
			_connectionString = settings.ConnectionStrings.First();
		}
		
		/// <summary>
		/// Creates a new instance of the <see cref="Radischevo.Wahha.Data.IDbDataProvider"/> 
		/// class, using the default connection string defined in the configuration file.
		/// </summary>
		public virtual IDbDataProvider CreateProvider()
		{
			return CreateProvider(_connectionString);
		}

		/// <summary>
		/// When overridden in a derived class, creates a new instance of the 
		/// <see cref="Radischevo.Wahha.Data.IDbDataProvider"/> class, 
		/// using the specified connection string.
		/// </summary>
		protected abstract IDbDataProvider CreateProvider(string connectionString);

		/// <summary>
		/// When overridden in a derived class, disposes the specified 
		/// <see cref="Radischevo.Wahha.Data.IDbDataProvider"/>.
		/// </summary>
		/// <param name="provider">The provider instance to dispose.</param>
        public virtual void DisposeProvider(IDbDataProvider provider)
        {
            Precondition.Require(provider, () => Error.ArgumentNull("provider"));
            provider.Dispose();
        }
        #endregion
    }
}

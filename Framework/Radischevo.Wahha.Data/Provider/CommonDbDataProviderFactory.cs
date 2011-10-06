using System;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data.Configurations;

namespace Radischevo.Wahha.Data.Provider
{
    public abstract class CommonDbDataProviderFactory : IDbDataProviderFactory
    {
        #region Constructors
        protected CommonDbDataProviderFactory()
        {
		}
        #endregion

        #region Instance Methods
		/// <summary>
		/// Creates a new instance of the <see cref="Radischevo.Wahha.Data.IDbDataProvider"/> 
		/// class, using the default connection string defined in the configuration file.
		/// </summary>
		public virtual IDbDataProvider CreateProvider()
		{
			string connectionString = Configuration.Instance
				.Database.ConnectionStrings["default"];

			Precondition.Defined(connectionString, () => 
				Error.ConnectionStringNotConfigured());

			return CreateProvider(connectionString);
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

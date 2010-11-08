using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Routing.Configurations;

namespace Radischevo.Wahha.Web.Routing
{
    /// <summary>
    /// Provides a contract for the 
    /// route table persistence provider.
    /// </summary>
    public interface IRouteTableProvider
    {
        #region Instance Methods
		/// <summary>
		/// Initializes the current instance of the <see cref="T:IRouteTableProvider"/>.
		/// </summary>
		/// <param name="settings">The <see cref="T:IValueSet"/> containing 
		/// provider settings.</param>
        void Init(IValueSet settings);

        /// <summary>
        /// Gets the currently configured route table.
        /// </summary>
        RouteTableProviderResult GetRouteTable();
        #endregion
    }
}

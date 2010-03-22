using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Routing.Configurations;

namespace Radischevo.Wahha.Web.Routing
{
    /// <summary>
    /// Provides a contract for the 
    /// route table persistence provider
    /// </summary>
    public interface IRouteTableProvider
    {
        #region Instance Methods
        void Init(IValueSet settings);

        /// <summary>
        /// Gets the currently configured route table
        /// </summary>
        IDictionary<string, RouteBase> GetRouteTable();
        #endregion
    }
}

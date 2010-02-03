using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Routing.Providers
{
    public sealed class DefaultRouteTableProvider : IRouteTableProvider
    {
        #region Constructors
        public DefaultRouteTableProvider()
        {
        }
        #endregion

        #region IRouteTableProvider Members
        void IRouteTableProvider.Init(IValueSet settings)
        {
        }

        public IDictionary<string, RouteBase> GetRouteTable()
        {
            return new Dictionary<string, RouteBase>(StringComparer.OrdinalIgnoreCase);
        }
        #endregion
    }
}

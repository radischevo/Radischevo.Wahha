using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Web.Mvc
{
    /// <summary>
    /// Defines the contract for temp data providers 
    /// which store data viewed on the next request.
    /// </summary>
    public interface ITempDataProvider
    {
        #region Instance Methods
        IDictionary<string, object> Load(ControllerContext context);
        void Save(ControllerContext context, IDictionary<string, object> values);
        #endregion
    }
}

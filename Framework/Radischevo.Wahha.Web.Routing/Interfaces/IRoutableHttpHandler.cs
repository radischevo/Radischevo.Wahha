using System;
using System.Web;

namespace Radischevo.Wahha.Web.Routing
{
    public interface IRoutableHttpHandler : IHttpHandler
    {
        #region Instance Properties
        RequestContext RequestContext { get; set; }
        #endregion
    }
}

using System;

namespace Radischevo.Wahha.Web.Mvc
{
    public interface IAuthorizationFilter
    {
        void OnAuthorization(AuthorizationContext context);
    }
}

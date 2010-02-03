using System;
using System.Linq;
using System.Security.Principal;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class RequireRolesAttribute : RequireAuthorizationAttribute
    {
        #region Instance Fields
        private string[] _roles;
        #endregion

        #region Constructors
        public RequireRolesAttribute(params string[] roles)
        {
            Precondition.Require(roles, Error.ArgumentNull("roles"));
            _roles = roles;
        }
        #endregion

        #region Instance Properties
        protected override ActionResult DefaultResult
        {
            get
            {
                return new HttpForbiddenResult(false);
            }
        }
        #endregion

        #region Instance Methods
        protected override bool Validate(HttpContextBase context)
        {
            bool isAuthorized = base.Validate(context);
            if (!isAuthorized)
                return false;

            if (_roles == null || _roles.Length < 1)
                return true;

            IPrincipal user = context.User;
            return _roles.Any(role => user.IsInRole(role));
        }
        #endregion
    }
}

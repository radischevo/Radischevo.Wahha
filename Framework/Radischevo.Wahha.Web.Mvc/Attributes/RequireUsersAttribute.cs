using System;
using System.Linq;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class RequireUsersAttribute : RequireAuthorizationAttribute
    {
        #region Instance Fields
        private string[] _users;
        #endregion

        #region Constructors
        public RequireUsersAttribute(params string[] users)
        {
            Precondition.Require(users, Error.ArgumentNull("users"));
            _users = users;
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

            if (_users == null || _users.Length < 1)
                return true;

            string userName = context.User.Identity.Name;
            return _users.Any(s => s.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }
        #endregion
    }
}

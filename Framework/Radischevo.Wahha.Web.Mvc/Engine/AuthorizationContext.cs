using System;
using System.Reflection;

using Radischevo.Wahha.Core;
using System.Security.Principal;

namespace Radischevo.Wahha.Web.Mvc
{
    public class AuthorizationContext : ActionContext
    {
        #region Instance Fields
        private bool _cancel;
        #endregion

        #region Constructors
        public AuthorizationContext(ControllerContext context, ActionDescriptor action)
            : base(context, action)
        {
        }
        #endregion

        #region Instance Properties
        public IPrincipal User
        {
            get
            {
                if (base.Context == null)
                    return null;

                return base.Context.User;
            }
        }

        public bool Cancel
        {
            get
            {
                return _cancel;
            }
            set
            {
                _cancel = value;
            }
        }
        #endregion
    }
}

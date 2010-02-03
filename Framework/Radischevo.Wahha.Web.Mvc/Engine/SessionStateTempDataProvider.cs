using System;
using System.Collections.Generic;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
    public class SessionStateTempDataProvider : ITempDataProvider
    {
        #region Static Fields
        internal const string SessionStateKey = "Radischevo.Wahha.Web.Mvc.ControllerTempData";
        #endregion

        #region Instance Methods
        public virtual IDictionary<string, object> Load(ControllerContext context)
        {
            HttpContextBase ctx = context.Context;
            Precondition.Require(ctx.Session, Error.SessionStateDisabled());

            Dictionary<string, object> data = (ctx.Session[SessionStateKey] as Dictionary<string, object>);
            if (data == null)
                data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            else
                ctx.Session.Remove(SessionStateKey);

            return data;
        }

        public virtual void Save(ControllerContext context, IDictionary<string, object> values)
        {
            HttpContextBase ctx = context.Context;
            Precondition.Require(ctx.Session, Error.SessionStateDisabled());

            ctx.Session[SessionStateKey] = values;
        }
        #endregion
    }
}

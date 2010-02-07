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
            HttpSessionStateBase session = context.Context.Session;

			if (session != null)
			{
				Dictionary<string, object> data = (session[SessionStateKey] as Dictionary<string, object>);
				if (data != null)
					session.Remove(SessionStateKey);
			}
            return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public virtual void Save(ControllerContext context, IDictionary<string, object> values)
        {
            HttpSessionStateBase session = context.Context.Session;
			bool isDirty = (values != null && values.Count > 0);

			if (session == null)
			{
				if (isDirty)
					throw Error.SessionStateDisabled();
			}
			else
			{
				if (isDirty)
					session[SessionStateKey] = values;
				else
				{
					if (session[SessionStateKey] != null)
						session.Remove(SessionStateKey);
				}
			}
        }
        #endregion
    }
}

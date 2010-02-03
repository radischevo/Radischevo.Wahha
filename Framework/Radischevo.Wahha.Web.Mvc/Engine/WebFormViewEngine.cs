using System;
using System.Collections.Generic;
using System.Globalization;

namespace Radischevo.Wahha.Web.Mvc
{
    public class WebFormViewEngine : VirtualPathViewEngine
    {
        #region Constructors
        public WebFormViewEngine() 
            : base("~/{0}.aspx", "~/{0}.ascx", 
                 "~/Views/{0}.aspx", "~/Views/{0}.ascx", 
                 "~/Views/{1}/{0}.aspx", "~/Views/{1}/{0}.ascx", 
                 "~/Views/Shared/{0}.aspx", "~/Views/Shared/{0}.ascx")
        {   }
        #endregion

        #region Instance Methods
        protected override ViewEngineResult CreateView(ControllerContext context, string viewName)
        {
            string path = base.GetViewPath(context, viewName);
            if (String.IsNullOrEmpty(path))
                return null;

            return new ViewEngineResult(new WebFormView(path), this);
        }
        #endregion
    }
}

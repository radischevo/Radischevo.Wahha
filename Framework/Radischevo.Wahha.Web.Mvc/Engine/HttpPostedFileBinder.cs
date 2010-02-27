using System;
using System.Collections.Generic;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
    public class HttpPostedFileBinder : IModelBinder
    {
        #region Constructors
        public HttpPostedFileBinder()
        {   }
        #endregion

        #region Instance Methods
        public object Bind(BindingContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));

            HttpPostedFileBase file = context.Context.Request.Files[context.ModelName];
            if (file == null)
                return null;
            
            if (file.ContentLength == 0 && String.IsNullOrEmpty(file.FileName))
                return null;
            
            return file;
        }
        #endregion
    }
}

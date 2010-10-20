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
		protected virtual HttpPostedFileBase ExtractFile(string name, 
			HttpFileCollectionBase collection)
		{
			HttpPostedFileBase file = collection[name];
			if (file == null)
				return null;

			if (file.ContentLength == 0 && 
				String.IsNullOrEmpty(file.FileName))
				return null;

			return file;
		}

		public object Bind(BindingContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));

			return ExtractFile(context.ModelName, context.Context.Request.Files);
        }
        #endregion
    }
}

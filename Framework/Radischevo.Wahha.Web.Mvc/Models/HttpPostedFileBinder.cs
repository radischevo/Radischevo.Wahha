using System;

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

			ValueProviderResult result = context.ValueProvider.GetValue(context.ModelName);
			if (result == null)
				return null;

			return result.GetValue<HttpPostedFileBase>();
        }
        #endregion
    }
}

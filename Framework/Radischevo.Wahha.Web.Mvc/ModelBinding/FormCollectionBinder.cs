using System;
using System.Collections.Generic;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
    public class FormCollectionBinder : IModelBinder
    {
        #region Constructors
        public FormCollectionBinder()
        {   }
        #endregion

        #region Instance Methods
        public object Bind(BindingContext context)
        {
            // Заполняем FormCollection теми данными, 
            // что были использованы как источник в 
            // атрибуте BindAttribute.

            Precondition.Require(context, () => Error.ArgumentNull("context"));
            return new FormCollection(context.Data);
        }
        #endregion
    }
}

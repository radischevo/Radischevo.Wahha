using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Web.Mvc
{
    public interface IModelBinder
    {
        #region Instance Methods
        object Bind(BindingContext context);
        #endregion
    }
}

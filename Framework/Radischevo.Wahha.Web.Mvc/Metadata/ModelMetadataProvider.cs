using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public abstract class ModelMetadataProvider
    {
        #region Constructors
        protected ModelMetadataProvider()
        {
        }
        #endregion

        #region Instance Methods
        public virtual void Init()
        {
        }

        public abstract ModelMetadata GetMetadata(Type modelType);
        #endregion
    }
}

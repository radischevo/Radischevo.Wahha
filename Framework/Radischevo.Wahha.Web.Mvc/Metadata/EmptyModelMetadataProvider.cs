using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Web.Mvc
{
    public sealed class EmptyModelMetadataProvider : ModelMetadataProvider
    {
        #region Constructors
        public EmptyModelMetadataProvider()
            : base()
        {
        }
        #endregion

        #region Instance Methods
        public override ModelMetadata GetMetadata(Type modelType)
        {
            return new ModelMetadata(modelType, obj => obj);
        }
        #endregion
    }
}

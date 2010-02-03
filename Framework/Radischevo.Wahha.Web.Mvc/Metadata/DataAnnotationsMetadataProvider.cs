using System;

namespace Radischevo.Wahha.Web.Mvc
{
    public class DataAnnotationsMetadataProvider : ModelMetadataProvider
    {
        #region Constructors
        public DataAnnotationsMetadataProvider()
            : base()
        {
        }
        #endregion

        #region Instance Methods
        public override ModelMetadata GetMetadata(Type modelType)
        {
            return new DataAnnotationsModelMetadata(modelType, obj => obj);
        }
        #endregion
    }
}

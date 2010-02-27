using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Validation
{
    public class DataAnnotationsValidatorProvider : ModelValidatorProvider
    {
        #region Constructors
        public DataAnnotationsValidatorProvider()
            : base()
        {
        }
        #endregion

        #region Instance Methods
        public override ModelValidator GetValidator(Type type)
        {
            Precondition.Require(type, () => Error.ArgumentNull("type"));
            return new DataAnnotationsModelValidator(type);
        }
        #endregion
    }
}

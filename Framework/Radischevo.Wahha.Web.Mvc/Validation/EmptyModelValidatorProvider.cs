using System;

namespace Radischevo.Wahha.Web.Mvc.Validation
{
    public sealed class EmptyModelValidatorProvider : ModelValidatorProvider
    {
        #region Constructors
        public EmptyModelValidatorProvider()
            : base()
        {
        }
        #endregion

        #region Instance Methods
        public override ModelValidator GetValidator(Type type)
        {
            return new ModelValidator(type);
        }
        #endregion
    }
}

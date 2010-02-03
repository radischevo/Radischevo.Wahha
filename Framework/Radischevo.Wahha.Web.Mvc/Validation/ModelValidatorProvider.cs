using System;

namespace Radischevo.Wahha.Web.Mvc.Validation
{
    public abstract class ModelValidatorProvider
    {
        #region Constructors
        protected ModelValidatorProvider()
        {
        }
        #endregion

        #region Instance Methods
        public virtual void Init()
        {
        }

        public abstract ModelValidator GetValidator(Type type);
        #endregion
    }
}

using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Interface | 
        AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Class | 
		AttributeTargets.Property | AttributeTargets.Parameter, 
		AllowMultiple = false, Inherited = false)]
    public class ModelBinderAttribute : Attribute
    {
        #region Instance Fields
        private Type _binderType;
        #endregion
        
        #region Constructors
		protected ModelBinderAttribute()
		{
		}

        public ModelBinderAttribute(Type binderType)
        {
            Precondition.Require(binderType, () => Error.ArgumentNull("binderType"));
            _binderType = binderType;
        }
        #endregion
        
        #region Instance Properties
        public Type BinderType
        {
            get
            {
                return _binderType;
            }
        }
        #endregion

        #region Instance Methods
        public virtual IModelBinder GetBinder()
        {
			if (_binderType == null)
				throw Error.MustOverrideGetBinderToUseEmptyType();

            if (!typeof(IModelBinder).IsAssignableFrom(_binderType))
                throw Error.IncompatibleModelBinderType(_binderType);

			return (IModelBinder)ServiceLocator.Instance.GetService(_binderType);
        }
        #endregion
    }
}

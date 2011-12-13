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
        private Type _type;
        #endregion
        
        #region Constructors
		protected ModelBinderAttribute()
		{
		}

        public ModelBinderAttribute(Type type)
        {
            Precondition.Require(type, () => Error.ArgumentNull("type"));
            _type = type;
        }
        #endregion
        
        #region Instance Properties
        public Type Type
        {
            get
            {
                return _type;
            }
        }
        #endregion

        #region Instance Methods
        public virtual IModelBinder GetBinder()
        {
			if (_type == null)
				throw Error.MustOverrideGetBinderToUseEmptyType();

            if (!typeof(IModelBinder).IsAssignableFrom(_type))
                throw Error.IncompatibleModelBinderType(_type);

			return (IModelBinder)ServiceLocator.Instance.GetService(_type);
        }
        #endregion
    }
}

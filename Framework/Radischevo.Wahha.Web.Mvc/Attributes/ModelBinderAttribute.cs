using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Interface | 
        AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Class, 
        AllowMultiple = false, Inherited = false)]
    public class ModelBinderAttribute : Attribute
    {
        #region Instance Fields
        private Type _binderType;
        #endregion
        
        #region Constructors
        public ModelBinderAttribute(Type binderType)
        {
            Precondition.Require(binderType, Error.ArgumentNull("binderType"));
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
            if (_binderType.GetInterface(typeof(IModelBinder).Name) == null)
                throw Error.IncompatibleModelBinderType(_binderType);

            if (_binderType.GetConstructor(Type.EmptyTypes) == null)
                throw Error.ModelBinderMustHaveDefaultConstructor(_binderType);

            return (IModelBinder)Activator.CreateInstance(_binderType);
        }
        #endregion
    }
}

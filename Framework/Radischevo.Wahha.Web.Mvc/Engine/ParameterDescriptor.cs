using System;
using System.Reflection;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public abstract class ParameterDescriptor : ICustomAttributeProvider
    {
        #region Nested Types
        private sealed class EmptyParameterBinding : ParameterBinding
        {
            public EmptyParameterBinding()
                : base()
            {
            }
        }
        #endregion

        #region Static Fields
        private static readonly ParameterBinding _emptyBinding = new EmptyParameterBinding();
        #endregion

        #region Constructors
        protected ParameterDescriptor()
        {   }
        #endregion

        #region Instance Properties
        public abstract ActionDescriptor Action 
        { 
            get; 
        }
 
        public virtual ParameterBinding Binding
        {
            get
            {
                return _emptyBinding;
            }
        }

        public abstract ParameterInfo Parameter
        {
            get;
        }

        public abstract string Name 
        { 
            get; 
        }

        public abstract Type Type 
        { 
            get; 
        }
        #endregion

        #region Instance Methods
        public virtual object[] GetCustomAttributes(bool inherit)
        {
            return GetCustomAttributes(typeof(object), inherit);
        }

        public virtual object[] GetCustomAttributes(Type type, bool inherit)
        {
            Precondition.Require(type, () => Error.ArgumentNull("type"));   
            return (object[])Array.CreateInstance(type, 0);
        }

        public virtual bool IsDefined(Type type, bool inherit)
        {
            Precondition.Require(type, () => Error.ArgumentNull("type"));
            return false;
        }
        #endregion
    }
}

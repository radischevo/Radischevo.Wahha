using System;
using System.Collections.Generic;
using System.Reflection;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public abstract class ControllerDescriptor : ICustomAttributeProvider
    {
        #region Constructors
        protected ControllerDescriptor()
        {   }
        #endregion

        #region Instance Properties
        public virtual string Name
        {
            get
            {
                return (Type == null) ? String.Empty : Type.Name;
            }
        }

        public abstract Type Type 
        { 
            get; 
        }
        #endregion

        #region Instance Methods
        public abstract ActionDescriptor FindAction(ControllerContext context, string name);

        public abstract IEnumerable<ActionDescriptor> GetCanonicalActions();

        public virtual object[] GetCustomAttributes(bool inherit)
        {
            return GetCustomAttributes(typeof(object), inherit);
        }

        public virtual object[] GetCustomAttributes(Type type, bool inherit)
        {
            Precondition.Require(type, Error.ArgumentNull("type"));
            return (object[])Array.CreateInstance(type, 0);
        }

        public virtual bool IsDefined(Type type, bool inherit)
        {
            Precondition.Require(type, Error.ArgumentNull("type"));
            return false;
        }
        #endregion
    }
}

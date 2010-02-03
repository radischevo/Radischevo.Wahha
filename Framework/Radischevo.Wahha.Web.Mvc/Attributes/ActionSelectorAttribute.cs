using System;
using System.Reflection;

namespace Radischevo.Wahha.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public abstract class ActionSelectorAttribute : Attribute
    {
        #region Instance Methods
        public abstract bool IsValid(ControllerContext context, MethodInfo actionMethod);
        #endregion
    }
}

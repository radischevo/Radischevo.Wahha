using System;
using System.Reflection;

namespace Radischevo.Wahha.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class IgnoreActionAttribute : Attribute
    {
        #region Constructors
        public IgnoreActionAttribute()
        { }
        #endregion
    }
}

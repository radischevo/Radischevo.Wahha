using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ActionNameAttribute : Attribute
    {
        #region Instance Fields
        private string _name;
        #endregion

        #region Constructors
        public ActionNameAttribute(string name)
        {
            Precondition.Defined(name, () => Error.ArgumentNull("name"));
            _name = name;
        }
        #endregion

        #region Instance Properties
        public string Name
        {
            get
            {
                return _name;
            }
        }        
        #endregion
    }
}

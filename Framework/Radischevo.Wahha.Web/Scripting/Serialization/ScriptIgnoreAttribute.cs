using System;

namespace Radischevo.Wahha.Web.Scripting.Serialization
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ScriptIgnoreAttribute : Attribute
    {
        #region Constructors
        public ScriptIgnoreAttribute() { }
        #endregion
    }
}

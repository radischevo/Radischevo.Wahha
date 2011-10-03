using System;

namespace Radischevo.Wahha.Data.Serialization
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ScriptIgnoreAttribute : Attribute
    {
        #region Constructors
        public ScriptIgnoreAttribute() { }
        #endregion
    }
}

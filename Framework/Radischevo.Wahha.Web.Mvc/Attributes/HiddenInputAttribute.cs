using System;

namespace Radischevo.Wahha.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, 
        AllowMultiple = false, Inherited = true)]
    public sealed class HiddenInputAttribute : Attribute
    {
        #region Instance Fields
        private bool _displayValue;
        #endregion

        #region Constructors
        public HiddenInputAttribute()
        {
            _displayValue = true;
        }
        #endregion

        #region Instance Properties
        public bool DisplayValue
        {
            get
            {
                return _displayValue;
            }
            set
            {
                _displayValue = value;
            }
        }
        #endregion
    }
}

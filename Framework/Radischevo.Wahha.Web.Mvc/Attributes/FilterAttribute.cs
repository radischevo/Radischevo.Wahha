using System;

namespace Radischevo.Wahha.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Class |
        AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class FilterAttribute : Attribute
    {
        #region Instance Fields
        private int _order;
        #endregion

        #region Constructors
        public FilterAttribute()
        {
            _order = -1;
        }
        #endregion

        #region Instance Properties
        public int Order
        {
            get
            {
                return _order;
            }
            set
            {
                if (value < -1)
                    throw new ArgumentOutOfRangeException("order");
                _order = value;
            }
        }
        #endregion
    }
}

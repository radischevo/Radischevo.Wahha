using System;
using System.ComponentModel;

namespace Radischevo.Wahha.Web.Mvc
{
    public class ViewDataInfo
    {
        #region Instance Fields
        private object _container;
        private PropertyDescriptor _descriptor;
        private object _value;
        #endregion

        #region Constructors
        public ViewDataInfo()
        {
        }
        #endregion

        #region Instance Properties
        public object Container
        {
            get
            {
                return _container;
            }
            set
            {
                _container = value;
            }
        }

        public PropertyDescriptor Descriptor
        {
            get
            {
                return _descriptor;
            }
            set
            {
                _descriptor = value;
            }
        }

        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
        #endregion
    }
}

using System;
using System.ComponentModel;
using System.Web.UI;

namespace Radischevo.Wahha.Web.Mvc.Html.Forms
{
    [ControlBuilder(typeof(ViewTypeControlBuilder)), 
     NonVisualControl]
    public class ViewType : Control
    {
        #region Instance Fields
        private string _typeName;
        #endregion

        #region Constructors
        public ViewType()
        {   }
        #endregion

        #region Instance Properties
        [DefaultValue("")]
        public string TypeName
        {
            get
            {
                return _typeName ?? String.Empty;
            }
            set
            {
                _typeName = value;
            }
        }
        #endregion
    }
}

using System;

namespace Radischevo.Wahha.Web.Mvc
{
    internal sealed class RedirectData
    {
        #region Instance Fields
        private string _controller;
        private string _action;
        #endregion

        #region Constructors
        public RedirectData()
        {
        }

        public RedirectData(string controller, 
            string action)
        {
            _controller = controller;
            _action = action;
        }
        #endregion

        #region Instance Properties
        public string Controller
        {
            get
            {
                return _controller;
            }
            set
            {
                _controller = value;
            }
        }

        public string Action
        {
            get
            {
                return _action;
            }
            set
            {
                _action = value;
            }
        }
        #endregion
    }
}

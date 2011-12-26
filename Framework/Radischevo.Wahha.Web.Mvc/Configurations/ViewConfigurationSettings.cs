using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Web.Mvc.Configurations
{
    public sealed class ViewConfigurationSettings
    {
        #region Instance Fields
        private ViewEngineCollection _viewEngines;
        #endregion

        #region Constructors
        internal ViewConfigurationSettings()
        {
            _viewEngines = new ViewEngineCollection();
        }
        #endregion

        #region Instance Properties
        public ViewEngineCollection ViewEngines
        {
            get
            {
                return _viewEngines;
            }
        }
        #endregion
    }
}

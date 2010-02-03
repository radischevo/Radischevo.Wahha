using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Routing
{
    internal sealed class BoundUrl
    {
        #region Instance Fields
        private string _url;
        private ValueDictionary _values;
        #endregion

        #region Constructors
        public BoundUrl(string url, ValueDictionary values)
        {
            _url = url;
            _values = values;
        }
        #endregion

        #region Instance Properties
        public string Url
        {
            get
            {
                return _url;
            }
        }

        public ValueDictionary Values
        {
            get
            {
                return _values;
            }
        }
        #endregion
    }
}

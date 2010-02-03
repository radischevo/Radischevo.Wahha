using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Routing
{
    /// <summary>
    /// Contains the information about 
    /// an URL route and its virtual path
    /// </summary>
    public class VirtualPathData
    {
        #region Instance Fields
        private string _virtualPath;
        private Route _route;
        private ValueDictionary _tokens;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="VirtualPathData"/> class
        /// </summary>
        /// <param name="route">An URL route</param>
        /// <param name="virtualPath">A virtual path string, 
        /// related to the supplied route</param>
        public VirtualPathData(Route route, string virtualPath)
        {
            _route = route;
            _virtualPath = virtualPath;
            _tokens = new ValueDictionary();
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets the URL route, 
        /// stored in this instance
        /// </summary>
        public Route Route
        {
            get
            {
                return _route;
            }
        }

        public ValueDictionary Tokens
        {
            get
            {
                return _tokens;
            }
        }

        /// <summary>
        /// Gets or sets the virtual path, 
        /// stored in this instance
        /// </summary>
        public string VirtualPath
        {
            get
            {
                return (_virtualPath == null) ? 
                    String.Empty : _virtualPath;
            }
            set
            {
                _virtualPath = value;
            }
        }
        #endregion
    }
}

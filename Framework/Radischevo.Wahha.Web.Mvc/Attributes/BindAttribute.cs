using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radischevo.Wahha.Web.Mvc
{
    /// <summary>
    /// Attribute used to provide details on 
    /// how model binding to a parameter should occur.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter,
        AllowMultiple = false, Inherited = false)]
    public sealed class BindAttribute : Attribute
    {
        #region Instance Fields
        private string _name;
        private string _source;
        private object _default;
        #endregion

        #region Constructors
        public BindAttribute() 
            : this(null)
        {   }

        public BindAttribute(string name)
        {
            _name = name;
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets or sets the prefix to use when 
        /// binding to an action argument or model property.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// Gets or sets the default value for the parameter.
        /// </summary>
        public object Default
        {
            get
            {
                return _default;
            }
            set
            {
                _default = value;
            }
        }

        /// <summary>
        /// Gets or sets comma delimited list of one or more
        /// parameter source names specifying locations 
		/// where the parameter value is searched.
        /// </summary>
        public string Source
        {
            get
            {
                return _source;
            }
			set
			{
				_source = value;
			}
        }
        #endregion
    }
}

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
        private string[] _blackList;
        private string[] _whiteList;
        private object _default;
        #endregion

        #region Constructors
        public BindAttribute() 
            : this(null)
        {   }

        public BindAttribute(string name)
        {
            _name = name;
            _blackList = new string[0];
            _whiteList = new string[0];
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// A comma delimited black list of property 
        /// names for which binding is not allowed.
        /// </summary>
        public string Exclude
        {
            get
            {
                return String.Join(",", _blackList);
            }
            set
            {
                _blackList = (String.IsNullOrEmpty(value))
                    ? new string[0]
                    : value.Split(new char[] { ',' }, 
                        StringSplitOptions.RemoveEmptyEntries);
            }
        }

        /// <summary>
        /// A comma delimited white list of property 
        /// names for which binding is allowed.
        /// </summary>
        public string Include
        {
            get
            {
                return String.Join(",", _whiteList);
            }
            set
            {
                _whiteList = (String.IsNullOrEmpty(value))
                    ? new string[0]
                    : value.Split(new char[] { ',' },
                        StringSplitOptions.RemoveEmptyEntries);
            }
        }

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
        /// <see cref="Radischevo.Wahha.Web.Mvc.ParameterSource"/> values 
        /// specifying locations where the parameter value is searched.
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

        #region Static Methods
        public static bool IsUpdateAllowed(string memberName, string[] whiteList, string[] blackList)
        {
            if (String.IsNullOrEmpty(memberName))
                return false;

            bool include = (whiteList == null || whiteList.Length == 0 || whiteList.Contains(memberName, StringComparer.OrdinalIgnoreCase));
            bool exclude = (blackList != null && blackList.Contains(memberName, StringComparer.OrdinalIgnoreCase));

            return (include && !exclude);
        }
        #endregion

        #region Instance Methods
        public bool IsUpdateAllowed(string memberName)
        {
            return IsUpdateAllowed(memberName, _whiteList, _blackList);
        }
        #endregion
    }
}

using System;
using System.ComponentModel.DataAnnotations;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Validation
{
    /// <summary>
    /// Specifies a custom rule validation constraint 
    /// for the value of a data field.
    /// </summary>
    public class RemoteValidationAttribute : ValidationAttribute
    {
        #region Instance Fields
        private string _url;
        #endregion

        #region Constructors
        public RemoteValidationAttribute(string url)
        {
            Url = url;
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets or sets the URL of the handler, 
        /// which will be used to validate the value 
        /// of a data field.
        /// </summary>
        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                Precondition.Defined(value, () => Error.ArgumentNull("value"));
                _url = value;
            }
        }
        #endregion

        #region Instance Methods
        public override bool IsValid(object value)
        {
            // This is the default behaviour 
            // for server-side validation, and it 
            // can be overridden in a dervied class.
            return true;
        }
        #endregion
    }
}

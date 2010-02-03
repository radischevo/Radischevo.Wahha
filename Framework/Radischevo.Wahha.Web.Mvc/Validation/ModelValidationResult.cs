using System;

namespace Radischevo.Wahha.Web.Mvc.Validation
{
    public class ModelValidationResult
    {
        #region Instance Fields
        private string _member;
        private string _message;
        #endregion

        #region Constructors
        public ModelValidationResult()
        {
        }
        #endregion

        #region Instance Properties
        public string Member
        {
            get
            {
                return _member;
            }
            set
            {
                _member = value;
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
            }
        }
        #endregion
    }
}

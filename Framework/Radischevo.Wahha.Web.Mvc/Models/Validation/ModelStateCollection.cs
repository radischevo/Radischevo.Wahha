using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data;

namespace Radischevo.Wahha.Web.Mvc
{
    /// <summary>
    /// A collection of <see cref="Radischevo.Wahha.Web.Mvc.ValidationError"/> instances.
    /// </summary>
    public sealed class ModelStateCollection
    {
        #region Instance Fields
        private ModelErrorCollection _errors;
        #endregion

        #region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Radischevo.Wahha.Web.Mvc.ModelStateCollection"/> class.
		/// </summary>
        public ModelStateCollection()
        {
            _errors = new ModelErrorCollection();
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Adds a validation error message to the 
        /// <see cref="Radischevo.Wahha.Web.Mvc.ModelStateCollection"/>
        /// </summary>
        public void Add(string key, string message)
        {
            Add(key, null, message);
        }

        public void Add(string key, Exception exception)
        {
            Add(key, null, exception);
        }
		
		public void Add(string key, object attemptedValue, string message)
		{
			Add(key, attemptedValue, message, null);
		}
		
		public void Add(string key, object attemptedValue, Exception exception)
		{
			Add(key, attemptedValue, null, exception);
		}

        public void Add(string key, object attemptedValue, string message, Exception exception)
        {
            Add(key, new ValidationError(key, attemptedValue, message, exception));
        }

        /// <summary>
        /// Adds a validation error to the 
        /// <see cref="Radischevo.Wahha.Web.Mvc.ModelStateCollection"/>
        /// </summary>
        public void Add(string key, ValidationError error)
        {
			_errors.Add(key, error);
        }

		public bool IsValid()
		{
			return (_errors.Count == 0);
		}

		public bool IsValid(string key)
		{
			return !_errors.Contains(key);
		}

		public void Clear()
		{
			_errors.Clear();
		}

        public void Clear(string key)
        {
            _errors.Clear(key);
        }
        #endregion
    }
}

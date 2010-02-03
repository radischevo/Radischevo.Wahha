using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;
using System.Collections;

namespace Radischevo.Wahha.Web.Mvc
{
    /// <summary>
    /// A collection of <see cref="Radischevo.Wahha.Web.Mvc.ValidationError"/> instances.
    /// </summary>
    public sealed class ValidationErrorCollection : IEnumerable, 
        IEnumerable<KeyValuePair<string, ValidationError>>
    {
        #region Instance Fields
        private Dictionary<string, ValidationError> _errors;
        #endregion

        #region Constructors
        public ValidationErrorCollection()
        {
            _errors = new Dictionary<string, ValidationError>(
                StringComparer.OrdinalIgnoreCase);
        }
        #endregion

        #region Instance Properties
        public int Count
        {
            get
            {
                return _errors.Count;
            }
        }

        public ValidationError this[string key]
        {
            get
            {
                return _errors[key];
            }
            set
            {
                _errors[key] = value;
            }
        }

        public bool IsValid
        {
            get
            {
                return (_errors.Count == 0);
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return _errors.Keys;
            }
        }

        public IEnumerable<ValidationError> Values
        {
            get
            {
                return _errors.Values;
            }
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Adds a validation error message to the 
        /// <see cref="Radischevo.Wahha.Web.Mvc.ValidationErrorCollection"/>
        /// </summary>
        public void Add(string key, string message)
        {
            Add(key, new ValidationError(message, null));
        }

        public void Add(string key, Exception exception)
        {
            Add(key, new ValidationError(exception));
        }

        public void Add(string key, string message, Exception exception)
        {
            Add(key, new ValidationError(message, exception));
        }

        /// <summary>
        /// Adds a validation error to the 
        /// <see cref="Radischevo.Wahha.Web.Mvc.ValidationErrorCollection"/>
        /// </summary>
        public void Add(string key, ValidationError error)
        {
            Precondition.Require(key, Error.InvalidArgument("key"));
            Precondition.Require(error, Error.ArgumentNull("error"));

            _errors[key] = error;
        }

        public bool ContainsKey(string key)
        {
            return _errors.ContainsKey(key);
        }

        public bool TryGetError(string key, out ValidationError error)
        {
            return _errors.TryGetValue(key, out error);
        }

        public void Remove(string key)
        {
            _errors.Remove(key);
        }

        public ValidationErrorCollection Merge(ValidationErrorCollection errors)
        {
            if (errors == null)
                return this;

            foreach (KeyValuePair<string, ValidationError> kvp in errors)
                this[kvp.Key] = kvp.Value;

            return this;
        }

        public Dictionary<string, ValidationError>.Enumerator GetEnumerator()
        {
            return _errors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        IEnumerator<KeyValuePair<string, ValidationError>> 
            IEnumerable<KeyValuePair<string, ValidationError>>.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }

    public class ValidationError
    {
        #region Instance Fields
        private object _attemptedValue;
        private string _message;
        private Exception _error;
        #endregion

        #region Constructors
        public ValidationError(Exception error) 
            : this(error.Message, null, error)
        {
        }

        public ValidationError(string message, Exception error)
            : this(message, null, error)
        {
        }

        public ValidationError(string message, 
            object attemptedValue, Exception error)
        {
            _message = message;
            _error = error;
            _attemptedValue = attemptedValue;
        }
        #endregion

        #region Instance Properties
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

        public Exception Error
        {
            get
            {
                return _error;
            }
            set
            {
                _error = value;
            }
        }

        public object AttemptedValue
        {
            get
            {
                return _attemptedValue;
            }
            set 
            {
                _attemptedValue = value;
            }
        }
        #endregion
    }
}

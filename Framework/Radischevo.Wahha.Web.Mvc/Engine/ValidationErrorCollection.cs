using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    /// <summary>
    /// A collection of <see cref="Radischevo.Wahha.Web.Mvc.ValidationError"/> instances.
    /// </summary>
    public sealed class ValidationErrorCollection : IEnumerable, IEnumerable<ValidationError>
    {
        #region Instance Fields
        private Dictionary<string, ICollection<ValidationError>> _errors;
        #endregion

        #region Constructors
        public ValidationErrorCollection()
        {
            _errors = new Dictionary<string, ICollection<ValidationError>>(
				StringComparer.OrdinalIgnoreCase);
        }
        #endregion

        #region Instance Properties
        public int Count
        {
            get
            {
				int count = 0;
				foreach (string key in _errors.Keys)
					count += _errors[key].Evaluate(c => c.Count, 0);
			
                return count;
            }
        }

        public IEnumerable<ValidationError> this[string key]
        {
            get
            {
				ICollection<ValidationError> list;
				_errors.TryGetValue(key, out list);

                return (list ?? Enumerable.Empty<ValidationError>());
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return _errors.Keys;
            }
        }
        #endregion

        #region Instance Methods
		private ICollection<ValidationError> GetOrCreateItem(string key)
		{
			ICollection<ValidationError> list;
			if (!_errors.TryGetValue(key, out list) || list == null)
				_errors[key] = list = new List<ValidationError>();

			return list;
		}

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
            Precondition.Require(key, () => Error.InvalidArgument("key"));
            Precondition.Require(error, () => Error.ArgumentNull("error"));

			ICollection<ValidationError> list = GetOrCreateItem(key);
			error.Member = key;

			list.Add(error);
        }

		public bool IsValid()
		{
			return (_errors.Count == 0);
		}

		public bool IsValid(string key)
		{
			ICollection<ValidationError> list;
			if (_errors.TryGetValue(key, out list))
				return (list.Count < 1);
			
			return true;
		}

		public void Clear()
		{
			_errors.Clear();
		}

        public void Clear(string key)
        {
            _errors.Remove(key);
        }

        public ValidationErrorCollection Merge(ValidationErrorCollection errors)
        {
            if (errors == null)
                return this;

			foreach (string key in errors.Keys)
			{
				ICollection<ValidationError> list = GetOrCreateItem(key);
				foreach (ValidationError error in errors[key])
					list.Add(error);
			}
            return this;
        }

        public IEnumerator<ValidationError> GetEnumerator()
        {
            return _errors.SelectMany(c => c.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }

    public class ValidationError
    {
        #region Instance Fields
		private string _member;
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
		public string Member
		{
			get
			{
				return _member ?? String.Empty;
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

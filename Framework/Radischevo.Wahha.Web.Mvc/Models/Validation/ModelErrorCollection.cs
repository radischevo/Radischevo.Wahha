using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data;

namespace Radischevo.Wahha.Web.Mvc
{
    /// <summary>
    /// A collection of <see cref="Radischevo.Wahha.Web.Mvc.ValidationError"/> instances 
    /// related to the current model binding state.
    /// </summary>
    public sealed class ModelErrorCollection : IEnumerable<ValidationError>
	{
        #region Instance Fields
        private Dictionary<string, ICollection<ValidationError>> _errors;
        #endregion

        #region Constructors
        public ModelErrorCollection()
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
        /// Adds a validation error to the 
        /// <see cref="Radischevo.Wahha.Web.Mvc.ModelErrorCollection"/>
        /// </summary>
        public void Add(string key, ValidationError error)
        {
            Precondition.Require(key, () => Error.InvalidArgument("key"));
            Precondition.Require(error, () => Error.ArgumentNull("error"));

			ICollection<ValidationError> list = GetOrCreateItem(key);
			list.Add(error);
        }
		
		/// <summary>
		/// Gets a value indicating whether the current 
		/// collection contains a record with the specified key.
		/// </summary>
		/// <param name="key">The key to find.</param> 
		public bool Contains(string key)
		{
			return _errors.ContainsKey(key);
		}
		
		/// <summary>
		/// Clears this instance.
		/// </summary>
		public void Clear()
		{
			_errors.Clear();
		}
		
		/// <summary>
		/// Removes all errors with the specified key.
		/// </summary>
		/// <param name="key">The key to remove.</param>
        public void Clear(string key)
        {
            _errors.Remove(key);
        }
		
		/// <summary>
		/// Adds all errors from the provided collection to the 
		/// current instance.
		/// </summary>
		/// <param name="errors">The collection containing errors to add.</param>
        public ModelErrorCollection Merge(ModelErrorCollection errors)
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
        
		public IEnumerator<ValidationError> GetEnumerator ()
		{
			return _errors.SelectMany(a => a.Value).GetEnumerator();
		}
		#endregion

		#region IEnumerable Methods
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator();
		}
		#endregion
    }
}

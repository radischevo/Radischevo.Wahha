using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;

namespace Radischevo.Wahha.Core
{
    /// <summary>
    /// Represents a collection of 
    /// keys and values.
    /// </summary>
	[Serializable, DebuggerDisplay("Count = {Count}")]
    public class ValueDictionary : Dictionary<string, object>, IValueSet
	{
		#region Constructors
		/// <summary>
        /// Initializes a new instance of the 
        /// <see cref="ValueDictionary"/> class
        /// </summary>
        public ValueDictionary()
            : base(StringComparer.OrdinalIgnoreCase)
        { }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="ValueDictionary"/> class
        /// </summary>
        /// <param name="dictionary">The collection, which keys and values 
        /// should be copied into a new dictionary</param>
        public ValueDictionary(IDictionary<string, object> dictionary)
            : base(dictionary, StringComparer.OrdinalIgnoreCase)
        { }

		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="ValueDictionary"/> class.
		/// </summary>
		/// <param name="collection">The collection, which keys and values 
		/// should be copied into a new dictionary.</param>
		public ValueDictionary(NameValueCollection collection)
			: base(StringComparer.OrdinalIgnoreCase)
		{
			Initialize(collection);
		}

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="ValueDictionary"/> class
        /// </summary>
        /// <param name="values">A value set, which keys and values 
        /// will be used to populate the dictionary</param>
        public ValueDictionary(IValueSet values)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            Initialize(values);
        }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="ValueDictionary"/> class
        /// </summary>
        /// <param name="values">An object, whose property values 
        /// will be used to populate the dictionary</param>
        public ValueDictionary(object values)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            Initialize(values);
        }

		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="ValueDictionary"/> class with serialized data
		/// </summary>
		/// <param name="info">An object containing the information required to 
		/// serialize the dictionary</param>
		/// <param name="context">A structure containing the source and destination 
		/// of the serialized stream associated with the dictionary</param>
		protected ValueDictionary(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
        #endregion

		#region Static Methods
		private static string NullKey(string key)
		{
			// since Dictionary implementation doesn't
			// allow null keys, we should make a substitution.
			return key ?? String.Empty;
		}
		#endregion

		#region Instance Methods
		private void SafeAdd(string key, object value)
		{
			base.Add(NullKey(key), value);
		}

		private void SafeInsert(string key, object value)
		{
			base[NullKey(key)] = value;
		}

		private bool SafeContainsKey(string key)
		{
			return base.ContainsKey(NullKey(key));
		}

		private bool SafeRemove(string key)
		{
			return base.Remove(NullKey(key));
		}

		private bool SafeTryGetValue(string key, out object value)
		{
			return base.TryGetValue(NullKey(key), out value);
		}

		/// <summary>
        /// Gets the typed value with the 
        /// specified key.
        /// </summary>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="key">The key to find.</param>
        /// <param name="defaultValue">The default value of the variable.</param>
		/// <param name="provider">An <see cref="IFormatProvider" /> interface implementation that 
		/// supplies culture-specific formatting information.</param>
        public TValue GetValue<TValue>(string key, TValue defaultValue, 
			IFormatProvider provider)
        {
            object value;
			
            if (!SafeTryGetValue(key, out value))
                return defaultValue;

            if (typeof(TValue) == typeof(bool) && value is string)
            {
                switch (((string)value).Define().ToLowerInvariant())
                {
                    case "on":
                    case "yes":
                    case "true":
                        value = true;
                        break;
                    default:
                        value = false;
                        break;
                }
            }
            return Converter.ChangeType<TValue>(value, defaultValue, provider);
        }

        /// <summary>
        /// Merges two collections and returns a result.
        /// </summary>
        /// <param name="values">An object, whose property values 
        /// will be used to populate the dictionary.</param>
        public ValueDictionary Merge(object values)
        {
            return Merge(values, true);
        }

        /// <summary>
        /// Merges two collections and returns a result.
        /// </summary>
        /// <param name="values">An object, whose property values 
        /// will be used to populate the dictionary.</param>
        /// <param name="replace">True to replace existing values.</param>
        public ValueDictionary Merge(object values, bool replace)
        {
			return Merge((IDictionary<string, object>)new ValueDictionary(values), replace);
        }

        /// <summary>
        /// Merges two collections and returns the result.
        /// </summary>
        /// <param name="dictionary">The collection, which keys and values 
        /// should be merged with the dictionary.</param>
        public ValueDictionary Merge(IDictionary<string, object> dictionary)
        {
            return Merge(dictionary, true);
        }

        /// <summary>
        /// Merges two collections and returns the result.
        /// </summary>
        /// <param name="dictionary">The collection, which keys and values 
        /// should be merged with the dictionary.</param>
        /// <param name="replace">True to replace existing values.</param>
        public ValueDictionary Merge(IDictionary<string, object> dictionary, bool replace)
        {
            if (dictionary == null)
                return this;

            foreach (KeyValuePair<string, object> kvp in dictionary)
            {
				if (SafeContainsKey(kvp.Key) && !replace)
                    continue;

                SafeInsert(kvp.Key, kvp.Value);
            }
            return this;
        }

        /// <summary>
        /// Merges two collections and returns the result.
        /// </summary>
        /// <param name="values">The collection, which keys and values 
        /// should be merged with the dictionary.</param>
        public ValueDictionary Merge(IValueSet values)
        {
            return Merge(values, true);
        }

        /// <summary>
        /// Merges two collections and returns the result.
        /// </summary>
        /// <param name="values">The collection, which keys and values 
        /// should be merged with the dictionary</param>
        /// <param name="replace">True to replace existing values.</param>
        public ValueDictionary Merge(IValueSet values, bool replace)
        {
            if (values == null)
                return this;

            foreach (string key in values.Keys)
            {
                if (SafeContainsKey(key) && !replace)
                    continue;

                SafeInsert(key, values[key]);
            }
            return this;
        }        

        protected void Initialize(NameValueCollection values)
        {
            if (values == null)
                return;

			foreach (string key in values.Keys)
				SafeAdd(key, values.Get(key));
        }

		protected void Initialize(IValueSet values)
		{
			if (values == null)
				return;

			foreach (string key in values.Keys)
				SafeAdd(key, values[key]);
		}

        protected void Initialize(object values)
        {
            if (values == null)
                return;

            foreach (KeyValuePair<string, object> kvp in
                Converter.ToDictionary(values))
                SafeAdd(kvp.Key, kvp.Value);
        }
        #endregion

        #region IValueSet Members
		IEnumerable<string> IValueSet.Keys
        {
            get 
            {
                return base.Keys;
            }
        }
        #endregion
	}
}

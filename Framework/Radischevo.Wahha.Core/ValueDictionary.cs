using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Radischevo.Wahha.Core
{
    /// <summary>
    /// Represents a collection of 
    /// keys and values
    /// </summary>
    [Serializable]
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
        /// <see cref="ValueDictionary"/> class with serialized data
        /// </summary>
        /// <param name="info">An object containing the information required to 
        /// serialize the dictionary</param>
        /// <param name="context">A structure containing the source and destination 
        /// of the serialized stream associated with the dictionary</param>
        protected ValueDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="ValueDictionary"/> class
        /// </summary>
        /// <param name="values">A value set, which keys and values 
        /// will be used to populate the dictionary</param>
        public ValueDictionary(IValueSet values)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            if (values != null)
                AddValues(values);
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
            if (values != null)
                AddValues(values);
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Gets the typed value with the 
        /// specified key
        /// </summary>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="key">The key to find</param>
        public TValue GetValue<TValue>(string key)
        {
            return GetValue<TValue>(key, default(TValue));
        }

        /// <summary>
        /// Gets the typed value with the 
        /// specified key
        /// </summary>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="key">The key to find</param>
        /// <param name="defaultValue">The default value of the variable</param>
        public TValue GetValue<TValue>(string key, TValue defaultValue)
        {
            object value;

            if (!base.TryGetValue(key, out value))
                return defaultValue;

            if (typeof(TValue) == typeof(bool)) // специальная методика для bool 
            {
                switch (value.ToString().ToLower())
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
            return Converter.ChangeType<TValue>(value, defaultValue);
        }

        /// <summary>
        /// Merges two collections and returns a result
        /// </summary>
        /// <param name="values">An object, whose property values 
        /// will be used to populate the dictionary</param>
        public ValueDictionary Merge(object values)
        {
            return Merge(values, true);
        }

        /// <summary>
        /// Merges two collections and returns a result
        /// </summary>
        /// <param name="values">An object, whose property values 
        /// will be used to populate the dictionary</param>
        /// <param name="replace">True to replace existing values</param>
        public ValueDictionary Merge(object values, bool replace)
        {
            return Merge((IValueSet)new ValueDictionary(values), replace);
        }

        /// <summary>
        /// Merges two collections and returns the result.
        /// </summary>
        /// <param name="dictionary">The collection, which keys and values 
        /// should be merged with the dictionary</param>
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
                if (ContainsKey(kvp.Key) && !replace)
                    continue;

                this[kvp.Key] = kvp.Value;
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
                if (ContainsKey(key) && !replace)
                    continue;

                this[key] = values[key];
            }
            return this;
        }        

        protected void AddValues(IValueSet values)
        {
            if (values == null)
                return;

            foreach (string key in values.Keys)
                base.Add(key, values[key]);
        }

        protected void AddValues(object values)
        {
            if (values == null)
                return;

            foreach (KeyValuePair<string, object> kvp in
                Converter.ToDictionary(values))
                base.Add(kvp.Key, kvp.Value);
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

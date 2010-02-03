using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public class TempDataDictionary : IValueSet, IDictionary<string, object>, ISerializable, 
        ICollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>
    {
        #region Static Fields
        private const string _serializationKey = "Radischevo.Wahha.Web.Mvc.TempDataDictionary.Serialization";
        #endregion

        #region Instance Fields
        private Dictionary<string, object> _data;
        #endregion

        #region Constructors
        public TempDataDictionary()
        {
        }

        protected TempDataDictionary(SerializationInfo info, StreamingContext context)
        {
            _data = info.GetValue(_serializationKey, typeof(Dictionary<string, object>)) as Dictionary<string, object>;
        }
        #endregion

        #region Instance Properties
        public int Count
        {
            get
            {
                return _data.Count;
            }
        }

        public object this[string key]
        {
            get
            {
                object obj;
                if (!_data.TryGetValue(key, out obj))
                    return null;

                return obj;
            }
            set
            {
                _data[key] = value;
            }
        }

        public Dictionary<string, object>.KeyCollection Keys
        {
            get
            {
                return _data.Keys;
            }
        }

        public Dictionary<string, object>.ValueCollection Values
        {
            get
            {
                return _data.Values;
            }
        }

        public bool IsReadOnly
        {
            get 
            {
                return ((ICollection<KeyValuePair<string, object>>)_data).IsReadOnly;
            }
        }        
        #endregion

        #region Instance Methods
        public void Load(ControllerContext context, ITempDataProvider provider)
        {
            Precondition.Require(context, Error.ArgumentNull("context"));
            Precondition.Require(provider, Error.ArgumentNull("provider"));

            IDictionary<string, object> pd = provider.Load(context);
            _data = (pd == null) ? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) : 
                new Dictionary<string, object>(pd, StringComparer.OrdinalIgnoreCase);
        }

        public void Save(ControllerContext context, ITempDataProvider provider)
        {
            Precondition.Require(context, Error.ArgumentNull("context"));
            Precondition.Require(provider, Error.ArgumentNull("provider"));

            provider.Save(context, _data);
        }

        public void Add(string key, object value)
        {
            _data.Add(key, value);
        }

        public void Clear()
        {
            _data.Clear();
        }

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

            if (!TryGetValue(key, out value))
                return defaultValue;

            if (typeof(TValue) == typeof(bool)) // специальная методика для bool 
            {
                switch (value.ToString().ToLower())
                {
                    case "on":
                    case "yes":
                        value = true;
                        break;
                }
            }
            return Converter.ChangeType<TValue>(value, defaultValue);
        }

        public bool ContainsAll(params string[] keys)
        {
            foreach (string key in keys)
                if (!ContainsKey(key))
                    return false;

            return true;
        }

        public bool ContainsAny(params string[] keys)
        {
            foreach (string key in keys)
                if (ContainsKey(key))
                    return true;

            return false;
        }

        public bool ContainsKey(string key)
        {
            return _data.ContainsKey(key);
        }

        public bool ContainsValue(object value)
        {
            return _data.ContainsValue(value);
        }

        public bool Remove(string key)
        {
            return _data.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return _data.TryGetValue(key, out value);
        }

        public Dictionary<string, object>.Enumerator GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        protected virtual void GetObjectData(SerializationInfo info,
            StreamingContext context)
        {
            info.AddValue(_serializationKey, _data);
        }
        #endregion

        #region Implemented Methods
        IEnumerable<string> IValueSet.Keys
        {
            get
            {
                return Keys;
            }
        }

        ICollection<string> IDictionary<string, object>.Keys
        {
            get
            {
                return Keys;
            }
        }

        ICollection<object> IDictionary<string, object>.Values
        {
            get
            {
                return Values;
            }
        }

        void ICollection<KeyValuePair<string, object>>.Add(
            KeyValuePair<string, object> item)
        {
            ((ICollection<KeyValuePair<string, object>>)_data).Add(item);
        }

        bool ICollection<KeyValuePair<string, object>>.Contains(
            KeyValuePair<string, object> item)
        {
            return ((ICollection<KeyValuePair<string, object>>)_data).Contains(item);
        }

        void ICollection<KeyValuePair<string, object>>.CopyTo(
            KeyValuePair<string, object>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, object>>)_data).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<string, object>>.Remove(
            KeyValuePair<string, object> item)
        {
            return ((ICollection<KeyValuePair<string, object>>)_data).Remove(item);
        }        

        IEnumerator<KeyValuePair<string, object>> 
            IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return _data.GetEnumerator();
        }       
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }
        
        void ISerializable.GetObjectData(SerializationInfo info, 
            StreamingContext context)
        {
            GetObjectData(info, context);
        }
        #endregion
    }
}

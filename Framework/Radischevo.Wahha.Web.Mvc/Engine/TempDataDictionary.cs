using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	[Serializable]
    public class TempDataDictionary : IValueSet, IDictionary<string, object>, ISerializable
	{
		#region Nested Types
		private sealed class TempDataEnumerator : IEnumerator<KeyValuePair<string, object>>
		{
			#region Instance Fields
			private IEnumerator<KeyValuePair<string, object>> _enumerator;
			private TempDataDictionary _data;
			#endregion

			#region Constructors
			public TempDataEnumerator(TempDataDictionary tempData)
			{
				_data = tempData;
				_enumerator = _data._data.GetEnumerator();
			}
			#endregion

			#region Instance Properties
			public KeyValuePair<string, object> Current
			{
				get
				{
					KeyValuePair<string, object> kvp = _enumerator.Current;
					_data._initialKeys.Remove(kvp.Key);

					return kvp;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}
			#endregion

			#region Instance Methods
			public bool MoveNext()
			{
				return _enumerator.MoveNext();
			}

			public void Reset()
			{
				_enumerator.Reset();
			}

			void IDisposable.Dispose()
			{
				_enumerator.Dispose();
			}
			#endregion
		}
		#endregion

		#region Static Fields
		private const string _serializationKey = "Radischevo.Wahha.Web.Mvc.TempDataDictionary.Serialization";
        #endregion

        #region Instance Fields
        private Dictionary<string, object> _data;
		private HashSet<string> _initialKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		private HashSet<string> _retainedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        #endregion

        #region Constructors
        public TempDataDictionary()
        {
			_data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
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

				_initialKeys.Remove(key);
                return obj;
            }
            set
            {
                _data[key] = value;
				_initialKeys.Add(key);
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return _data.Keys;
            }
        }

        public ICollection<object> Values
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

		#region Static Methods
		private static TValue ConvertValue<TValue>(object value, TValue defaultValue, 
			IFormatProvider provider)
		{
			return Converter.ChangeType<TValue>(value, defaultValue, provider);
		}
		#endregion

		#region Instance Methods
		public void Keep()
		{
			_retainedKeys.Clear();
			_retainedKeys.UnionWith(_data.Keys);
		}

		public void Keep(string key)
		{
			_retainedKeys.Add(key);
		}

        public void Load(ControllerContext context, ITempDataProvider provider)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            Precondition.Require(provider, () => Error.ArgumentNull("provider"));

            IDictionary<string, object> pd = provider.Load(context);
            _data = (pd == null) ? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) : 
                new Dictionary<string, object>(pd, StringComparer.OrdinalIgnoreCase);

			_initialKeys = new HashSet<string>(_data.Keys, StringComparer.OrdinalIgnoreCase);
			_retainedKeys.Clear();
        }

		public object Peek(string key)
		{
			object value;
			_data.TryGetValue(key, out value);

			return value;
		}

		public TValue Peek<TValue>(string key)
		{
			return Peek<TValue>(key, default(TValue), 
				CultureInfo.CurrentCulture);
		}

		public TValue Peek<TValue>(string key, TValue defaultValue)
		{
			return Peek<TValue>(key, defaultValue, CultureInfo.CurrentCulture);
		}

		public TValue Peek<TValue>(string key, TValue defaultValue, 
			IFormatProvider provider)
		{
			object value;

			if (!_data.TryGetValue(key, out value))
				return defaultValue;

			return ConvertValue(value, defaultValue, provider);
		}

        public void Save(ControllerContext context, ITempDataProvider provider)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            Precondition.Require(provider, () => Error.ArgumentNull("provider"));

			string[] keysToKeep = _initialKeys.Union(_retainedKeys, StringComparer.OrdinalIgnoreCase).ToArray();
			string[] keysToRemove = _data.Keys.Except(keysToKeep, StringComparer.OrdinalIgnoreCase).ToArray();

			foreach (string key in keysToRemove)
				_data.Remove(key);
			
			provider.Save(context, _data);
        }

        public void Add(string key, object value)
        {
            _data.Add(key, value);
			_initialKeys.Add(key);
        }

        public void Clear()
        {
            _data.Clear();
			_retainedKeys.Clear();
			_initialKeys.Clear();
        }

        /// <summary>
        /// Gets the typed value with the 
        /// specified key
        /// </summary>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="key">The key to find</param>
        /// <param name="defaultValue">The default value of the variable</param>
        public TValue GetValue<TValue>(string key, TValue defaultValue, 
			IFormatProvider provider)
        {
            object value;

            if (!TryGetValue(key, out value))
                return defaultValue;

			return ConvertValue(value, defaultValue, provider);
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
			_retainedKeys.Remove(key);
			_initialKeys.Remove(key);

            return _data.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
			_initialKeys.Remove(key);
            return _data.TryGetValue(key, out value);
        }

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
			return new TempDataEnumerator(this);
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

        void ICollection<KeyValuePair<string, object>>.Add(
            KeyValuePair<string, object> item)
        {
			_initialKeys.Add(item.Key);
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
			_initialKeys.Remove(item.Key);
            return ((ICollection<KeyValuePair<string, object>>)_data).Remove(item);
        }        

        IEnumerator<KeyValuePair<string, object>> 
            IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
			return GetEnumerator();
        }       
        
        IEnumerator IEnumerable.GetEnumerator()
        {
			return GetEnumerator();
        }
        
        void ISerializable.GetObjectData(SerializationInfo info, 
            StreamingContext context)
        {
            GetObjectData(info, context);
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Text
{
    public sealed class HtmlAttributeDictionary : IEnumerable<KeyValuePair<string, object>>
	{
		#region Nested Types
		private static class IdentifierValidator
		{
			#region Static Methods
			private static bool IsAllowableSpecialCharacter(char c)
			{
				switch (c)
				{
					case '-':
					case '_':
					case ':':
					case '.':
						return true;

					default:
						return false;
				}
			}

			private static bool IsDigit(char c)
			{
				return ('0' <= c && c <= '9');
			}

			public static bool IsLetter(char c)
			{
				return (('A' <= c && c <= 'Z') || ('a' <= c && c <= 'z'));
			}

			public static bool IsValidCharacter(char c)
			{
				return (IsLetter(c) || IsDigit(c) || 
					IsAllowableSpecialCharacter(c));
			}
			#endregion
		}
		#endregion

		#region Instance Fields
		private ValueDictionary _values;
        #endregion

        #region Constructors
        public HtmlAttributeDictionary()
        {
            _values = new ValueDictionary();
        }

        public HtmlAttributeDictionary(
            IDictionary<string, object> dictionary)
        {
            _values = new ValueDictionary(dictionary);
        }

        public HtmlAttributeDictionary(object attributes)
        {
            _values = new ValueDictionary(attributes);
        }
        #endregion

        #region Instance Properties
        public object this[string key]
        {
            get
            {
                return _values[key];
            }
            set
            {
                _values[key] = value;
            }
        }

        public int Count
        {
            get
            {
                return _values.Count;
            }
        }
        #endregion

		#region Static Methods
		private static bool ValidateIdentifier(string identifier)
		{
			if(String.IsNullOrEmpty(identifier))
				return false;

			if (!IdentifierValidator.IsLetter(identifier[0]))
				return false;

			for (int i = 1; i < identifier.Length; ++i)
			{
				if (!IdentifierValidator.IsValidCharacter(identifier[i]))
					return false;
			}
			return true;
		}
		#endregion

		#region Instance Methods
		public bool TryGetValue(string key, out object value)
        {
            return _values.TryGetValue(key, out value);
        }

        public void Add(string key, object value)
        {
            _values.Add(key, value);
        }

        public bool Remove(string key)
        {
            return _values.Remove(key);
        }

        public bool ContainsKey(string key)
        {
            return _values.ContainsKey(key);
        }

        public void Clear()
        {
            _values.Clear();
        }

        public void Merge(string key, object value)
        {
            Merge(key, value, true);
        }

        public void Merge(string key, object value, bool replaceExisting)
        {
            Precondition.Defined(key, () => Error.ArgumentNull("key"));

            if (!_values.ContainsKey(key) || replaceExisting)
                _values[key] = value;
        }

        public void Merge(object attributes)
        {
            Merge(attributes, true);
        }

        public void Merge(object attributes, bool replaceExisting)
        {
            if (attributes == null)
                return;

            ValueDictionary attrs = new ValueDictionary(attributes);
            Merge<string, object>(attrs, replaceExisting);
        }

        public void Merge<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        {
            Merge(dictionary, true);
        }

        public void Merge<TKey, TValue>(IDictionary<TKey, TValue> dictionary, bool replaceExisting)
        {
            if (dictionary == null)
                return;

            foreach (KeyValuePair<TKey, TValue> entry in dictionary)
                Merge(Convert.ToString(entry.Key, CultureInfo.InvariantCulture), 
                    entry.Value, replaceExisting);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, object> attr in _values)
            {
                string value = (attr.Value == null) ? String.Empty : 
                    HttpUtility.HtmlAttributeEncode(attr.Value.ToString());

				if (attr.Key.Equals("id", StringComparison.OrdinalIgnoreCase)
					&& !ValidateIdentifier(value))
					continue;

                sb.AppendFormat(" {0}=\"{1}\"", 
                    attr.Key.ToLowerInvariant(), value);
            }
            return sb.ToString();
        }
        #endregion

        #region IEnumerable<KeyValuePair<string,object>> Members
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _values.GetEnumerator();
        }
        #endregion

        #region IEnumerable Members
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}

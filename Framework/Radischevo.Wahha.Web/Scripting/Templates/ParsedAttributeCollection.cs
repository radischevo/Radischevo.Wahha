using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Radischevo.Wahha.Web.Scripting.Templates
{
    public class ParsedAttributeCollection
    {
        #region Instance Fields
        private Dictionary<string, string> _collection;
        #endregion

        #region Constructors
        public ParsedAttributeCollection()
        {
            _collection = new Dictionary<string, string>();
        }
        #endregion

        #region Instance Properties
        public string this[string key]
        {
            get
            {
				string value;
				if (_collection.TryGetValue(key, out value))
					return value;

                return null;
            }
            set
            {
                _collection[key] = value;
            }
        }

        public int Count
        {
            get 
            { 
                return _collection.Count; 
            }
        }
        #endregion

        #region Instance Methods
        public void Add(string key, string value)
        {
            if (value != null)
                value = HttpUtility.HtmlDecode(value.ToString());

            _collection[key] = value;
        }

		public void Clear()
		{
			_collection.Clear();
		}

		public bool Remove(string key)
		{
			return _collection.Remove(key);
		}

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            string value;
            foreach (string key in _collection.Keys)
            {
                result.Append(key);
                value = this[key] as string;
                if (value != null)
                    result.AppendFormat("=\"{0}\"", value);

                result.Append(' ');
            }

            if (result.Length > 0 && result[result.Length - 1] == ' ')
                result.Length--;

            return result.ToString();
        }
        #endregion
    }
}

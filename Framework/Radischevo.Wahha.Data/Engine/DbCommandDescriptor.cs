using System;
using System.Collections.Generic;
using System.Globalization;
using System.Data;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	[Serializable]
    public class DbCommandDescriptor
	{
		#region Constants
		private const int DEFAULT_TIMEOUT = 10;
		#endregion

		#region Instance Fields
		private string _text;
		private int _timeout;
        private CommandType _type;
        private DbParameterCollection _parameters;
        #endregion

        #region Constructors
        public DbCommandDescriptor()
            : this(String.Empty)
        {
        }

        public DbCommandDescriptor(string text)
            : this(text, CommandType.Text)
        {
        }

        public DbCommandDescriptor(string text, CommandType type)
			: this(text, type, null)
        {
        }

		public DbCommandDescriptor(string text, IValueSet parameters)
			: this(text, CommandType.Text, parameters)
		{
		}

		public DbCommandDescriptor(string text, object parameters)
			: this(text, CommandType.Text, parameters)
		{
		}

		public DbCommandDescriptor(string text, CommandType type, object parameters)
			: this(text, type, new ValueDictionary(parameters))
		{
		}

        public DbCommandDescriptor(string text, CommandType type, IValueSet parameters)
        {
			_text = text;
			_type = type;
			_timeout = DEFAULT_TIMEOUT;
			_parameters = CreateParameters(parameters);
        }
        #endregion

        #region Instance Properties
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }

        public CommandType Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        public int Timeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                Precondition.Require(value >= 0,
					() => Error.ParameterMustBeGreaterThanOrEqual("value", 0, value));

                _timeout = value;
            }
        }

        public DbParameterCollection Parameters
        {
            get
            {
                if (_parameters == null)
                    _parameters = new DbParameterCollection();

                return _parameters;
            }
        }
        #endregion

		#region Static Methods
		private static string BuildParameterName(string name)
		{
			return (name.StartsWith("@")) ? name : "@" + name;
		}

		private static DbParameterCollection CreateParameters(IValueSet values)
		{
			if (values == null)
				return null;

			DbParameterCollection collection = new DbParameterCollection();
			foreach (string key in values.Keys)
			{
				collection.Add(new DbParameterDescriptor(
					BuildParameterName(key), values[key]));
			}
			return collection;
		}
		#endregion
    }
}

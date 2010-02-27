using System;
using System.Collections.Generic;
using System.Globalization;
using System.Data;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
    public class DbCommandDescriptor
    {
        #region Instance Fields
        private string _text;
        private CommandType _type;
        private int _timeout;
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

        public DbCommandDescriptor(string text,
            CommandType type)
        {
            _text = text;
            _type = type;
            _timeout = 10;
            _parameters = new DbParameterCollection();
        }

        public DbCommandDescriptor(string text, 
            CommandType type, IValueSet parameters)
            : this(text, type)
        {
            AppendParameters(parameters);
        }

        public DbCommandDescriptor(string text, 
            CommandType type, object parameters)
            : this(text, type)
        {
            AppendParameters(new ValueDictionary(parameters));
        }

        public DbCommandDescriptor(string text,
            CommandType type, object[] parameters)
            : this(text, type)
        {
            AppendParameters(parameters);
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
                Precondition.Require(_timeout > 0,
					() => Error.ParameterMustBeGreaterThan("value", 0, value));

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

        #region Instance Methods
        private void AppendParameters(IValueSet values)
        {
            if (values == null)
                return;

            foreach (string key in values.Keys)
            {
                string parameterName = (key.StartsWith("@")) ? key : "@" + key;
                _parameters.Add(new DbParameterDescriptor(
                    parameterName, values[key]));
            }
        }

        private void AppendParameters(object[] parameters)
        {
            if (parameters == null || parameters.Length < 1)
                return;

            string[] paramPlaceHolders = new string[parameters.Length];
            string parameterName;

            for (int i = 0; i < paramPlaceHolders.Length; i++)
            {
                parameterName = "@p" + i.ToString(CultureInfo.InvariantCulture);
                paramPlaceHolders[i] = parameterName;

                _parameters.Add(new DbParameterDescriptor(parameterName, parameters[i]));
            }
            _text = String.Format(CultureInfo.InvariantCulture, Text, paramPlaceHolders);
        }
        #endregion
    }
}

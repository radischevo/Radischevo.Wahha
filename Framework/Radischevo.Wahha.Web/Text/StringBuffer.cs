using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Text
{
    public sealed class StringBuffer
    {
        #region Constants
        public const char EOF = '\uffff';
        #endregion

        #region Instance Fields
        private int _position;
        private string _value;
        #endregion

        #region Constructors
        public StringBuffer(string value)
        {
			Precondition.Require(value, () => Error.ArgumentNull("value"));

            _position = -1;
            _value = value;
        }
        #endregion

        #region Instance Properties
        public int Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (value < 0 || value >= _value.Length)
                    throw new ArgumentOutOfRangeException("position");

                _position = value;
            }
        }

        public char this[int index]
        {
            get
            {
                return _value[index];
            }
        }

        public char Current
        {
            get
            {
                if (_position > -1 && _position < _value.Length)
                    return _value[_position];

                return EOF;
            }
        }
        #endregion

        #region Instance Methods
        public bool Read()
        {
            if (_value.Length > ++_position)
                return true;

            _position--;
            return false;
        }

        public bool Read(char ch)
        {
            if (_value.Length > ++_position)
            {
                if (Current == ch)
                    return true;

                _position--;
            }
            return false;
        }

        public bool Match(string pattern)
        {
            return Match(pattern, StringComparison.CurrentCulture);
        }

        public bool Match(string pattern, StringComparison comparison)
        {
            return (_value.IndexOf(pattern, _position, comparison) == _position);
        }

        public char Offset(int offset)
        {
            int pos = _position + offset;
            if (pos > -1 && pos < _value.Length)
                return _value[pos];

            return EOF;
        }

        public string Offset(int offset, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count");

            int pos = _position + offset;
            if (pos > -1 && pos + count <= _value.Length)
                return _value.Substring(pos, count);

            return null;
        }

        public override string ToString()
        {
            if (_value.Length <= _position)
                return String.Empty;

            return _value.Substring(_position);
        }
        #endregion
    }
}

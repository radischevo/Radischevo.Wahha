using System;
using System.Globalization;
using System.Text;

namespace Radischevo.Wahha.Data.Serialization
{
    internal class JavaScriptString
    {
        #region Instance Fields
        private int _index;
        private string _value;
        #endregion

        #region Constructors
        internal JavaScriptString(string value)
        {
            _value = value;
        }
        #endregion

        #region Static Methods
        private static void AppendCharAsUnicode(StringBuilder builder, char c)
        {
            builder.Append("\\u");
            builder.AppendFormat(CultureInfo.InvariantCulture, "{0:x4}", (int)c);
        }

        internal static string QuoteString(string value, bool addQuotes)
        {
            string str = QuoteString(value);
            if (addQuotes)
                return String.Concat("\"", str, "\"");

            return str;
        }

        internal static string QuoteString(string value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            int index = 0;
            char c;
            StringBuilder sb = new StringBuilder(value.Length + 5);

            while (index < value.Length)
            {
                c = value[index++];
                switch (c)
                {
                    case '\u0008':
                        sb.Append("\\b");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\u000C':
                        sb.Append("\\f");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '>':
                    case '<':
                    case '=':
                    case '\u000B':
                        AppendCharAsUnicode(sb, c);
                        break;
                    default:
                        if (c < '\u0020')
                            AppendCharAsUnicode(sb, c);
                        else
                            sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }
        #endregion

        #region Instance Methods
        internal string GetDebugString(string message)
        {
            return String.Concat(message, " (", _index, "): ", _value);
        }

        internal char? GetNextNonEmptyChar()
        {
            char c;
            while (_value.Length > _index)
            {
                c = _value[_index++];
                if (!Char.IsWhiteSpace(c))
                    return c;
            }
            return null;
        }

        internal string MoveNext(int count)
        {
            if (_value.Length >= (_index + count))
            {
                string str = _value.Substring(_index, count);
                _index += count;

                return str;
            }
            return null;
        }

        internal char? MoveNext()
        {
            if (_value.Length > _index)
                return _value[_index++];

            return null;
        }

        internal void MovePrev(int count)
        {
            while (_index > 0 && count > 0)
            {
                _index--;
                count--;
            }
        }

        internal void MovePrev()
        {
            if (_index > 0)
                _index--;
        }

        public override string ToString()
        {
            if (_value.Length <= _index)
                return String.Empty;

            return _value.Substring(_index);
        }
        #endregion
    }
}

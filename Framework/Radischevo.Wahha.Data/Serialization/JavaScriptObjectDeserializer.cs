using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Radischevo.Wahha.Data.Serialization
{
    internal class JavaScriptObjectDeserializer
    {
        #region Instance Fields
        private int _depthLimit;
        private JavaScriptSerializer _serializer;
        internal JavaScriptString _serializedValue;
        #endregion

        #region Constructors
        private JavaScriptObjectDeserializer(string input, 
            int depthLimit, JavaScriptSerializer serializer)
        {
            _serializedValue = new JavaScriptString(input);
            _depthLimit = depthLimit;
            _serializer = serializer;
        }
        #endregion

        #region Static Methods
        internal static object BasicDeserialize(string input, int depthLimit, JavaScriptSerializer serializer)
        {
            JavaScriptObjectDeserializer deserializer =
                new JavaScriptObjectDeserializer(input, depthLimit, serializer);

            object result = deserializer.DeserializeInternal(0);
            char? nextNonEmptyChar = deserializer._serializedValue.GetNextNonEmptyChar();
            int? charIndex = (nextNonEmptyChar.HasValue) ? new int?(nextNonEmptyChar.GetValueOrDefault()) : null;

            if (charIndex.HasValue)
                throw Error.IllegalJavaScriptPrimitive();

            return result;
        }
        
        private static char CheckQuoteChar(char? c)
        {
            if (c == '\'')
                return c.Value;

            if (c != '"')
                throw Error.StringIsNotQuoted();

            return '"';
        }

        private static bool IsNextElementArray(char? c)
        {
            return (c == '[');
        }

        private static bool IsNextElementObject(char? c)
        {
            return (c == '{');
        }

        private static bool IsNextElementString(char? c)
        {
            return (c == '"' || c == '\'');
        }
        #endregion

        #region Instance Methods
        private string DeserializeMemberName()
        {
            char? nonEmptyChar = _serializedValue.GetNextNonEmptyChar();
            if (!nonEmptyChar.HasValue)
                return null;
            
            _serializedValue.MovePrev();
            if (IsNextElementString(nonEmptyChar))
                return DeserializeString();
            
            return DeserializePrimitiveToken();
        }

        private void AppendEscapeSequence(char? c, StringBuilder sb)
        {
            if (c == '"' || c == '\'' || c == '/')
                sb.Append(c);
            else if (c == 'b')
                sb.Append('\b');
            else if (c == 'f')
                sb.Append('\f');
            else if (c == 'n')
                sb.Append('\n');
            else if (c == 'r')
                sb.Append('\r');
            else if (c == 't')
                sb.Append('\t');
            else
            {
                if (c != 'u')
                    throw Error.InvalidEscapeSequence();

                sb.Append((char)int.Parse(_serializedValue.MoveNext(4), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
            }
        }

        private bool IsNextElementDateTime()
        {
            int jsonPrefixLength = JavaScriptSerializer.JsonDateTimePrefix.Length;
            int jsPrefixLength = JavaScriptSerializer.JsDateTimePrefix.Length;

            string value = _serializedValue.MoveNext(jsonPrefixLength);
            if (value != null)
            {
                _serializedValue.MovePrev(jsonPrefixLength);
                if (String.Equals(value, JavaScriptSerializer.JsonDateTimePrefix, 
                    StringComparison.Ordinal))
                {
                    return true;
                }
                else
                {
                    value = _serializedValue.MoveNext(jsPrefixLength);
                    if (value != null)
                    {
                        _serializedValue.MovePrev(9);
                        return String.Equals(value, JavaScriptSerializer.JsDateTimePrefix,
                           StringComparison.Ordinal);
                    }
                }
            }
            return false;
        }

        private string DeserializeString()
        {
            StringBuilder sb = new StringBuilder();

            bool hasBackSlashes = false;
            char quoteChar = CheckQuoteChar(_serializedValue.MoveNext());

            while (true)
            {
                char? nextChar = _serializedValue.MoveNext();
                if (!nextChar.HasValue)
                    throw Error.UnterminatedStringConstant();
                
                if (nextChar == '\\')
                {
                    if (hasBackSlashes)
                    {
                        sb.Append('\\');
                        hasBackSlashes = false;
                    }
                    else
                        hasBackSlashes = true;
                }
                else if (hasBackSlashes)
                {
                    AppendEscapeSequence(nextChar, sb);
                    hasBackSlashes = false;
                }
                else
                {
                    if (nextChar.HasValue && nextChar == quoteChar)
                        return sb.ToString();
                    
                    sb.Append(nextChar);
                }
            }
        }

        private object DeserializeStringIntoDateTime()
        {
            long ticks;
            Match match = Regex.Match(_serializedValue.ToString(), 
                JavaScriptSerializer.JsonDateTimePattern);
            Match jsMatch = Regex.Match(_serializedValue.ToString(),
                JavaScriptSerializer.JsDateTimePattern);

            if (long.TryParse(match.Groups["ticks"].Value, out ticks))
            {
                _serializedValue.MoveNext(match.Length);
                return new DateTime((ticks * 10000L) + 
                    JavaScriptSerializer.DateTimeMinTimeTicks, DateTimeKind.Utc);
            }
            else if (long.TryParse(jsMatch.Groups["ticks"].Value, out ticks))
            {
                _serializedValue.MoveNext(jsMatch.Length);
                return new DateTime((ticks * 10000L) +
                    JavaScriptSerializer.DateTimeMinTimeTicks, DateTimeKind.Utc);
            }
            return DeserializeString();
        }

        private string DeserializePrimitiveToken()
        {
            StringBuilder builder = new StringBuilder();

            while (true)
            {
                char? nextChar = _serializedValue.MoveNext();
                if (nextChar.HasValue)
                {
                    if (Char.IsLetterOrDigit(nextChar.Value) || nextChar.Value == '.' ||
                        nextChar.Value == '-' || nextChar.Value == '_' || nextChar.Value == '+')
                    {
                        builder.Append(nextChar);
                    }
                    else
                    {
                        _serializedValue.MovePrev();
                        return builder.ToString();
                    }
                }
                else
                {
                    return builder.ToString();
                }
            }
        }

        private object DeserializePrimitiveObject()
        {
            string token = DeserializePrimitiveToken();

            if (token.Equals("null"))
                return null;
           
            if (token.Equals("true"))
                return true;
            
            if (token.Equals("false"))
                return false;
            
            bool hasDots = token.IndexOf('.') >= 0;
            if (token.LastIndexOf("e", StringComparison.OrdinalIgnoreCase) < 0)
            {
                decimal valueAsDecimal;
                int valueAsInt;
                long valueAsLong;

                if (!hasDots)
                {
                    if (Int32.TryParse(token, NumberStyles.Integer, 
                        CultureInfo.InvariantCulture, out valueAsInt))
                        return valueAsInt;

                    if (Int64.TryParse(token, NumberStyles.Integer, 
                        CultureInfo.InvariantCulture, out valueAsLong))
                        return valueAsLong;
                }

                if (Decimal.TryParse(token, NumberStyles.Number, CultureInfo.InvariantCulture, out valueAsDecimal))
                    return valueAsDecimal;
            }

            double valueAsDouble;
            if (!double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out valueAsDouble))
                throw Error.IllegalJavaScriptPrimitive();
            
            return valueAsDouble;
        }

        private IList DeserializeList(int depth)
        {
            char? nextChar;
            bool hasMoreElements = false;
            IList list = new ArrayList();

            if (_serializedValue.MoveNext() != '[')
                throw Error.IllegalArrayStartingSymbol();

            while (true)
            {
                nextChar = _serializedValue.GetNextNonEmptyChar();
                if (nextChar.HasValue && nextChar != ']')
                {
                    _serializedValue.MovePrev();

                    object obj = DeserializeInternal(depth);
                    list.Add(obj);

                    hasMoreElements = false;

                    nextChar = _serializedValue.GetNextNonEmptyChar();
                    if (nextChar != ']')
                    {
                        hasMoreElements = true;
                        if (nextChar != ',')
                            throw Error.CommaWasExpectedInArrayDeclaration();
                    }
                    else
                        break;
                }
                else
                    break;
            }
            if (hasMoreElements)
                throw Error.ExtraCommaAtArrayEndingFound();

            if (nextChar != ']')
                throw Error.InvalidArrayEndingSymbol();
            
            return list;
        }

        private IDictionary<string, object> DeserializeDictionary(int depth)
        {
            IDictionary<string, object> dictionary = null;
            char? nextChar;

            if (_serializedValue.MoveNext() != '{')
                throw Error.OpeningBraceExpected();

            while ((nextChar = _serializedValue.GetNextNonEmptyChar()).HasValue)
            {
                _serializedValue.MovePrev();
                if (nextChar == ':')
                    throw Error.InvalidMemberName();

                string memberName = null;
                if (nextChar != '}')
                {
                    memberName = DeserializeMemberName();
                    if (String.IsNullOrEmpty(memberName))
                        throw Error.InvalidMemberName();

                    if (_serializedValue.GetNextNonEmptyChar() != ':')
                        throw Error.InvalidObjectDefinition();
                }

                if (dictionary == null)
                {
                    dictionary = new Dictionary<string, object>();
                    if (String.IsNullOrEmpty(memberName))
                    {
                        nextChar = _serializedValue.GetNextNonEmptyChar();
                        break;
                    }
                }
                object value = DeserializeInternal(depth);
                dictionary[memberName] = value;

                nextChar = _serializedValue.GetNextNonEmptyChar();
                if (nextChar != '}')
                {
                    if (nextChar != ',')
                        throw Error.InvalidObjectDefinition();
                }
                else
                    break;
            }

            if (nextChar != '}')
                throw Error.InvalidObjectDefinition();
            
            return dictionary;
        }

        private object DeserializeInternal(int depth)
        {
            if (++depth > _depthLimit)
                throw Error.DepthLimitExceeded();
            
            char? nextChar = _serializedValue.GetNextNonEmptyChar();
            if (!nextChar.HasValue)
                return null;

            JavaScriptTypeResolver resolver = _serializer.TypeResolver;

            _serializedValue.MovePrev();
            if (IsNextElementDateTime())
                return DeserializeStringIntoDateTime();
            
            if (IsNextElementObject(nextChar))
            {
                IDictionary<string, object> dict = DeserializeDictionary(depth);
                if (dict.ContainsKey(JavaScriptSerializer.ServerTypeFieldName))
                {
                    return ObjectConverter.ConvertObject(dict, 
                        (resolver == null) ? null : resolver.ResolveType(
                            dict[JavaScriptSerializer.ServerTypeFieldName].ToString()), 
                            _serializer);
                }
                return dict;
            }

            if (IsNextElementArray(nextChar))
                return DeserializeList(depth);
            
            if (IsNextElementString(nextChar))
                return DeserializeString();
            
            return DeserializePrimitiveObject();
        }
        #endregion
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Scripting.Serialization
{
    public enum SerializationFormat : int
    {
        Json = 0,
        JavaScript
    }

    public class JavaScriptSerializer
    {
        #region Nested Types
        private class ReferenceComparer : IEqualityComparer
        {
            #region Constructors
            public ReferenceComparer()
            {   }
            #endregion

            #region Instance Methods
            bool IEqualityComparer.Equals(object x, object y)
            {
                return Object.ReferenceEquals(x, y);
            }

            int IEqualityComparer.GetHashCode(object obj)
            {
                return obj.GetHashCode();
            }
            #endregion
        }
        #endregion

        #region Static Fields
        internal static readonly long DateTimeMinTimeTicks;
        internal const int DefaultMaxJsonLength = 2097152;
        internal const int DefaultRecursionLimit = 100;
        internal const string JsonDateTimePrefix = "\"\\/Date(";
        internal const string JsonDateTimePattern = 
            "^\"\\\\/Date\\((?<ticks>-?[0-9]+)(?:[a-zA-Z]|(?:\\+|-)[0-9]{4})?\\)\\\\/\"";
        internal const string JsDateTimePrefix = "new Date(";
        internal const string JsDateTimePattern = 
            "^new\\s*Date\\((?<ticks>-?[0-9]+)(?:[a-zA-Z]|(?:\\+|-)[0-9]{4})?\\)";
        internal const string ServerTypeFieldName = "__type";
        #endregion

        #region Instance Fields
        private Dictionary<Type, JavaScriptConverter> _converters;
        private JavaScriptTypeResolver _typeResolver;
        private int _maxJsonLength;
        private int _recursionLimit;
        #endregion

        #region Constructors
        static JavaScriptSerializer()
        {
            DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTimeMinTimeTicks = time.Ticks;
        }

        public JavaScriptSerializer()
            : this(null)
        {   }

        public JavaScriptSerializer(JavaScriptTypeResolver resolver)
        {
            _typeResolver = resolver;
            _recursionLimit = DefaultRecursionLimit;
            _maxJsonLength = DefaultMaxJsonLength;
        }
        #endregion

        #region Instance Properties
        public int RecursionLimit
        {
            get
            {
                return _recursionLimit;
            }
            set
            {
                if (value < 1)
                    throw Error.ParameterMustBeGreaterThan("RecursionLimit", 0, value);
                _recursionLimit = value;
            }
        }

        public int MaxJsonLength
        {
            get
            {
                return _maxJsonLength;
            }
            set
            {
                if (value < 1)
                    throw Error.ParameterMustBeGreaterThan("MaxJsonLength", 0, value);
                _maxJsonLength = value;
            }
        }

        public JavaScriptTypeResolver TypeResolver
        {
            get
            {
                return _typeResolver;
            }
            set
            {
                _typeResolver = value;
            }
        }

        private Dictionary<Type, JavaScriptConverter> Converters
        {
            get
            {
                if (_converters == null)
                    _converters = new Dictionary<Type, JavaScriptConverter>();

                return _converters;
            }
        }
        #endregion

        #region Static Methods
        private static object Deserialize(JavaScriptSerializer serializer, 
            string input, Type type, int depthLimit)
        {
            Precondition.Require(input, Error.ArgumentNull("input"));
            if (input.Length > serializer.MaxJsonLength)
                throw Error.MaximumJsonStringLengthExceeded();
            
            return ObjectConverter.ConvertObject(
                JavaScriptObjectDeserializer.BasicDeserialize(input, depthLimit, serializer), 
                type, serializer);
        }
        #endregion

        #region Private Serialization Methods
        private static void SerializeBoolean(bool b, StringBuilder sb)
        {
            sb.Append((b) ? "true" : "false");
        }

        private static void SerializeDateTime(DateTime datetime,
            StringBuilder sb, SerializationFormat serializationFormat)
        {
            if (serializationFormat == SerializationFormat.Json)
            {
                sb.Append(JavaScriptSerializer.JsonDateTimePrefix)
                    .Append((long)(datetime.ToUniversalTime().Ticks - DateTimeMinTimeTicks) / 10000L)
                    .Append(")\\/\"");
            }
            else
            {
                sb.Append(JavaScriptSerializer.JsDateTimePrefix)
                    .Append((long)((datetime.ToUniversalTime().Ticks - DateTimeMinTimeTicks) / 10000L))
                    .Append(")");
            }
        }

        private static void SerializeString(string input, StringBuilder sb)
        {
            sb.Append('"')
                .Append(JavaScriptString.QuoteString(input))
                .Append('"');
        }

        private static void SerializeUri(Uri uri, StringBuilder sb)
        {
            sb.Append("\"").Append(uri.GetComponents(UriComponents.SerializationInfoString,
                UriFormat.UriEscaped)).Append("\"");
        }

        private static void SerializeGuid(Guid guid, StringBuilder sb)
        {
            sb.Append("\"").Append(guid.ToString()).Append("\"");
        }

        private void SerializeEnumerable(IEnumerable enumerable, StringBuilder sb,
            int depth, Hashtable objectsInUse, SerializationFormat serializationFormat)
        {
            sb.Append('[');
            bool isFirst = true;
            foreach (object obj in enumerable)
            {
                if (!isFirst)
                    sb.Append(',');

                SerializeValue(obj, sb, depth, objectsInUse, serializationFormat);
                isFirst = false;
            }
            sb.Append(']');
        }
        
        private void SerializeDictionary(IDictionary obj, StringBuilder sb,
            int depth, Hashtable objectsInUse, SerializationFormat serializationFormat)
        {
            sb.Append('{');
            bool isFirst = true;
            bool hasTypeKey = false;

            if (obj.Contains(ServerTypeFieldName))
            {
                isFirst = false;
                hasTypeKey = true;

                SerializeDictionaryKeyValue(ServerTypeFieldName, 
                    obj[ServerTypeFieldName], sb, depth, 
                    objectsInUse, serializationFormat);
            }

            foreach (DictionaryEntry entry in obj)
            {
                string key = (entry.Key as string);
                if (key == null)
                    throw Error.SuppliedDictionaryTypeIsNotSupported();

                if (hasTypeKey && string.Equals(key, ServerTypeFieldName, StringComparison.Ordinal))
                {
                    hasTypeKey = false;
                }
                else
                {
                    if (!isFirst)
                        sb.Append(',');

                    SerializeDictionaryKeyValue(key, entry.Value, sb, depth, objectsInUse, serializationFormat);
                    isFirst = false;
                }
            }
            sb.Append('}');
        }

        private void SerializeDictionaryKeyValue(string key, object value, StringBuilder sb,
            int depth, Hashtable objectsInUse, SerializationFormat serializationFormat)
        {
            SerializeString(key, sb);
            sb.Append(':');
            SerializeValue(value, sb, depth, objectsInUse, serializationFormat);
        }

        private void SerializeCustomObject(object obj, StringBuilder sb,
            int depth, Hashtable objectsInUse, SerializationFormat serializationFormat)
        {
            bool isFirst = true;
            Type type = obj.GetType();
            sb.Append('{');

            if (_typeResolver != null)
            {
                string typeId = _typeResolver.ResolveTypeId(type);
                if (typeId != null)
                {
                    SerializeString(ServerTypeFieldName, sb);
                    sb.Append(':');
                    SerializeValue(typeId, sb, depth, objectsInUse, serializationFormat);
                    isFirst = false;
                }
            }

            foreach (FieldInfo fi in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!fi.IsDefined(typeof(ScriptIgnoreAttribute), true))
                {
                    if (!isFirst)
                        sb.Append(',');

                    SerializeString(fi.Name, sb);
                    sb.Append(':');
                    SerializeValue(fi.GetValue(obj), sb, depth, objectsInUse, serializationFormat);

                    isFirst = false;
                }
            }
            foreach (PropertyInfo pi in type.GetProperties(BindingFlags.GetProperty |
                BindingFlags.Public | BindingFlags.Instance))
            {
                if (!pi.IsDefined(typeof(ScriptIgnoreAttribute), true))
                {
                    MethodInfo getMethod = pi.GetGetMethod();
                    if (getMethod != null && getMethod.GetParameters().Length <= 0)
                    {
                        if (!isFirst)
                            sb.Append(',');

                        SerializeString(pi.Name, sb);
                        sb.Append(':');
                        SerializeValue(getMethod.Invoke(obj, null), sb, depth, objectsInUse, serializationFormat);

                        isFirst = false;
                    }
                }
            }
            sb.Append('}');
        }

        private void SerializeValue(object obj, StringBuilder sb,
            int depth, Hashtable objectsInUse, SerializationFormat serializationFormat)
        {
            if (++depth > _recursionLimit)
                throw Error.MaximumRecursionDepthExceeded();

            JavaScriptConverter converter = null;
            if (obj != null && TryGetConverter(obj.GetType(), out converter))
            {
                IDictionary<string, object> dictionary = converter.Serialize(obj, this);
                if (_typeResolver != null)
                {
                    string typeId = _typeResolver.ResolveTypeId(obj.GetType());
                    if (typeId != null)
                        dictionary[ServerTypeFieldName] = typeId;
                }
                sb.Append(Serialize(dictionary, serializationFormat));
            }
            else
                SerializeValueInternal(obj, sb, depth, objectsInUse, serializationFormat);
        }

        private void SerializeValueInternal(object obj, StringBuilder sb, int depth,
            Hashtable objectsInUse, SerializationFormat serializationFormat)
        {
            if (obj == null || DBNull.Value.Equals(obj))
            {
                sb.Append("null");
            }
            else
            {
                string input = (obj as string);
                if (input != null)
                {
                    SerializeString(input, sb);
                }
                else if (obj is char)
                {
                    if (((char)obj) == '\0')
                        sb.Append("null");
                    else
                        SerializeString(obj.ToString(), sb);
                }
                else if (obj is bool)
                {
                    SerializeBoolean((bool)obj, sb);
                }
                else if (obj is DateTime)
                {
                    SerializeDateTime((DateTime)obj, sb, serializationFormat);
                }
                else if (obj is Guid)
                {
                    SerializeGuid((Guid)obj, sb);
                }
                else
                {
                    Uri uri = (obj as Uri);
                    if (uri != null)
                    {
                        SerializeUri(uri, sb);
                    }
                    else if (obj is double)
                    {
                        sb.Append(((double)obj).ToString("r", CultureInfo.InvariantCulture));
                    }
                    else if (obj is float)
                    {
                        sb.Append(((float)obj).ToString("r", CultureInfo.InvariantCulture));
                    }
                    else if (obj.GetType().IsPrimitive || (obj is decimal))
                    {
                        IConvertible convertible = (obj as IConvertible);
                        if (convertible != null)
                            sb.Append(convertible.ToString(CultureInfo.InvariantCulture));
                        else
                            sb.Append(obj.ToString());
                    }
                    else
                    {
                        Type type = obj.GetType();
                        if (type.IsEnum)
                        {
                            Type underlyingType = Enum.GetUnderlyingType(type);
                            if (underlyingType == typeof(long) || underlyingType == typeof(ulong))
                                throw Error.InvalidEnumType(type);

                            sb.Append(((Enum)obj).ToString("D"));
                        }
                        else
                        {
                            try
                            {
                                if (objectsInUse == null)
                                    objectsInUse = new Hashtable(new ReferenceComparer());
                                else if (objectsInUse.ContainsKey(obj))
                                    throw Error.CircularReferenceFoundAtType(type);

                                objectsInUse.Add(obj, null);
                                IDictionary dictionary = (obj as IDictionary);
                                if (dictionary != null)
                                {
                                    SerializeDictionary(dictionary, sb, depth, objectsInUse, serializationFormat);
                                }
                                else
                                {
                                    IEnumerable enumerable = (obj as IEnumerable);
                                    if (enumerable != null)
                                        this.SerializeEnumerable(enumerable, sb, depth, objectsInUse, serializationFormat);
                                    else
                                        this.SerializeCustomObject(obj, sb, depth, objectsInUse, serializationFormat);
                                }
                            }
                            finally
                            {
                                if (objectsInUse != null)
                                    objectsInUse.Remove(obj);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Instance Methods
        private JavaScriptConverter GetConverter(Type type)
        {
            while (type != null)
            {
                if (Converters.ContainsKey(type))
                    return _converters[type];
                
                type = type.BaseType;
            }
            return null;
        }

        public bool TryGetConverter(Type type, out JavaScriptConverter converter)
        {
            converter = GetConverter(type);
            return (converter != null);
        }

        public void RegisterConverters(IEnumerable<JavaScriptConverter> converters)
        {
            Precondition.Require(converters, Error.ArgumentNull("converters"));
            
            foreach (JavaScriptConverter converter in converters)
            {
                IEnumerable<Type> supportedTypes = converter.SupportedTypes;
                if (supportedTypes != null)
                {
                    foreach (Type type in supportedTypes)
                        Converters[type] = converter;
                }
            }
        }

        public string Serialize(object obj)
        {
            return Serialize(obj, SerializationFormat.Json);
        }

        public void Serialize(object obj, StringBuilder output)
        {
            this.Serialize(obj, output, SerializationFormat.Json);
        }

        public string Serialize(object obj, SerializationFormat format)
        {
            StringBuilder output = new StringBuilder();
            Serialize(obj, output, format);

            return output.ToString();
        }

        public void Serialize(object obj, StringBuilder output, SerializationFormat format)
        {
            SerializeValue(obj, output, 0, null, format);
            if (format == SerializationFormat.Json && output.Length > _maxJsonLength)
                throw Error.MaximumJsonStringLengthExceeded();
        }

        public object DeserializeObject(string input)
        {
            return Deserialize(this, input, null, _recursionLimit);
        }

        public T Deserialize<T>(string input)
        {
            return (T)Deserialize(this, input, typeof(T), _recursionLimit);
        }

        public T ConvertToType<T>(object obj)
        {
            return (T)ObjectConverter.ConvertObject(obj, typeof(T), this);
        }
        #endregion
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Radischevo.Wahha.Data.Serialization
{
    internal static class ObjectConverter
    {
        #region Static Fields
        private static readonly Type[] _emptyTypeArray = Type.EmptyTypes;
		private static Type _enumerableGenericType = typeof(IEnumerable<>);
		private static Type _idictionaryGenericType = typeof(IDictionary<,>);
        private static Type _dictionaryGenericType = typeof(Dictionary<,>);
        private static Type _listGenericType = typeof(List<>);
        #endregion

        #region Static Methods
        private static bool IsNonNullableValueType(Type type)
        {
            if (type == null)
                return false;
            
            if (!type.IsValueType)
                return false;
            
            if (!type.IsGenericType)
                return true;
            
            return (type.GetGenericTypeDefinition() != typeof(Nullable<>));
        }

        private static bool IsArrayListCompatible(Type type)
        {
            if (type.IsArray)
                return true;
            
            if (type == typeof(ArrayList))
                return true;
            
            if (type == typeof(IEnumerable))
                return true;
            
            if (type == typeof(IList))
                return true;
            
            return (type == typeof(ICollection));
        }

        private static bool IsGenericDictionary(Type type)
        {
            if (type == null)
                return false;
            
            if (!type.IsGenericType)
                return false;
            
            if (typeof(IDictionary).IsAssignableFrom(type))
                return (type.GetGenericArguments().Length == 2);
            
            if (type.GetGenericTypeDefinition() != _idictionaryGenericType)
                return false;
            
            return (type.GetGenericArguments().Length == 2);
        }

        internal static bool IsClientInstantiatableType(Type type, 
            JavaScriptSerializer serializer)
        {
            if (type == null)
                return false;
            
            if (type.IsAbstract)
                return false;
            
            if (type.IsInterface)
                return false;
            
            if (type.IsArray)
                return false;
            
            if (type == typeof(object))
                return false;
            
            JavaScriptConverter converter = null;
            if (serializer.TryGetConverter(type, out converter))
                return true;
            
            if (type.IsValueType)
                return true;

            if (type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, 
                null, _emptyTypeArray, null) != null)
                return true;

            return false;
        }

        private static bool TryConvertObjectToType(object obj, Type type, 
            JavaScriptSerializer serializer, bool throwOnError, out object convertedObject)
        {
            if (obj == null)
            {
                if (type == typeof(char))
                {
                    convertedObject = '\0';
                    return true;
                }
                else
                {
                    if (IsNonNullableValueType(type))
                    {
                        if (throwOnError)
                        {
                            throw Error.ArgumentNull("obj");
                        }
                        else
                        {
                            convertedObject = null;
                            return false;
                        }
                    }
                    convertedObject = null;
                    return true;
                }
            }
            if (obj.GetType() != type)
                return ObjectConverter.TryConvertObjectToTypeInternal(obj, type, 
                    serializer, throwOnError, out convertedObject);

            convertedObject = obj;
            return true;
        }

        private static bool TryConvertObjectToTypeInternal(object obj, Type type, 
            JavaScriptSerializer serializer, bool throwOnError, out object convertedObject)
        {
			Type instanceType = obj.GetType();
            if (type == null || type.IsAssignableFrom(instanceType))
            {
                convertedObject = obj;
                return true;
            }

            IDictionary<string, object> dictionary = (obj as IDictionary<string, object>);
            if (dictionary != null)
                return ConvertDictionary(dictionary, type, serializer, 
                    throwOnError, out convertedObject);
            
			IList list = (obj as IList);
            if (list != null)
            {
                if (ConvertList(list, type, serializer, throwOnError, out list))
                {
                    convertedObject = list;
                    return true;
                }
                convertedObject = null;
                return false;
            }

            TypeConverter converter = TypeDescriptor.GetConverter(type);
            if (converter.CanConvertFrom(instanceType))
            {
                try
                {
                    convertedObject = converter.ConvertFrom(null, 
                        CultureInfo.InvariantCulture, obj);
                    return true;
                }
                catch
                {
                    if (throwOnError)
                        throw;
                    
                    convertedObject = null;
                    return false;
                }
            }

            if (converter.CanConvertFrom(typeof(string)))
            {
                TypeConverter stringConverter = TypeDescriptor.GetConverter(obj);
                try
                {
                    string text = stringConverter.ConvertToInvariantString(obj);
                    convertedObject = converter.ConvertFromInvariantString(text);

                    return true;
                }
                catch
                {
                    if (throwOnError)
                        throw;
                    
                    convertedObject = null;
                    return false;
                }
            }

            if (type.IsAssignableFrom(instanceType))
            {
                convertedObject = obj;
                return true;
            }

            if (throwOnError)
                throw Error.CouldNotConvertObjectToType(type);
            
            convertedObject = null;
            return false;
        }

        private static bool AssignToPropertyOrField(object propertyValue, object obj,
            string memberName, JavaScriptSerializer serializer, bool throwOnError)
        {
            IDictionary dictionary = (obj as IDictionary);
            if (dictionary != null)
            {
                if (!TryConvertObjectToType(propertyValue, null, serializer, throwOnError, out propertyValue))
                    return false;

                dictionary[memberName] = propertyValue;
                return true;
            }

            Type type = obj.GetType();
            PropertyInfo property = type.GetProperty(memberName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (property != null)
            {
                MethodInfo setMethod = property.GetSetMethod();
                if (setMethod != null)
                {
                    if (!TryConvertObjectToType(propertyValue, property.PropertyType,
                        serializer, throwOnError, out propertyValue))
                        return false;

                    try
                    {
                        setMethod.Invoke(obj, new object[] { propertyValue });
                        return true;
                    }
                    catch
                    {
                        if (throwOnError)
                            throw;

                        return false;
                    }
                }
            }

            FieldInfo field = type.GetField(memberName, BindingFlags.Public |
                BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (field != null)
            {
                if (!TryConvertObjectToType(propertyValue, field.FieldType,
                    serializer, throwOnError, out propertyValue))
                    return false;

                try
                {
                    field.SetValue(obj, propertyValue);
                    return true;
                }
                catch
                {
                    if (throwOnError)
                        throw;

                    return false;
                }
            }
            return true;
        }

        private static bool AddItemToList(IList oldList, IList newList, 
            Type elementType, JavaScriptSerializer serializer, bool throwOnError)
        {
            foreach (object obj in oldList)
            {
                object result;
                if (!TryConvertObjectToType(obj, elementType, serializer, throwOnError, out result))
                    return false;
                
                newList.Add(result);
            }
            return true;
        }

        private static bool ConvertDictionary(IDictionary<string, object> dictionary, 
            Type type, JavaScriptSerializer serializer, bool throwOnError, out object convertedObject)
        {
            Type t = type;
            string typeId = null;
            object customObject = dictionary;
            object typeKey;

            if (dictionary.TryGetValue(JavaScriptSerializer.ServerTypeFieldName, out typeKey))
            {
                if (!TryConvertObjectToType(typeKey, typeof(string), serializer, throwOnError, out typeKey))
                {
                    convertedObject = null;
                    return false;
                }

                typeId = (string)typeKey;
                if (typeId != null)
                {
                    if (serializer.TypeResolver != null)
                    {
                        t = serializer.TypeResolver.ResolveType(typeId);
                        if (t == null)
                        {
                            if (throwOnError)
                                throw Error.CouldNotResolveType(typeId);
                            
                            convertedObject = null;
                            return false;
                        }
                    }
                    dictionary.Remove(JavaScriptSerializer.ServerTypeFieldName);
                }
            }

            JavaScriptConverter converter = null;
            if (t != null && serializer.TryGetConverter(t, out converter))
            {
                try
                {
                    convertedObject = converter.Deserialize(dictionary, t, serializer);
                    return true;
                }
                catch
                {
                    if (throwOnError)
                        throw;
                    
                    convertedObject = null;
                    return false;
                }
            }
            if (typeId != null || IsClientInstantiatableType(t, serializer))
                customObject = Activator.CreateInstance(t);
            
            List<string> list = new List<string>(dictionary.Keys);
            if (IsGenericDictionary(type))
            {
                Type keyType = type.GetGenericArguments()[0];
                if (!keyType.IsPrimitive)
                {
                    if (throwOnError)
                        throw Error.SuppliedDictionaryTypeIsNotSupported();
                    
                    convertedObject = null;
                    return false;
                }

                Type valueType = type.GetGenericArguments()[1];
                IDictionary dict = null;

                if (IsClientInstantiatableType(type, serializer))
                    dict = (IDictionary)Activator.CreateInstance(type);
                
                else
                    dict = (IDictionary)Activator.CreateInstance(
                        _dictionaryGenericType.MakeGenericType(keyType, valueType));
                
                if (dict != null)
                {
                    foreach (string key in list)
                    {
						object convertedKey = Convert.ChangeType(
							key, keyType, CultureInfo.InvariantCulture);
                        object value;
						
                        if (!TryConvertObjectToType(dictionary[key], 
                            valueType, serializer, throwOnError, out value))
                        {
                            convertedObject = null;
                            return false;
                        }
                        dict[convertedKey] = value;
                    }

                    convertedObject = dict;
                    return true;
                }
            }

            if (type != null && !type.IsAssignableFrom(customObject.GetType()))
            {
                if (!throwOnError)
                {
                    convertedObject = null;
                    return false;
                }
                if (type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null) == null)
                    throw Error.TypeMustHaveParameterlessConstructor(type);
                
                throw Error.DeserializerTypeMismatch();
            }

            foreach (string key in list)
            {
                object propertyValue = dictionary[key];
                if (!AssignToPropertyOrField(propertyValue, customObject, 
                    key, serializer, throwOnError))
                {
                    convertedObject = null;
                    return false;
                }
            }
            convertedObject = customObject;
            return true;
        }

        private static bool ConvertList(IList list, Type type, JavaScriptSerializer serializer, 
            bool throwOnError, out IList convertedList)
        {
            if ((type == null || type == typeof(object)) || IsArrayListCompatible(type))
            {
                Type elementType = typeof(object);
                if (type != null && type != typeof(object))
                    elementType = type.GetElementType();
                
                ArrayList newList = new ArrayList();
                if (!AddItemToList(list, newList, elementType, serializer, throwOnError))
                {
                    convertedList = null;
                    return false;
                }

                if (type == typeof(ArrayList) || type == typeof(IEnumerable) || 
                    type == typeof(IList) || type == typeof(ICollection))
                {
                    convertedList = newList;
                    return true;
                }

                convertedList = newList.ToArray(elementType);
                return true;
            }

            if (type.IsGenericType && (type.GetGenericArguments().Length == 1))
            {
                Type elementType = type.GetGenericArguments()[0];
                if (_enumerableGenericType.MakeGenericType(elementType).IsAssignableFrom(type))
                {
                    Type genericListType = _listGenericType.MakeGenericType(elementType);
                    IList genericList = null;

                    if (IsClientInstantiatableType(type, serializer) && 
                        typeof(IList).IsAssignableFrom(type))
                    {
                        genericList = (IList)Activator.CreateInstance(type);
                    }
                    else
                    {
						if (!type.IsAssignableFrom(genericListType))
                        {
                            if (throwOnError)
                                throw Error.CouldNotCreateListType(genericListType);
                            
                            convertedList = null;
                            return false;
                        }
                        genericList = (IList)Activator.CreateInstance(genericListType);
                    }

                    if (!AddItemToList(list, genericList, elementType, serializer, throwOnError))
                    {
                        convertedList = null;
                        return false;
                    }
                    convertedList = genericList;
                    return true;
                }
            }
            else if (IsClientInstantiatableType(type, serializer) && typeof(IList).IsAssignableFrom(type))
            {
                IList simpleList = (IList)Activator.CreateInstance(type);
                if (!AddItemToList(list, simpleList, null, serializer, throwOnError))
                {
                    convertedList = null;
                    return false;
                }
                convertedList = simpleList;
                return true;
            }
            if (throwOnError)
                throw Error.ArrayTypeIsNotSupported(type);
            
            convertedList = null;
            return false;
        }

        internal static object ConvertObject(object obj, Type type, JavaScriptSerializer serializer)
        {
            object result;
            TryConvertObjectToType(obj, type, serializer, true, out result);

            return result;
        }
        #endregion
    }
}

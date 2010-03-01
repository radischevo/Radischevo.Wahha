using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Radischevo.Wahha.Core
{
    public static class DictionaryExtensions
    {
        #region Nested Types
        public delegate bool TryGetValueDelegate(object dictionary, string key, out object value);
        #endregion

        #region Static Fields
        private static readonly MethodInfo _strongTryGetValueImplInfo = typeof(DictionaryExtensions)
            .GetMethod("StrongTryGetValueImpl", BindingFlags.NonPublic | BindingFlags.Static);
        #endregion

        #region Static Methods
        public static bool TryGetValue(this IDictionary dictionary, string key, out object value)
        {
			Precondition.Require(dictionary, () => Error.ArgumentNull("dictionary"));
            TryGetValueDelegate dlgt = CreateTryGetValueDelegate(dictionary.GetType());

            if (dlgt == null)
                throw Error.CouldNotCreateTryGetValueDelegateForType(dictionary.GetType());

            return dlgt(dictionary, key, out value);
        }

        private static TryGetValueDelegate CreateTryGetValueDelegate(Type targetType)
        {
            Type dictionaryType = targetType.GetGenericInterface(typeof(IDictionary<,>));
            if (dictionaryType != null)
            {
                Type[] typeArguments = dictionaryType.GetGenericArguments();
                Type keyType = typeArguments[0];
                Type returnType = typeArguments[1];

                if (keyType.IsAssignableFrom(typeof(string)))
                {
                    MethodInfo strongImplInfo = _strongTryGetValueImplInfo.MakeGenericMethod(keyType, returnType);
                    return (TryGetValueDelegate)Delegate.CreateDelegate(typeof(TryGetValueDelegate), strongImplInfo);
                }
            }
            if (typeof(IDictionary).IsAssignableFrom(targetType))
                return TryGetValueFromNonGenericDictionary;

            return null;
        }

        private static bool StrongTryGetValueImpl<TKey, TValue>(object dictionary, string key, out object value)
        {
            IDictionary<TKey, TValue> strongDict = (IDictionary<TKey, TValue>)dictionary;

            TValue strongValue;
            bool retVal = strongDict.TryGetValue((TKey)(object)key, out strongValue);
            value = strongValue;

            return retVal;
        }

        private static bool TryGetValueFromNonGenericDictionary(object dictionary, string key, out object value)
        {
            IDictionary weakDict = (IDictionary)dictionary;

            bool containsKey = weakDict.Contains(key);
            value = (containsKey) ? weakDict[key] : null;

            return containsKey;
        }
        #endregion
    }
}

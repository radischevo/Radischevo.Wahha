using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Radischevo.Wahha.Web.Mvc
{
    internal static class CollectionHelpers
    {
        #region Static Fields
        private static readonly MethodInfo _copyArrayMethod = typeof(CollectionHelpers)
            .GetMethod("CopyArrayImpl", BindingFlags.Static | BindingFlags.NonPublic);
        private static readonly MethodInfo _copyCollectionMethod = typeof(CollectionHelpers)
            .GetMethod("CopyCollectionImpl", BindingFlags.Static | BindingFlags.NonPublic);
        private static readonly MethodInfo _copyDictionaryMethod = typeof(CollectionHelpers)
            .GetMethod("CopyDictionaryImpl", BindingFlags.Static | BindingFlags.NonPublic);
        #endregion

        #region Static Methods
        public static void CopyArray(Type elementType, object collection, object contents)
        {
            MethodInfo targetMethod = _copyArrayMethod.MakeGenericMethod(elementType);
            targetMethod.Invoke(null, new object[] { collection, contents });
        }

        public static void CopyCollection(Type elementType, object collection, object contents)
        {
            MethodInfo targetMethod = _copyCollectionMethod.MakeGenericMethod(elementType);
            targetMethod.Invoke(null, new object[] { collection, contents });
        }

        public static void CopyDictionary(Type keyType, Type valueType, 
            object dictionary, object contents)
        {
            MethodInfo targetMethod = _copyDictionaryMethod.MakeGenericMethod(keyType, valueType);
            targetMethod.Invoke(null, new object[] { dictionary, contents });
        }

        private static void CopyArrayImpl<T>(Array collection, IEnumerable contents)
        {
            if (contents != null)
            {
                int index = 0;
                foreach (object item in contents)
                {
                    // if the item was not a T, some conversion failed. the error message will be propagated,
                    // but in the meanwhile we need to make a placeholder element in the array.
                    T castItem = (item is T) ? (T)item : default(T);
                    if (index >= collection.GetLength(0))
                        return;

                    collection.SetValue(castItem, index++);
                }
            }
        }

        private static void CopyCollectionImpl<T>(ICollection<T> collection, IEnumerable contents)
        {
            collection.Clear();
            if (contents != null)
            {
                foreach (object item in contents)
                {
                    // if the item was not a T, some conversion failed. the error message will be propagated,
                    // but in the meanwhile we need to make a placeholder element in the array.
                    T castItem = (item is T) ? (T)item : default(T);
                    collection.Add(castItem);
                }
            }
        }

        private static void CopyDictionaryImpl<TKey, TValue>(IDictionary<TKey, TValue> dictionary, 
            IEnumerable<KeyValuePair<object, object>> contents)
        {
            dictionary.Clear();
            foreach (KeyValuePair<object, object> item in contents)
            {
                // if the item was not a T, some conversion failed. the error message will be propagated,
                // but in the meanwhile we need to make a placeholder element in the dictionary.
                TKey castKey = (TKey)item.Key; // this cast shouldn't fail
                TValue castValue = (item.Value is TValue) ? 
                    (TValue)item.Value : default(TValue);

                dictionary[castKey] = castValue;
            }
        }
        #endregion
    }
}

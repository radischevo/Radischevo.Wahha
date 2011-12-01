using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Web.Mvc
{
    internal static class CollectionHelpers
    {
        #region Static Methods
        public static void CopyArray<T>(T[] collection, IEnumerable<T> contents)
        {
            if (contents != null)
            {
                int index = 0;
                foreach (T item in contents)
                {
                    if (index >= collection.GetLength(0))
                        return;

                    collection.SetValue(item, index++);
                }
            }
        }

        public static void CopyCollection<T>(ICollection<T> collection, IEnumerable<T> contents)
        {
            collection.Clear();
            if (contents != null)
            {
                foreach (T item in contents)
                {
                    collection.Add(item);
                }
            }
        }

        public static void CopyDictionary<TKey, TValue>(IDictionary<TKey, TValue> dictionary, 
            IEnumerable<KeyValuePair<TKey, TValue>> contents)
        {
            dictionary.Clear();
            foreach (KeyValuePair<TKey, TValue> item in contents)
            {
                dictionary[item.Key] = item.Value;
            }
        }
        #endregion
    }
}

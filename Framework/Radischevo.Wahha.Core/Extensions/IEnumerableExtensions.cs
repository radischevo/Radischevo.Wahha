using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Radischevo.Wahha.Core
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> Convert<T>(this IEnumerable collection)
        {
            foreach (object item in collection)
                yield return Converter.ChangeType<T>(item, CultureInfo.InvariantCulture);
        }

        public static IEnumerable<T> Each<T>(this IEnumerable<T> instance, Action<T> action)
        {
            foreach (T item in instance)
                action(item);

			return instance;
        }

        public static IEnumerable<T> Each<T>(this IEnumerable<T> instance, Action<T, int> action)
        {
            int index = 0;
            foreach (T item in instance)
                action(item, index++);

			return instance;
        }

        public static ReadOnlyCollection<T> AsReadOnly<T>(this IEnumerable<T> collection)
        {
            ReadOnlyCollection<T> roc = (collection as ReadOnlyCollection<T>);
            if (roc == null)
            {
                if (collection == null)
                    roc = EmptyReadOnlyCollection<T>.Empty;
                else
                    roc = new List<T>(collection).AsReadOnly();
            }
            return roc;
        }

        internal class EmptyReadOnlyCollection<T>
        {
            internal static readonly ReadOnlyCollection<T> Empty = new List<T>().AsReadOnly();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Radischevo.Wahha.Core
{
    public static class EnumerableExtensions
	{
		#region Nested Types
		private class EmptyReadOnlyCollection<T>
		{
			#region Static Fields
			public static readonly ReadOnlyCollection<T> Empty = new List<T>().AsReadOnly();
			#endregion
		}
		#endregion

		#region Extension Methods
		public static IEnumerable<T> Convert<T>(this IEnumerable collection)
        {
			return Convert<T>(collection, CultureInfo.CurrentCulture);
        }

		public static IEnumerable<T> Convert<T>(this IEnumerable collection, IFormatProvider provider)
		{
			foreach (object item in collection)
				yield return Converter.ChangeType<T>(item, provider);
		}

        public static IEnumerable<T> Each<T>(this IEnumerable<T> instance, Action<T> action)
        {
			foreach (T item in instance)
			{
				action(item);
				yield return item;
			}
        }

        public static IEnumerable<T> Each<T>(this IEnumerable<T> instance, Action<T, int> action)
        {
            int index = 0;
			foreach (T item in instance)
			{
				action(item, index++);
				yield return item;
			}
        }

		public static void ForEach<T>(this IEnumerable<T> instance, Action<T> action)
		{
			foreach (T item in instance)
				action(item);
		}

		public static void ForEach<T>(this IEnumerable<T> instance, Action<T, int> action)
		{
			int index = 0;
			foreach (T item in instance)
				action(item, index++);
		}

        public static ReadOnlyCollection<T> AsReadOnly<T>(this IEnumerable<T> collection)
        {
			ReadOnlyCollection<T> list = (collection as ReadOnlyCollection<T>);
            if (list == null)
            {
                if (collection == null)
                    return EmptyReadOnlyCollection<T>.Empty;

                return new List<T>(collection).AsReadOnly();
            }
            return list;
		}
		#endregion
	}
}

using System;
using System.Collections.Generic;

namespace Jeltofiol.Wahha.Data.Linq
{
    public static class TopologicalSorter
    {
        public static IEnumerable<T> Sort<T>(this IEnumerable<T> items, 
            Func<T, IEnumerable<T>> itemsBefore)
        {
            return Sort<T>(items, itemsBefore, null);
        }

        public static IEnumerable<T> Sort<T>(this IEnumerable<T> items, 
            Func<T, IEnumerable<T>> itemsBefore, IEqualityComparer<T> comparer)
        {
            List<T> result = new List<T>();
            HashSet<T> seen = (comparer != null) ? new HashSet<T>(comparer) : new HashSet<T>();
            HashSet<T> done = (comparer != null) ? new HashSet<T>(comparer) : new HashSet<T>();

            foreach (T item in items)
                SortItem(item, itemsBefore, seen, done, result);
            
            return result;
        }

        private static void SortItem<T>(T item, Func<T, IEnumerable<T>> itemsBefore, 
            HashSet<T> seen, HashSet<T> done, List<T> result)
        {
            if (!done.Contains(item))
            {
                if (seen.Contains(item))
                    throw Error.CycleInTopologicalSort();
                
                seen.Add(item);
                IEnumerable<T> items = itemsBefore(item);
                
                if (items != null)
                {
                    foreach (T itemBefore in items)
                        SortItem(itemBefore, itemsBefore, seen, done, result);
                }
                result.Add(item);
                done.Add(item);
            }
        }
    }
}

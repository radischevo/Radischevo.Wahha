using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Radischevo.Wahha.Core
{
    /// <summary>
    /// This helper class is used throughout the framework. It provides assertion methods,
    /// standardized error reporting and automatic exception creation. 
    /// </summary>
    public static class Precondition
    {
        #region Internal Methods
        /// <summary>
        /// Internal method performing throwing of exceptions.
        /// </summary>
        /// <param name="error">The exception leading to this error (if any).</param>
        private static void Fail(Exception error)
        {
            if (error != null)
                throw error;
        }
        #endregion

        #region Static Methods
        public static void Require(bool condition, Exception error)
        {
            if (!condition)
                Fail(error);
        }

        public static void Require(object obj, Exception error)
        {
            if (obj == null)
                Fail(error);
        }

        public static void Exist<T>(IEnumerable<T> collection, T value, 
            Exception error)
        {
            if (!collection.Contains(value))
                Fail(error);
        }

        public static void Exist<T>(IEnumerable<T> collection, T value, 
            IEqualityComparer<T> comparer, Exception error)
        {
            if (!collection.Contains(value, comparer))
                Fail(error);
        }

        public static void Any<T>(IEnumerable<T> collection, 
            Func<T, bool> predicate, Exception error)
        {
            if (!collection.Any(predicate))
                Fail(error);
        }

        public static void All<T>(IEnumerable<T> collection,
            Func<T, bool> predicate, Exception error)
        {
            if (!collection.All(predicate))
                Fail(error);
        }

        public static void Require<TException>(bool condition)
            where TException : Exception, new()
        {
            if (!condition)
                Fail(new TException());
        }

        public static void Require<TException>(object obj)
            where TException : Exception, new()
        {
            if (obj == null)
                Fail(new TException());
        }

        public static void Exist<T, TException>(IEnumerable<T> collection, T value)
            where TException : Exception, new()
        {
            if (!collection.Contains(value))
                Fail(new TException());
        }

        public static void Exist<T, TException>(IEnumerable<T> collection, T value,
            IEqualityComparer<T> comparer)
            where TException : Exception, new()
        {
            if (!collection.Contains(value, comparer))
                Fail(new TException());
        }

        public static void Any<T, TException>(IEnumerable<T> collection,
            Func<T, bool> predicate)
            where TException : Exception, new()
        {
            if (!collection.Any(predicate))
                Fail(new TException());
        }

        public static void All<T, TException>(IEnumerable<T> collection,
            Func<T, bool> predicate)
            where TException : Exception, new()
        {
            if (!collection.All(predicate))
                Fail(new TException());
        }
        #endregion
    }
}

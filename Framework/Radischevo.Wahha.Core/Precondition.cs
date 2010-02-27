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
        private static void Fail(Exception error)
        {
            if (error != null)
                throw error;
        }

		private static void Fail<TException>(Func<TException> error)
			where TException : Exception
		{
			if (error != null)
				Fail(error());
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

		public static void Defined(string str, Exception error)
		{
			if (String.IsNullOrEmpty(str))
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

        public static void Require<TException>(
			bool condition, Func<TException> error)
            where TException : Exception
        {
            if (!condition)
                Fail(error);
        }

        public static void Require<TException>(
			object obj, Func<TException> error)
            where TException : Exception
        {
            if (obj == null)
                Fail(error);
        }

		public static void Defined<TException>(
			string str, Func<TException> error)
			where TException : Exception
		{
			if (String.IsNullOrEmpty(str))
				Fail(error);
		}

        public static void Exist<T, TException>(
			IEnumerable<T> collection, T value, 
			Func<TException> error)
            where TException : Exception
        {
            if (!collection.Contains(value))
                Fail(error);
        }

        public static void Exist<T, TException>(IEnumerable<T> collection, 
			T value, IEqualityComparer<T> comparer, Func<TException> error)
            where TException : Exception
        {
            if (!collection.Contains(value, comparer))
                Fail(error);
        }

        public static void Any<T, TException>(IEnumerable<T> collection,
			Func<T, bool> predicate, Func<TException> error)
			where TException : Exception
        {
            if (!collection.Any(predicate))
                Fail(error);
        }

        public static void All<T, TException>(IEnumerable<T> collection,
            Func<T, bool> predicate, Func<TException> error)
            where TException : Exception
        {
            if (!collection.All(predicate))
                Fail(error);
        }
        #endregion
    }
}

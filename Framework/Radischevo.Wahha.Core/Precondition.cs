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
		#region Nested Types
		/// <summary>
		/// Descibes a type acting like an extension point 
		/// for condition checking.
		/// </summary>
		public sealed class Registry : IHideObjectMembers
		{
			#region Instance Methods
			public void Throw(Exception error)
			{
				if (error != null)
					throw error;
			}

			public void Throw<TException>(Func<TException> error)
				where TException : Exception
			{
				if (error != null)
					Throw(error());
			}
			#endregion
		}
		#endregion

		#region Static Fields
		private static readonly Registry _methods = new Registry();
		#endregion

		#region Static Properties
		/// <summary>
		/// Gets an extension point for precondition check methods.
		/// </summary>
		public static Registry Methods
		{
			get
			{
				return _methods;
			}
		}
		#endregion

		#region Internal Methods
		private static void Throw(Exception error)
        {
            if (error != null)
                throw error;
        }

		private static void Throw<TException>(Func<TException> error)
			where TException : Exception
		{
			if (error != null)
				Throw(error());
		}
        #endregion

        #region Static Methods
        public static void Require(bool condition, Exception error)
        {
            if (!condition)
                Throw(error);
        }

        public static void Require(object obj, Exception error)
        {
            if (obj == null)
                Throw(error);
        }

		public static void Defined(string str, Exception error)
		{
			if (String.IsNullOrEmpty(str))
				Throw(error);
		}

        public static void Exist<T>(IEnumerable<T> collection, T value, 
            Exception error)
        {
            if (!collection.Contains(value))
                Throw(error);
        }

        public static void Exist<T>(IEnumerable<T> collection, T value, 
            IEqualityComparer<T> comparer, Exception error)
        {
            if (!collection.Contains(value, comparer))
                Throw(error);
        }

        public static void Any<T>(IEnumerable<T> collection, 
            Func<T, bool> predicate, Exception error)
        {
            if (!collection.Any(predicate))
                Throw(error);
        }

        public static void All<T>(IEnumerable<T> collection,
            Func<T, bool> predicate, Exception error)
        {
            if (!collection.All(predicate))
                Throw(error);
        }

        public static void Require<TException>(
			bool condition, Func<TException> error)
            where TException : Exception
        {
            if (!condition)
                Throw(error);
        }

        public static void Require<TException>(
			object obj, Func<TException> error)
            where TException : Exception
        {
            if (obj == null)
                Throw(error);
        }

		public static void Defined<TException>(
			string str, Func<TException> error)
			where TException : Exception
		{
			if (String.IsNullOrEmpty(str))
				Throw(error);
		}

        public static void Exist<T, TException>(
			IEnumerable<T> collection, T value, 
			Func<TException> error)
            where TException : Exception
        {
            if (!collection.Contains(value))
                Throw(error);
        }

        public static void Exist<T, TException>(IEnumerable<T> collection, 
			T value, IEqualityComparer<T> comparer, Func<TException> error)
            where TException : Exception
        {
            if (!collection.Contains(value, comparer))
                Throw(error);
        }

        public static void Any<T, TException>(IEnumerable<T> collection,
			Func<T, bool> predicate, Func<TException> error)
			where TException : Exception
        {
            if (!collection.Any(predicate))
                Throw(error);
        }

        public static void All<T, TException>(IEnumerable<T> collection,
            Func<T, bool> predicate, Func<TException> error)
            where TException : Exception
        {
            if (!collection.All(predicate))
                Throw(error);
        }
        #endregion
    }
}

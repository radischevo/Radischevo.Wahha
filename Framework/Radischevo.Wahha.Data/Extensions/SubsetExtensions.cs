using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public static class SubsetExtensions
	{
		#region Extension Methods
		/// <summary>
		/// Creates a subset over the provided collection.
		/// </summary>
		/// <typeparam name="T">The type of collection elements.</typeparam>
		/// <param name="collection">The collection to wrap.</param>
		public static ISubset<T> ToSubset<T>(this IEnumerable<T> collection)
		{
			return ToSubset(collection, -1);
		}

		/// <summary>
		/// Creates a subset over the provided collection.
		/// </summary>
		/// <typeparam name="T">The type of collection elements.</typeparam>
		/// <param name="collection">The collection to wrap.</param>
		/// <param name="total">The total number of elements that can be retrieved 
		/// from the sequence.</param>
		public static ISubset<T> ToSubset<T>(this IEnumerable<T> collection, int total)
		{
			return new Subset<T>(collection, total);
		}

		/// <summary>
		/// Returns the total number of elements that can be 
		/// retrieved from the sequence.
		/// </summary>
		public static int Total<T>(this IEnumerable<T> collection)
		{
			ISubset subset = (collection as ISubset);
			if (subset == null)
				return collection.Count();

			return subset.Total;
		}

		/// <summary>
		/// Converts the elements of an <see cref="T:Radischevo.Wahha.Data.ISubset"/> 
		/// to the specified type.
		/// </summary>
		/// <typeparam name="TInput">The type to convert the elements of source from.</typeparam>
		/// <typeparam name="TOutput">The type to convert the elements of source to.</typeparam>
		/// <param name="collection">The <see cref="T:Radischevo.Wahha.Data.ISubset"/>
		/// that contains the elements to be converted.</param>
		public static IEnumerable<TOutput> Cast<TInput, TOutput>(
			this ISubset<TInput> collection)
		{
			return new Subset<TOutput>(collection.Cast<TOutput>(), collection.Total);
		}

		/// <summary>
		/// Converts the elements of an <see cref="T:Radischevo.Wahha.Data.ISubset"/> 
		/// to the specified type.
		/// </summary>
		/// <typeparam name="TInput">The type to convert the elements of source from.</typeparam>
		/// <typeparam name="TOutput">The type to convert the elements of source to.</typeparam>
		/// <param name="collection">The <see cref="T:Radischevo.Wahha.Data.ISubset"/>
		/// that contains the elements to be converted.</param>
		public static IEnumerable<TOutput> Convert<TInput, TOutput>(
			this ISubset<TInput> collection)
		{
			return Convert<TInput, TOutput>(collection, CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Converts the elements of an <see cref="T:Radischevo.Wahha.Data.ISubset"/> 
		/// to the specified type.
		/// </summary>
		/// <typeparam name="TInput">The type to convert the elements of source from.</typeparam>
		/// <typeparam name="TOutput">The type to convert the elements of source to.</typeparam>
		/// <param name="collection">The <see cref="T:Radischevo.Wahha.Data.ISubset"/>
		/// that contains the elements to be converted.</param>
		/// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation 
		/// that supplies culture-specific formatting information.</param>
		public static IEnumerable<TOutput> Convert<TInput, TOutput>(
			this ISubset<TInput> collection, IFormatProvider provider)
		{
			return new Subset<TOutput>(collection.Convert<TOutput>(provider), collection.Total);
		}
		#endregion
	}
}

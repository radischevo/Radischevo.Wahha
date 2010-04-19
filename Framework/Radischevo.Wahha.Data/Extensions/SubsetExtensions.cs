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
		public static IEnumerable<TOutput> CastSubset<TInput, TOutput>(
			this IEnumerable<TInput> collection)
		{
			ISubset<TInput> input = (collection as ISubset<TInput>);
			return new Subset<TOutput>(collection.Cast<TOutput>(),
				input.Evaluate(s => s.Total, -1));
		}

		/// <summary>
		/// Converts the elements of an <see cref="T:Radischevo.Wahha.Data.ISubset"/> 
		/// to the specified type.
		/// </summary>
		/// <typeparam name="TInput">The type to convert the elements of source from.</typeparam>
		/// <typeparam name="TOutput">The type to convert the elements of source to.</typeparam>
		/// <param name="collection">The <see cref="T:Radischevo.Wahha.Data.ISubset"/>
		/// that contains the elements to be converted.</param>
		public static IEnumerable<TOutput> ConvertSubset<TInput, TOutput>(
			this IEnumerable<TInput> collection)
		{
			return ConvertSubset<TInput, TOutput>(collection, CultureInfo.CurrentCulture);
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
		public static IEnumerable<TOutput> ConvertSubset<TInput, TOutput>(
			this IEnumerable<TInput> collection, IFormatProvider provider)
		{
			ISubset<TInput> input = (collection as ISubset<TInput>);
			return new Subset<TOutput>(collection.Convert<TOutput>(provider),
				input.Evaluate(s => s.Total, -1));
		}
		#endregion
	}
}

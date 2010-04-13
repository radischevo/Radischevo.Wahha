using System;
using System.Collections;
using System.Collections.Generic;

namespace Radischevo.Wahha.Core
{
    public interface IValueSet
    {
        #region Instance Properties
        object this[string key]
        {
            get;
        }

        IEnumerable<string> Keys
        {
            get;
        }
        #endregion

        #region Instance Methods
        /// <summary>
		/// Gets the typed value with the specified key.
		/// </summary>
		/// <typeparam name="TValue">The type of value.</typeparam>
		/// <param name="key">The key to find.</param>
		/// <param name="defaultValue">The default value of the variable.</param>
		/// <param name="provider">An <see cref="IFormatProvider" /> interface implementation that 
		/// supplies culture-specific formatting information.</param>
		TValue GetValue<TValue>(string key, TValue defaultValue, IFormatProvider provider);
        #endregion
    }
}

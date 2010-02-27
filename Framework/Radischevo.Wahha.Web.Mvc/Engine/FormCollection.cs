using System;
using System.Collections;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    /// <summary>
    /// Provides a value collection suitable for 
    /// form binding scenarios.
    /// </summary>
    public class FormCollection : IValueSet
    {
        #region Instance Fields
        private IValueSet _collection;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Radischevo.Wahha.Web.Mvc.FormCollection"/> class.
        /// </summary>
        public FormCollection()
            : this(new ValueDictionary())
        {
        }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Radischevo.Wahha.Web.Mvc.FormCollection"/> class.
        /// </summary>
        /// <param name="collection">An existing collection to be 
        /// used as a data source for the current instance.</param>
        public FormCollection(IValueSet collection)
        {
            Precondition.Require(collection, 
				() => Error.ArgumentNull("collection"));
            _collection = collection;
        }
        #endregion

        #region Instance Properties
        public object this[string key]
        {
            get 
            {
                return _collection[key];
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return _collection.Keys;
            }
        }
        #endregion

        #region Instance Methods
        public TValue GetValue<TValue>(string key)
        {
            return GetValue<TValue>(key, default(TValue));
        }

        public TValue GetValue<TValue>(string key, TValue defaultValue)
        {
            return _collection.GetValue<TValue>(key, defaultValue);
        }
        #endregion
    }
}

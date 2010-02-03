using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
    /// <summary>
    /// Provides a two-way column lookup 
    /// dictionary for the result set. 
    /// Designed for internal use by the 
    /// <see cref="Radischevo.Wahha.Data.DbDataReader"/> 
    /// class.
    /// </summary>
    internal class DbFieldLookup
    {
        #region Instance Fields
        private IDictionary<string, int> _ordinalLookup;
        private IDictionary<int, string> _nameLookup;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Radischevo.Wahha.Data.DbFieldLookup"/> 
        /// class.
        /// </summary>
        public DbFieldLookup()
        {
            _nameLookup = new Dictionary<int, string>();
            _ordinalLookup = new Dictionary<string, int>(
                StringComparer.InvariantCultureIgnoreCase);
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets the collection of the field ordinals 
        /// which exist in the lookup dictionary.
        /// </summary>
        public IEnumerable<int> Ordinals
        {
            get
            {
                return _nameLookup.Keys;
            }
        }

        /// <summary>
        /// Gets the collection of the field 
        /// names which exist in 
        /// the lookup dictionary.
        /// </summary>
        public IEnumerable<string> Names
        {
            get
            {
                return _ordinalLookup.Keys;
            }
        }
        #endregion

        #region Instance Methods
        private void Remove(int ordinal, string name)
        {
            _nameLookup.Remove(ordinal);
            _ordinalLookup.Remove(name);
        }

        /// <summary>
        /// Adds a new lookup field into the 
        /// dictionary.
        /// </summary>
        /// <param name="ordinal">The zero-based field ordinal.</param>
        /// <param name="name">The name of the field.</param>
        public void Add(int ordinal, string name)
        {
            Precondition.Require(ordinal >= 0, 
                Error.ParameterMustBeGreaterThanOrEqual("ordinal", 0, ordinal));

			if (String.IsNullOrEmpty(name))
				name = "F" + ordinal;

            if (_ordinalLookup.ContainsKey(name))
                throw Error.AmbiguousColumnName(name);

            _nameLookup.Add(ordinal, name);
            _ordinalLookup.Add(name, ordinal);
        }

        /// <summary>
        /// Clear the lookup dictionary.
        /// </summary>
        public void Clear()
        {
            _nameLookup.Clear();
            _ordinalLookup.Clear();
        }

        /// <summary>
        /// Removes an entry with the specified ordinal 
        /// from the lookup dictionary.
        /// </summary>
        /// <param name="ordinal">The zero-based field ordinal.</param>
        public bool Remove(int ordinal)
        {
            string name;
            if (_nameLookup.TryGetValue(ordinal, out name))
            {
                Remove(ordinal, name);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes an entry with the specified name 
        /// from the lookup dictionary.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        public bool Remove(string name)
        {
            int ordinal;
            if (_ordinalLookup.TryGetValue(name, out ordinal))
            {
                Remove(ordinal, name);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets a value indicating whether the 
        /// field with the specified ordinal 
        /// exists in the lookup dictionary.
        /// </summary>
        /// <param name="ordinal">The zero-based field ordinal.</param>
        public bool Contains(int ordinal)
        {
            return _nameLookup.ContainsKey(ordinal);
        }

        /// <summary>
        /// Gets a value indicating whether the 
        /// field with the specified name 
        /// exists in the lookup dictionary.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        public bool Contains(string name)
        {
            return _ordinalLookup.ContainsKey(name);
        }

        /// <summary>
        /// Gets the name of the column with 
        /// the specified ordinal.
        /// </summary>
        /// <param name="ordinal">The zero-based field ordinal.</param>
        /// <param name="name">When the method returns, the name of the 
        /// column with the specified ordinal, if the column is found;
        /// otherwise, <value>null</value> is returned. 
        /// The parameter is passed uninitialized.</param>
        public bool TryGetName(int ordinal, out string name)
        {
            return (_nameLookup.TryGetValue(ordinal, out name));
        }

        /// <summary>
        /// Gets the zero-based ordinal of the column 
        /// with the specified name.
        /// </summary>
        /// <param name="ordinal">When the method returns, the 
        /// zero-based ordinal of the column with the specified name, 
        /// if the column is found; otherwise, <value>-1</value> 
        /// is returned. The parameter is passed uninitialized.</param>
        public bool TryGetOrdinal(string name, out int ordinal)
        {
            if (_ordinalLookup.TryGetValue(name, out ordinal))
                return true;

            ordinal = -1;
            return false;
        }
        #endregion
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Radischevo.Wahha.Core
{
    /// <summary>
    /// Provide a delegated lazy link 
    /// to a collection of objects.
    /// </summary>
    /// <typeparam name="T">The element type 
    /// of the linked collection.</typeparam>
    public class EnumerableLink<T> : IEnumerableLink<T>
    {
        #region Instance Fields
		private readonly object _lock = new object();
        private bool _hasLoadedValue;
        private bool _hasAssignedValue;
        private int _count;
		private object _tag;
        private Func<IEnumerable<T>> _source;
        private IEnumerable<T> _collection;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance 
        /// of the <see cref="Radischevo.Wahha.Core.EnumerableLink"/> class.
        /// </summary>
        public EnumerableLink()
        {
            _count = -1;
			_source = () => Enumerable.Empty<T>();
        }

        /// <summary>
        /// Initializes a new instance 
        /// of the <see cref="Radischevo.Wahha.Core.EnumerableLink"/> class 
        /// using the specified delegate, which is used to load the 
        /// linked collection into a link.
        /// </summary>
        public EnumerableLink(Func<IEnumerable<T>> source)
            : this()
        {
            _source = source;
        }

        /// <summary>
        /// Initializes a new instance 
        /// of the <see cref="Radischevo.Wahha.Core.EnumerableLink"/> class 
        /// using the specified collection.
        /// </summary>
        public EnumerableLink(IEnumerable<T> collection)
            : this()
        {
            _hasAssignedValue = true;
            _collection = collection;
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets a value, indicating 
        /// whether a linked collection 
        /// is loaded into the link.
        /// </summary>
        public virtual bool HasValue
        {
            get
            {
                return (_hasLoadedValue || _hasAssignedValue);
            }
        }

		/// <summary>
		/// Gets or sets the specific 
		/// tag that can help to determine the 
		/// linked object without loading.
		/// </summary>
		public object Tag
		{
			get
			{
				return _tag;
			}
			set
			{
				_tag = value;
			}
		}

        /// <summary>
        /// Gets or sets a delegate function, 
        /// which will be used to load the 
        /// linked object.
        /// </summary>
        public Func<IEnumerable<T>> Source
        {
            get
            {
                return _source;
            }
            set
            {
				_hasLoadedValue = false;
                _source = value;
            }
        }

        /// <summary>
        /// Gets or sets a number of elements in a sequence 
        /// or a maximum number of elements, that can be 
        /// retrieved from the collection.
        /// </summary>
        public virtual int Count
        {
            get
            {
                if (_count < 0)
                    _count = this.Count();

                return _count;
            }
            set
            {
                Precondition.Require(value > -1,
					() => Error.ParameterMustBeGreaterThanOrEqual("value", 0, value));
                _count = value;
            }
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Explicitly loads the linked collection 
        /// into the link.
        /// </summary>
        public virtual void Load()
        {
            if (!_hasLoadedValue)
            {
				lock (_lock)
				{
					if (!_hasLoadedValue)
					{
						Precondition.Require((_hasAssignedValue || _source != null),
							() => Error.LinkSourceIsNotInitialized());

						_collection = _source();
						_hasLoadedValue = true;
						_source = null;
					}
				}
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates 
        /// through the collection.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            if (!HasValue)
                Load();

            return _collection.GetEnumerator();
        }
        #endregion

        #region IEnumerable Members
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}

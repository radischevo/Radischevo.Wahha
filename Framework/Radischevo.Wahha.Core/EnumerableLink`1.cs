using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Radischevo.Wahha.Core
{
    /// <summary>
    /// Provide a delegated lazy link 
    /// to a collection of objects.
    /// </summary>
    /// <typeparam name="T">The element type 
    /// of the linked collection.</typeparam>
	[Serializable]
    public class EnumerableLink<T> : LinkBase<IEnumerable<T>>, IEnumerableLink<T>
	{
		#region Static Fields
		private static Func<IEnumerable<T>> _defaultSource;
		#endregion

		#region Instance Fields
		private int _count;
        #endregion

        #region Constructors
		static EnumerableLink()
		{
			_defaultSource = () => Enumerable.Empty<T>();
		}

        /// <summary>
        /// Initializes a new instance 
		/// of the <see cref="Radischevo.Wahha.Core.EnumerableLink{T}"/> class.
        /// </summary>
        public EnumerableLink() 
			: this(_defaultSource)
        {
        }

        /// <summary>
        /// Initializes a new instance 
		/// of the <see cref="Radischevo.Wahha.Core.EnumerableLink{T}"/> class 
        /// using the specified delegate, which is used to load the 
        /// linked collection into a link.
        /// </summary>
        public EnumerableLink(Func<IEnumerable<T>> source)
            : base(source)
        {
			_count = -1;
        }

        /// <summary>
        /// Initializes a new instance 
		/// of the <see cref="Radischevo.Wahha.Core.EnumerableLink{T}"/> class 
        /// using the specified collection.
        /// </summary>
        public EnumerableLink(IEnumerable<T> collection)
            : base(collection)
        {
        }

		/// <summary>
		/// Initializes a new instance 
		/// of the <see cref="Radischevo.Wahha.Core.EnumerableLink{T}"/> class 
		/// with serialized data.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> 
		/// that holds the serialized object data.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> 
		/// that contains contextual information about the source or destination.</param>
		protected EnumerableLink(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
        #endregion

        #region Instance Properties
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
        /// Returns an enumerator that iterates 
        /// through the collection.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
			return Value.GetEnumerator();
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

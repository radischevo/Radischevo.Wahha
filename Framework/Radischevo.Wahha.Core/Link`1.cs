using System;
using System.Runtime.Serialization;

namespace Radischevo.Wahha.Core
{
    /// <summary>
    /// Provide a delegated 
    /// lazy link to an object.
    /// </summary>
    /// <typeparam name="T">The type of linked object.</typeparam>
	[Serializable]
    public class Link<T> : LinkBase<T>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance 
		/// of the <see cref="Radischevo.Wahha.Core.Link{T}"/> class.
        /// </summary>
        public Link()
			: base()
        {
        }

        /// <summary>
        /// Initializes a new instance 
		/// of the <see cref="Radischevo.Wahha.Core.Link{T}"/> class.
        /// </summary>
        public Link(Func<T> source)
            : base(source)
        {
        }

        /// <summary>
        /// Initializes a new instance 
		/// of the <see cref="Radischevo.Wahha.Core.Link{T}"/> class.
        /// </summary>
        public Link(T value)
            : base(value)
        {
        }

		/// <summary>
		/// Initializes a new instance 
		/// of the <see cref="Radischevo.Wahha.Core.Link{T}"/> class 
		/// with serialized data.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> 
		/// that holds the serialized object data.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> 
		/// that contains contextual information about the source or destination.</param>
		protected Link(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
        #endregion
	}
}

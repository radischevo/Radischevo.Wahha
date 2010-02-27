using System;

namespace Radischevo.Wahha.Core
{
    /// <summary>
    /// Provide a delegated 
    /// lazy link to an object.
    /// </summary>
    /// <typeparam name="T">The type of linked object.</typeparam>
    public class Link<T> : ILink<T>
        where T: class
    {
        #region Instance Fields
		private readonly object _lock = new object();
        private bool _hasLoadedValue;
        private bool _hasAssignedValue;
        private Func<T> _source;
		private object _tag;
        private T _value;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance 
        /// of the <see cref="Radischevo.Wahha.Core.Link"/> class.
        /// </summary>
        public Link()
        {
			_source = () => default(T);
        }

        /// <summary>
        /// Initializes a new instance 
        /// of the <see cref="Radischevo.Wahha.Core.Link"/> class.
        /// </summary>
        public Link(Func<T> source)
            : this()
        {
            _source = source;
        }

        /// <summary>
        /// Initializes a new instance 
        /// of the <see cref="Radischevo.Wahha.Core.Link"/> class.
        /// </summary>
        public Link(T value)
            : this()
        {
            Value = value;
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets a value, indicating 
        /// whether a linked object 
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
        /// Gets or sets a delegate function, 
        /// which will be used to load the 
        /// linked object.
        /// </summary>
        public Func<T> Source
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
        /// Gets or sets a linked object 
        /// for this instance.
        /// </summary>
        public T Value
        {
            get
            {
                if (!HasValue)
                    Load();

                return _value;
            }
            set
            {
                _hasAssignedValue = true;
                _value = value;
            }
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Determines whether specified 
        /// <see cref="Radischevo.Wahha.Core.Link"/>s 
        /// are equal to each other.
        public static bool operator ==(Link<T> first, Link<T> second)
        {
            if (Object.ReferenceEquals(first, second))
                return true;

            if (Object.ReferenceEquals(second, null))
                return false;

            return first.Equals(second);
        }

        /// <summary>
        /// Determines whether specified 
        /// <see cref="Radischevo.Wahha.Core.Link"/>s 
        /// are not equal to each other.
        public static bool operator !=(Link<T> first, Link<T> second)
        {
            return !(first == second);
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Explicitly loads the linked object 
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

						_value = _source();
						_hasLoadedValue = true;
						_source = null;
					}
				}
            }
        }

        /// <summary>
        /// Determines whether the specified 
        /// <see cref="Radischevo.Wahha.Core.Link"/> 
        /// is equal to the current instance.
        /// </summary>
        /// <param name="other">A <see cref="Radischevo.Wahha.Core.Link"/> 
        /// to compare with the current instance.</param>
        protected virtual bool Equals(Link<T> other)
        {
            if (Object.ReferenceEquals(null, other))
                return false;

            if (Object.ReferenceEquals(_value, other._value))
                return true;

            return Object.ReferenceEquals(_source, other._source);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> 
        /// is equal to the current instance.
        /// </summary>
        /// <param name="obj">An <see cref="System.Object"/> 
        /// to compare with the current instance.</param>
        public override bool Equals(object obj)
        {
            return Equals(obj as Link<T>);
        }

        /// <summary>
        /// Serves as a hash function for a 
        /// <see cref="Radischevo.Wahha.Core.Link"/> type.
        /// </summary>
        public override int GetHashCode()
        {
			int baseCode = base.GetHashCode() << 4;

            int value = (_value == null) ? 
                baseCode : baseCode ^ _value.GetHashCode();

            if (_source != null)
                value ^= _source.GetHashCode();

            return value;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents 
        /// the current <see cref="Radischevo.Wahha.Core.Link"/>.
        /// </summary>
        public override string ToString()
        {
            return (_value == null) ?
                String.Format("Link`1[[{0}]]", typeof(T).FullName) :
                _value.ToString();
        }
        #endregion
    }
}

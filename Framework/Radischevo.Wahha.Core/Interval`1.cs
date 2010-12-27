using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Radischevo.Wahha.Core
{
	[Serializable]
    public class Interval<T>
    {
        #region Instance Fields
        private T _from;
        private T _to;
        #endregion

        #region Constructors
        public Interval()
        {
        }

        public Interval(T from)
            : this(from, from)
        {
        }

        public Interval(T from, T to)
            : this()
        {
            _from = from;
            _to = to;
        }

		protected Interval(SerializationInfo info, StreamingContext context)
        {
            _from = (T)info.GetValue("from", typeof(T));
			_to = (T)info.GetValue("to", typeof(T));
        }
        #endregion

        #region Instance Properties
        public T From
        {
            get
            {
                return _from;
            }
            set
            {
                _from = value;
            }
        }

        public T To
        {
            get
            {
                return _to;
            }
            set
            {
                _to = value;
            }
        }
        #endregion

        #region Static Methods
        public static bool operator ==(Interval<T> x, Interval<T> y)
        {
            if (Object.ReferenceEquals(x, y))
                return true;

            if (Object.ReferenceEquals(x, null))
                return false;

            return x.Equals(y);
        }

        public static bool operator !=(Interval<T> x, Interval<T> y)
        {
            return !(x == y);
        }
        #endregion

        #region Instance Methods
        public void Normalize()
        {
            Normalize(Comparer<T>.Default);
        }

		public bool IsEmpty()
		{
			return IsEmpty(Comparer<T>.Default);
		}

		public bool Contains(T value)
		{
			return Contains(value, Comparer<T>.Default);
		}

		public bool Contains(Interval<T> interval)
		{
			return Contains(interval, Comparer<T>.Default);
		}

		public bool IntersectWith(Interval<T> other)
		{
			return IntersectWith(other, Comparer<T>.Default);
		}

		public bool Equals(Interval<T> other)
		{
			return Equals(other, EqualityComparer<T>.Default);
		}

		public bool IsEmpty(IComparer<T> comparer)
		{
			Precondition.Require(comparer, () => Error.ArgumentNull("comparer"));
			return (comparer.Compare(_from, _to) == 0);
		}

		public virtual void Normalize(IComparer<T> comparer)
		{
			Precondition.Require(comparer, () => Error.ArgumentNull("comparer"));

			T left = _from;
			T right = _to;

			bool isRightOrder = (comparer.Compare(left, right) <= 0);

			_from = (isRightOrder) ? left : right;
			_to = (isRightOrder) ? right : left;
		}

		public virtual bool Contains(T value, IComparer<T> comparer)
		{
			Precondition.Require(comparer, () => Error.ArgumentNull("comparer"));

			return ((comparer.Compare(value, _from) >= 0) 
				 && (comparer.Compare(value, _to) <= 0));
		}

		public virtual bool Contains(Interval<T> interval, IComparer<T> comparer)
		{
			Precondition.Require(comparer, () => Error.ArgumentNull("comparer"));
			if (Object.ReferenceEquals(interval, null))
				return false;

			return (Contains(interval._from) && Contains(interval._to));
		}

		public virtual bool IntersectWith(Interval<T> other, IComparer<T> comparer)
		{
			Precondition.Require(comparer, () => Error.ArgumentNull("comparer"));
			if (Object.ReferenceEquals(other, null))
				return false;

			return (Contains(other._from) || Contains(other._to));
		}

		public virtual bool Equals(Interval<T> other, IEqualityComparer<T> comparer)
		{
			Precondition.Require(comparer, () => Error.ArgumentNull("comparer"));
			if (Object.ReferenceEquals(other, null))
				return false;

			return comparer.Equals(_from, other._from)
				&& comparer.Equals(_to, other._to);
		}

		protected virtual void GetObjectData(SerializationInfo info, 
			StreamingContext context)
        {
            info.AddValue("from", _from, typeof(T));
			info.AddValue("to", _to, typeof(T));
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Interval<T>);
        }

        public override int GetHashCode()
        {
            int code = GetType().FullName.GetHashCode();
            if (!Object.ReferenceEquals(_from, null))
                code ^= _from.GetHashCode();
            if (!Object.ReferenceEquals(_to, null))
                code ^= _to.GetHashCode();

            return code;
        }

        public override string ToString()
        {
            return String.Format("[{0} - {1}]", _from, _to);
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Radischevo.Wahha.Core
{
	[Serializable]
    public class Interval<T>
    {
        #region Nested Types
        private class DefaultComparer : IComparer<T>
        {
            #region Instance Methods
            public int Compare(T x, T y)
            {
                IComparable left = (x as IComparable);
                IComparable right = (y as IComparable);

                if (Object.ReferenceEquals(left, right))
                    return 0;

                if (Object.ReferenceEquals(null, left))
                    return -1;

                return left.CompareTo(right);
            }
            #endregion
        }
        #endregion

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
            Normalize(new DefaultComparer());
        }

        public void Normalize(IComparer<T> comparer)
        {
            T left = _from;
            T right = _to;

            bool isRightOrder = (comparer.Compare(left, right) <= 0);

            _from = (isRightOrder) ? left : right;
            _to = (isRightOrder) ? right : left;
        }

		protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("from", _from, typeof(T));
			info.AddValue("to", _to, typeof(T));
        }

        protected virtual bool Equals(Interval<T> other)
        {
            if (Object.ReferenceEquals(other, null))
                return false;

            return (Object.Equals(_from, other._from) 
                && Object.Equals(_to, other._to));
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
            return String.Format("{0} - {1}", _from, _to);
        }
        #endregion
    }
}

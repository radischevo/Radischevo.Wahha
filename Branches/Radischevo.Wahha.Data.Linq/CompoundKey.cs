using System;

namespace Jeltofiol.Wahha.Data.Linq
{
    public class CompoundKey : IEquatable<CompoundKey>
    {
        #region Instance Fields
        private object[] _values;
        private int _hashCode;
        #endregion

        #region Constructors
        public CompoundKey(params object[] values)
        {
            _values = values;
            for (int i = 0, n = values.Length; i < n; i++)
            {
                object value = values[i];
                if (value != null)
                    _hashCode ^= (value.GetHashCode() + i);
            }
        }
        #endregion

        #region Instance Methods
        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public bool Equals(CompoundKey other)
        {
            if (Object.ReferenceEquals(other, null))
                return false;

            if (other._values.Length != _values.Length)
                return false;

            for (int i = 0, n = other._values.Length; i < n; i++)
            {
                if (!Object.Equals(_values[i], other._values[i]))
                    return false;
            }
            return true;
        }
        #endregion
    }
}

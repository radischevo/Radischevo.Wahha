using System;
using System.Collections;

namespace Radischevo.Wahha.Core.Expressions
{
    internal class HashCodeCombiner
    {
        #region Instance Fields
        private long _combinedHash = 0x1505L;
        #endregion

        #region Constructors
        public HashCodeCombiner()
        {
        }
        #endregion

        #region Instance Properties
        public int CombinedHash
        {
            get
            {
                return _combinedHash.GetHashCode();
            }
        }
        #endregion

        #region Instance Methods
        public void AddFingerprint(ExpressionFingerprint fingerprint)
        {
            if (fingerprint != null)
                fingerprint.AddToHashCodeCombiner(this);
            else
                AddInt32(0);
        }

        public void AddEnumerable(IEnumerable e)
        {
            if (e == null)
                AddInt32(0);
            else
            {
                int count = 0;
                foreach (object o in e)
                {
                    AddObject(o);
                    count++;
                }
                AddInt32(count);
            }
        }

        public void AddInt32(int i)
        {
            _combinedHash = ((_combinedHash << 5) + _combinedHash) ^ i;
        }

        public void AddObject(object o)
        {
            AddInt32((o == null) ? 0 : o.GetHashCode());
        }
        #endregion
    }
}

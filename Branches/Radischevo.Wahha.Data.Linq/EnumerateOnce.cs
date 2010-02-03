using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Jeltofiol.Wahha.Data.Linq
{
    public class EnumerateOnce<T> : IEnumerable<T>, IEnumerable
    {
        #region Instance Fields
        private IEnumerable<T> _enumerable;
        #endregion

        #region Constructors
        public EnumerateOnce(IEnumerable<T> enumerable)
        {
            _enumerable = enumerable;
        }
        #endregion

        #region Instance Methods
        public IEnumerator<T> GetEnumerator()
        {
            IEnumerable<T> en = Interlocked.Exchange(ref _enumerable, null);
            if (en != null)
                return en.GetEnumerator();
            
            throw Error.CouldNotEnumerateMoreThanOnce();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}

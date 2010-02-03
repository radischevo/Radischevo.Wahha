using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Jeltofiol.Wahha.Data.Linq
{
    /// <summary>
    /// Simple implementation of the IGrouping<TKey, TElement> interface
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        #region Instance Fields
        private TKey _key;
        private IEnumerable<TElement> _group;
        #endregion

        #region Constructors
        public Grouping(TKey key, IEnumerable<TElement> group)
        {
            _key = key;
            _group = group;
        }
        #endregion

        #region Instance Properties
        public TKey Key
        {
            get
            {
                return _key;
            }
        }
        #endregion

        #region Instance Methods
        public IEnumerator<TElement> GetEnumerator()
        {
            return _group.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _group.GetEnumerator();
        }
        #endregion
    }   
}

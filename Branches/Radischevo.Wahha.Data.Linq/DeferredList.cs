using System;
using System.Collections;
using System.Collections.Generic;

namespace Jeltofiol.Wahha.Data.Linq
{
    /// <summary>
    /// A list implementation that is loaded the first the contents are examined.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    public class DeferredList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable, IDeferredLoadable
    {
        #region Instance Fields
        private IEnumerable<T> _source;
        private List<T> _values;
        #endregion

        #region Constructors
        public DeferredList(IEnumerable<T> source)
        {
            _source = source;
        }
        #endregion

        #region Instance Properties
        public bool IsLoaded
        {
            get 
            {
                return (_values != null);
            }
        }

        public T this[int index]
        {
            get
            {
                CheckLoadState();
                return _values[index];
            }
            set
            {
                CheckLoadState();
                _values[index] = value;
            }
        }

        public int Count
        {
            get
            {
                CheckLoadState();
                return _values.Count;
            }
        }

        public bool IsReadOnly
        {
            get 
            { 
                return false; 
            }
        }

        public bool IsFixedSize
        {
            get 
            { 
                return false; 
            }
        }
        #endregion

        #region Instance Methods
        private void CheckLoadState()
        {
            if (!IsLoaded)
                Load();            
        }

        public void Load()
        {
            _values = new List<T>(_source);
        }
        
        public int IndexOf(T item)
        {
            CheckLoadState();
            return _values.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            CheckLoadState();
            _values.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            CheckLoadState();
            _values.RemoveAt(index);
        }

        public void Add(T item)
        {
            CheckLoadState();
            _values.Add(item);
        }

        public void Clear()
        {
            CheckLoadState();
            _values.Clear();
        }

        public bool Contains(T item)
        {
            CheckLoadState();
            return _values.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            CheckLoadState();
            _values.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            CheckLoadState();
            return _values.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            CheckLoadState();
            return _values.GetEnumerator();
        }
        #endregion

        #region Interface Implementation
        object IList.this[int index]
        {
            get
            {
                CheckLoadState();
                return ((IList)_values)[index];
            }
            set
            {
                CheckLoadState();
                ((IList)_values)[index] = value;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return null;
            }
        }

        int IList.Add(object value)
        {
            CheckLoadState();
            return ((IList)_values).Add(value);
        }

        bool IList.Contains(object value)
        {
            CheckLoadState();
            return ((IList)_values).Contains(value);
        }

        int IList.IndexOf(object value)
        {
            CheckLoadState();
            return ((IList)_values).IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            CheckLoadState();
            ((IList)_values).Insert(index, value);
        }

        void IList.Remove(object value)
        {
            CheckLoadState();
            ((IList)_values).Remove(value);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            CheckLoadState();
            ((IList)_values).CopyTo(array, index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}

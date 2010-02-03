using System;

namespace Radischevo.Wahha.Web.Text.Sgml
{
    /// <summary>
    /// This stack maintains a high water mark for allocated objects so the client
    /// can reuse the objects in the stack to reduce memory allocations, this is
    /// used to maintain current state of the parser for element stack, and attributes
    /// in each element.
    /// </summary>
    internal class HWStack<T>
        where T : class
    {
        #region Instance Fields
        private int _count;
        private int _growth;
        private T[] _items;
        private int _size;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the HWStack class.
        /// </summary>
        /// <param name="growth">The amount to grow the stack 
        /// space by, if more space is needed on the stack.</param>
        public HWStack(int growth)
        {
            _growth = growth;
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Returns the item at the requested index 
        /// or null if index is out of bounds
        /// </summary>
        /// <param name="i">The index of the item to retrieve.</param>
        /// <returns>The item at the requested index or 
        /// null if index is out of bounds.</returns>
        public T this[int i]
        {
            get
            {
                if (i >= 0 && i < _size)
                    return _items[i];

                return null;
            }
            set
            {
                _items[i] = value;
            }
        }

        /// <summary>
        /// The number of items currently in the stack.
        /// </summary>
        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                _count = value;
            }
        }

        /// <summary>
        /// The size (capacity) of the stack.
        /// </summary>
        public int Size
        {
            get
            {
                return _size;
            }
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Removes and returns the item at the top of the stack
        /// </summary>
        /// <returns>The item at the top of the stack.</returns>
        public T Pop()
        {
            _count--;
            if (_count > 0)
                return _items[_count - 1];
            
            return null;
        }

        /// <summary>
        /// Pushes a new slot at the top of the stack.
        /// </summary>
        /// <returns>The object at the top of the stack.</returns>
        /// <remarks>
        /// This method tries to reuse a slot, if it returns null then
        /// the user has to call the other Push method.
        /// </remarks>
        public T Push()
        {
            if (_count == _size)
            {
                int num = _size + _growth;
                T[] destinationArray = new T[num];

                if (_items != null)
                    Array.Copy(_items, destinationArray, _size);
                
                _size = num;
                _items = destinationArray;
            }
            return _items[_count++];
        }

        /// <summary>
        /// Remove a specific item from the stack.
        /// </summary>
        /// <param name="i">The index of the item to remove.</param>
        public void RemoveAt(int i)
        {
            _items[i] = null;
            Array.Copy(_items, i + 1, _items, i, (_count - i) - 1);

            _count--;
        }
        #endregion
    }
}

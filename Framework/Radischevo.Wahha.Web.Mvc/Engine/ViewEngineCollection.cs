using System;
using System.Collections;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public sealed class ViewEngineCollection : ICollection<IViewEngine>
    {
        #region Instance Fields
        private List<IViewEngine> _list;
        #endregion

        #region Constructors
        public ViewEngineCollection()
        {
            _list = new List<IViewEngine>();
        }

        public ViewEngineCollection(IEnumerable<IViewEngine> collection)
        {
            _list = new List<IViewEngine>(collection);
        }
        #endregion

        #region Instance Properties
        public int Count
        {
            get 
            {
                return _list.Count;
            }
        }
        #endregion

        #region Instance Methods
        public void Add(IViewEngine item)
        {
            Precondition.Require(item, () => Error.ArgumentNull("item"));
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(IViewEngine item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(IViewEngine[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public ViewEngineResult FindView(ControllerContext context, string viewName)
        {
            foreach (IViewEngine engine in _list)
            {
                ViewEngineResult result = engine.FindView(context, viewName);
                if (result != null)
                    return result;
            }
            return null;
        }

        public bool Remove(IViewEngine item)
        {
            return _list.Remove(item);
        }

        public IEnumerator<IViewEngine> GetEnumerator()
        {
            return _list.GetEnumerator();
        }
        #endregion

        #region Interface Implementation
        bool ICollection<IViewEngine>.IsReadOnly
        {
            get
            {
                return false;
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}

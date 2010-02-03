using System;
using System.Collections;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Html
{
    public class SelectList : MultiSelectList
    {
        #region Instance Fields
        private object _selectedItem;
        #endregion

        #region Constructors
        public SelectList(IEnumerable<ListItem> items)
            : base(items)
        {
        }

        public SelectList(IEnumerable items)
            : this(items, null)
        {
        }

        public SelectList(IEnumerable items, object selectedItem)
            : this(items, null, null, selectedItem)
        {
        }

        public SelectList(IEnumerable items, 
            string dataTextField, string dataValueField)
            : this(items, dataTextField, dataValueField, null)
        {
        }

        public SelectList(IEnumerable items, string dataTextField, 
            string dataValueField, object selectedItem)
            : base(items, dataTextField, dataValueField, ToEnumerable(selectedItem))
        {
            _selectedItem = selectedItem;
        }
        #endregion

        #region Instance Properties
        public object SelectedItem
        {
            get
            {
                return _selectedItem;
            }
        }
        #endregion

        #region Static Methods
        private static IEnumerable ToEnumerable(object value)
        {
            if (value == null)
                return null;
            
            return new object[] { value };
        }
        #endregion
    }
}

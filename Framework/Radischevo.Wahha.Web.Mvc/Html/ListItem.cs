using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Html
{
    public sealed class ListItem
    {
        #region Instance Fields
        private string _value;
        private string _text;
        private bool _selected;
        private object _dataItem;
        #endregion

        #region Constructors
        public ListItem() 
            : this(null)
        {   }

        public ListItem(object dataItem)
        {
            _dataItem = dataItem;
        }
        #endregion

        #region Instance Properties
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }

        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
            }
        }

        public object DataItem
        {
            get
            {
                return _dataItem;
            }
            set
            {
                _dataItem = value;
            }
        }
        #endregion
    }
}

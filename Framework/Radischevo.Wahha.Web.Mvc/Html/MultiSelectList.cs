using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Html
{
    public class MultiSelectList : IEnumerable<ListItem>
    {
        #region Instance Fields
        private string _dataTextField;
        private string _dataValueField;
        private IEnumerable _items;
        private IEnumerable _selectedItems;
        private IEnumerable<ListItem> _listItems;
        #endregion

        #region Constructors
        public MultiSelectList(IEnumerable<ListItem> items)
        {
            Precondition.Require(items, () => Error.ArgumentNull("items"));

            _listItems = items;
        }

        public MultiSelectList(IEnumerable items)
            : this(items, null)
        {
        }

        public MultiSelectList(IEnumerable items, IEnumerable selectedItems)
            : this(items, null, null, selectedItems)
        {
        }

        public MultiSelectList(IEnumerable items, 
            string dataTextField, string dataValueField)
            : this(items, dataTextField, dataValueField, null)
        {
        }

        public MultiSelectList(IEnumerable items, string dataTextField, 
            string dataValueField, IEnumerable selectedItems)
        {
            Precondition.Require(items, () => Error.ArgumentNull("items"));
            
            _items = items;
            _dataTextField = dataTextField;
            _dataValueField = dataValueField;
            _selectedItems = selectedItems;
        }
        #endregion

        #region Instance Properties
        public string DataTextField
        {
            get
            {
                return _dataTextField;
            }
            set
            {
                _dataTextField = value;
            }
        }

        public string DataValueField
        {
            get
            {
                return _dataValueField;
            }
            set
            {
                _dataValueField = value;
            }
        }

        public IEnumerable Items
        {
            get
            {
                return _items;
            }
        }

        public IEnumerable SelectedItems
        {
            get
            {
                return _selectedItems;
            }
        }
        #endregion

        #region Static Methods
        private static string Eval(object container, string expression)
        {
            if (!String.IsNullOrEmpty(expression))
                container = ObjectExtensions.Evaluate(container, expression);
            
            return Convert.ToString(container, CultureInfo.CurrentCulture);
        }
        #endregion

        #region Instance Methods
        public IEnumerator<ListItem> GetEnumerator()
        {
            return GetListItems().GetEnumerator();
        }

        public virtual IEnumerable<ListItem> GetListItems()
        {
            if (_listItems == null)
            {
                if (String.IsNullOrEmpty(_dataValueField))
                    _listItems = GetItemsWithoutValueField();

                _listItems = GetItemsWithValueField();
            }
            return _listItems;
        }

        private List<ListItem> GetItemsWithValueField()
        {
            HashSet<string> selectedValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (_selectedItems != null)
                selectedValues.UnionWith(_selectedItems.Cast<object>()
                    .Select(value => Convert.ToString(value, CultureInfo.CurrentCulture)));
            
            return _items.Cast<object>().Select(item => new { Item = item, Value = Eval(item, _dataValueField) })
                .Select(item => { 
                    return new ListItem { 
                        DataItem = item.Item,
                        Value = item.Value, 
                        Text = Eval(item.Item, _dataTextField), 
                        Selected = selectedValues.Contains(item.Value) 
                    };
                }).ToList();
        }

        private List<ListItem> GetItemsWithoutValueField()
        {
            HashSet<string> selectedValues = new HashSet<string>();
            
            if (_selectedItems != null)
                selectedValues.UnionWith(_selectedItems.Cast<object>()
                    .Select(value => Convert.ToString(value, CultureInfo.CurrentCulture)));
            
            return _items.Cast<object>().Select(item => {
                    string text = (String.IsNullOrEmpty(_dataTextField)) 
                        ? Convert.ToString(item, CultureInfo.CurrentCulture) 
                        : Eval(item, _dataTextField);

                    return new ListItem {
                        DataItem = item,
                        Text = text, Value = text,
                        Selected = selectedValues.Contains(text) 
                    };
                }).ToList();
        }
        #endregion

        #region IEnumerable Members
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}

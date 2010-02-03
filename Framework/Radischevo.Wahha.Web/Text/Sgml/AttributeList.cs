using System;
using System.Collections;
using System.Collections.Generic;

namespace Radischevo.Wahha.Web.Text.Sgml
{
    public class AttributeList : IEnumerable<AttributeDefinition>
    {
        #region Instance Fields
        private Dictionary<string, AttributeDefinition> _values;
        #endregion

        #region Constructors
        public AttributeList()
        {
            _values = new Dictionary<string, AttributeDefinition>(
                StringComparer.OrdinalIgnoreCase);
        }
        #endregion

        #region Instance Properties
        public AttributeDefinition this[string name]
        {
            get
            {
                AttributeDefinition attr;
                return (_values.TryGetValue(name, out attr)) ? attr : null;
            }
        }
        #endregion

        #region Instance Methods
        public void Add(AttributeDefinition attr)
        {
            _values[attr.Name] = attr;
        }

        public IEnumerator<AttributeDefinition> GetEnumerator()
        {
            return _values.Values.GetEnumerator();
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

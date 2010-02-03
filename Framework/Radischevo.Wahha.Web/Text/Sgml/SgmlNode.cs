using System;
using System.Xml;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Text.Sgml
{
    /// <summary>
    /// This class models an XML node, an array of elements in scope is maintained while parsing
    /// for validation purposes, and these Node objects are reused to reduce object allocation,
    /// hence the reset method.  
    /// </summary>
    internal class SgmlNode
    {
        #region Instance Fields
        private HWStack<Attribute> _attributes;
        private State _state;
        private ElementDeclaration _dtdType;
        private bool _isEmpty;
        private string _name;
        private XmlNodeType _nodeType;
        private bool _simulated;
        private XmlSpace _space;
        private string _value;
        private string _xmlLang;
        #endregion

        #region Constructors
        public SgmlNode()
        {
            _attributes = new HWStack<Attribute>(10);
        }
        #endregion

        #region Instance Properties
        public int AttributeCount
        {
            get
            {
                return _attributes.Count;
            }
        }

        internal State State
        {
            get 
            { 
                return _state; 
            }
            set 
            { 
                _state = value; 
            }
        }

        public ElementDeclaration DtdType
        {
            get 
            { 
                return _dtdType; 
            }
            set 
            { 
                _dtdType = value; 
            }
        }

        public bool IsEmpty
        {
            get 
            { 
                return _isEmpty; 
            }
            set 
            { 
                _isEmpty = value; 
            }
        }

        public string Name
        {
            get 
            { 
                return _name; 
            }
            set 
            { 
                _name = value; 
            }
        }

        public XmlNodeType NodeType
        {
            get 
            { 
                return _nodeType; 
            }
            set 
            { 
                _nodeType = value; 
            }
        }

        public bool Simulated
        {
            get 
            { 
                return _simulated; 
            }
            set 
            { 
                _simulated = value; 
            }
        }

        public XmlSpace Space
        {
            get 
            { 
                return _space; 
            }
            set 
            { 
                _space = value; 
            }
        }

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

        public string XmlLang
        {
            get 
            { 
                return _xmlLang; 
            }
            set 
            { 
                _xmlLang = value; 
            }
        }
        #endregion

        #region Instance Methods
        public Attribute AddAttribute(string name, string value, 
            char quoteChar, bool caseInsensitive)
        {
            Attribute attribute;
            int index = 0;
            int count = _attributes.Count;

            while (index < count)
            {
                attribute = _attributes[index];
                if (String.Compare(attribute.Name, name, caseInsensitive) == 0)
                    return null;
                
                index++;
            }

            attribute = _attributes.Push();
            if (attribute == null)
            {
                attribute = new Attribute();
                _attributes[_attributes.Count - 1] = attribute;
            }

            attribute.Reset(name, value, quoteChar);
            return attribute;
        }

        public void CopyAttributes(SgmlNode node)
        {
            Precondition.Require(node, Error.ArgumentNull("node"));

            int index = 0;
            int count = node._attributes.Count;

            while (index < count)
            {
                Attribute attribute = node._attributes[index];
                AddAttribute(attribute.Name, attribute.Value, 
                    attribute.QuoteChar, false).DtdType = attribute.DtdType;

                index++;
            }
        }

        public Attribute GetAttribute(int i)
        {
            if (i >= 0 && i < _attributes.Count)
                return _attributes[i];
            
            return null;
        }

        public Attribute GetAttribute(string name)
        {
            int index = 0;
            int count = _attributes.Count;

            while (index < count)
            {
                Attribute attribute = _attributes[index];
                if (attribute.Name == name)
                    return attribute;
                
                index++;
            }
            return null;
        }

        public int GetAttributeIndex(string name)
        {
            int index = 0;
            int count = _attributes.Count;

            while (index < count)
            {
                Attribute attribute = _attributes[index];
                if (attribute.Name == name)
                    return index;

                index++;
            }
            return -1;
        }

        public void RemoveAttribute(string name)
        {
            int index = 0;
            int count = _attributes.Count;

            while (index < count)
            {
                Attribute attribute = _attributes[index];
                if (attribute.Name == name)
                {
                    _attributes.RemoveAt(index);
                    return;
                }
                index++;
            }
        }

        /// <summary>
        /// Attribute objects are reused during parsing to reduce memory allocations, 
        /// hence the Reset method. 
        /// </summary>
        public void Reset(string name, XmlNodeType nt, string value)
        {
            _value = value;
            _name = name;
            _nodeType = nt;
            _space = XmlSpace.None;
            _xmlLang = null;
            _isEmpty = true;
            _attributes.Count = 0;
            _dtdType = null;
        }
        #endregion
    }
}

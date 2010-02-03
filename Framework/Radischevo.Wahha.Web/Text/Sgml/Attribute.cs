using System;

namespace Radischevo.Wahha.Web.Text.Sgml
{
    /// <summary>
    /// This class represents an attribute. The AttributeDefinition is assigned
    /// from a validation process, and is used to provide default values.
    /// </summary>
    internal class Attribute
    {
        #region Instance Fields
        private AttributeDefinition _dtdType;
        private string _literalValue;
        private string _name;
        private char _quoteChar;
        #endregion

        #region Constructors
        public Attribute()
        {   }
        #endregion

        #region Instance Properties
        public bool IsDefault
        {
            get
            {
                return (_literalValue == null);
            }
        }

        /// <summary>
        /// The value of the attribute.
        /// </summary>
        public string Value
        {
            get
            {
                if (_literalValue != null)
                    return _literalValue;
                
                if (_dtdType != null)
                    return _dtdType.Default;
                
                return null;
            }
        }

        /// <summary>
        /// The literal value of the attribute.
        /// </summary>
        public string LiteralValue
        {
            get
            {
                return _literalValue;
            }
            set
            {
                _literalValue = value;
            }
        }

        /// <summary>
        /// The AttributeDefinition of the attribute 
        /// from the SGML DTD.
        /// </summary>
        public AttributeDefinition DtdType
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

        /// <summary>
        /// The atomized name.
        /// </summary>
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

        /// <summary>
        /// The quote character used 
        /// for the attribute value.
        /// </summary>
        public char QuoteChar
        {
            get
            {
                return _quoteChar;
            }
            set
            {
                _quoteChar = value;
            }
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Attribute objects are reused during parsing 
        /// to reduce memory allocations, 
        /// hence the Reset method.
        /// </summary>
        public void Reset(string name, string value, char quoteChar)
        {
            _name = name;
            _literalValue = value;
            _quoteChar = quoteChar;
            _dtdType = null;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Radischevo.Wahha.Web.Text.Sgml
{
    /// <summary>
    /// An attribute definition in a DTD.
    /// </summary>
    public class AttributeDefinition
    {
        #region Instance Fields
        private string _default;
        private string[] _enumValues;
        private string _name;
        private AttributePresence _presence;
        private AttributeType _type;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AttDef"/> class.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        public AttributeDefinition(string name)
        {
            _name = name;
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// The name of the attribute declared by 
        /// this attribute definition.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Gets of sets the default value of the attribute.
        /// </summary>
        public string Default
        {
            get
            {
                return _default;
            }
            set
            {
                _default = value;
            }
        }

        /// <summary>
        /// The constraints on the attribute's presence on an element.
        /// </summary>
        public AttributePresence Presence
        {
            get
            {
                return _presence;
            }
        }

        /// <summary>
        /// Gets the possible enumerated values for the attribute.
        /// </summary>
        public IEnumerable<string> EnumValues
        {
            get
            {
                return _enumValues;
            }
        }

        /// <summary>
        /// The <see cref="AttributeType"/> of the 
        /// attribute declaration.
        /// </summary>
        public AttributeType Type
        {
            get
            {
                return _type;
            }
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Sets the attribute definition to have an enumerated value.
        /// </summary>
        /// <param name="enumValues">The possible values in the enumeration.</param>
        /// <param name="type">The type to set the attribute to.</param>
        /// <exception cref="ArgumentException">If the type parameter is not either 
        /// <see cref="AttributeType.Enumeration"/> or <see cref="AttributeType.Notation"/>.</exception>
        public void SetEnumeratedType(string[] enumValues, AttributeType type)
        {
            if (type != AttributeType.Enumeration && type != AttributeType.Notation)
                throw Error.InvalidAttributeDefinitionType(type);

            _enumValues = enumValues;
            _type = type;
        }

        /// <summary>
        /// Sets the attribute presence declaration.
        /// </summary>
        /// <param name="token">The string representation of the attribute presence, 
        /// corresponding to one of the values in the <see cref="AttributePresence"/> enumeration.</param>
        /// <returns>true if the attribute presence implies the 
        /// element has a default value.</returns>
        public bool SetPresence(string token)
        {
            if (String.Equals(token, "FIXED", 
                StringComparison.InvariantCultureIgnoreCase))
            {
                _presence = AttributePresence.Fixed;
                return true;
            }
            else if (String.Equals(token, "REQUIRED", 
                StringComparison.InvariantCultureIgnoreCase))
            {
                _presence = AttributePresence.Required;
                return false;
            }
            else if (String.Equals(token, "IMPLIED", 
                StringComparison.InvariantCultureIgnoreCase))
            {
                _presence = AttributePresence.Implied;
                return false;
            }
            
            throw Error.InvalidAttributeDefinitionPresenceValue(token);
        }

        /// <summary>
        /// Sets the type of the attribute definition.
        /// </summary>
        /// <param name="type">The string representation of the attribute type, 
        /// corresponding to the values in the <see cref="AttributeType"/> enumeration.</param>
        public void SetType(string type)
        {
            switch (type)
            {
                case "CDATA":
                    _type = AttributeType.CData;
                    break;
                case "ENTITY":
                    _type = AttributeType.Entity;
                    break;
                case "ENTITIES":
                    _type = AttributeType.Entities;
                    break;
                case "ID":
                    _type = AttributeType.Id;
                    break;
                case "IDREF":
                    _type = AttributeType.IdRef;
                    break;
                case "IDREFS":
                    _type = AttributeType.IdRefs;
                    break;
                case "NAME":
                    _type = AttributeType.Name;
                    break;
                case "NAMES":
                    _type = AttributeType.Names;
                    break;
                case "NMTOKEN":
                    _type = AttributeType.NameToken;
                    break;
                case "NMTOKENS":
                    _type = AttributeType.NameTokens;
                    break;
                case "NUMBER":
                    _type = AttributeType.Number;
                    break;
                case "NUMBERS":
                    _type = AttributeType.Numbers;
                    break;
                case "NUTOKEN":
                    _type = AttributeType.NumberToken;
                    break;
                case "NUTOKENS":
                    _type = AttributeType.NumberTokens;
                    break;
                default:
                    throw Error.InvalidAttributeDefinitionTypeValue(type);
            }
        }
        #endregion
    }
}

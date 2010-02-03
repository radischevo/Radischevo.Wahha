using System;

namespace Radischevo.Wahha.Web.Text.Sgml
{
    /// <summary>
    /// An element declaration in a DTD.
    /// </summary>
    public class ElementDeclaration
    {
        #region Instance Fields
        private AttributeList _attributeList;
        private ContentModel _contentModel;
        private bool _startTagOptional;
        private bool _endTagOptional;
        private string[] _exclusions;
        private string[] _inclusions;
        private string _name;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new element declaration instance.
        /// </summary>
        /// <param name="name">The name of the element.</param>
        /// <param name="startTagOptional">Whether the start tag is optional.</param>
        /// <param name="endTagOptiona">Whether the end tag is optional.</param>
        /// <param name="model">The <see cref="ContentModel"/> of the element.</param>
        /// <param name="inclusions"></param>
        /// <param name="exclusions"></param>
        public ElementDeclaration(string name, bool startTagOptional, 
            bool endTagOptional, ContentModel model, 
            string[] inclusions, string[] exclusions)
        {
            _name = name;
            _startTagOptional = startTagOptional;
            _endTagOptional = endTagOptional;
            _contentModel = model;
            _inclusions = inclusions;
            _exclusions = exclusions;
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// The element name.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Whether the end tag of the element is optional.
        /// </summary>
        /// <value>true if the end tag of the element 
        /// is optional, otherwise false.</value>
        public bool EndTagOptional
        {
            get
            {
                return _endTagOptional;
            }
        }

        /// <summary>
        /// Whether the start tag of the element is optional.
        /// </summary>
        /// <value>true if the start tag of the element is 
        /// optional, otherwise false.</value>
        public bool StartTagOptional
        {
            get
            {
                return _startTagOptional;
            }
        }

        public AttributeList AttributeList
        {
            get
            {
                return _attributeList;
            }
        }

        /// <summary>
        /// The <see cref="ContentModel"/> of the element declaration.
        /// </summary>
        public ContentModel ContentModel
        {
            get
            {
                return _contentModel;
            }
        }
        #endregion

        #region Instance Methods
        public void AddAttributeDefinitions(AttributeList list)
        {
            if (_attributeList == null)
            {
                _attributeList = list;
            }
            else
            {
                foreach (AttributeDefinition def in list)
                {
                    if (_attributeList[def.Name] == null)
                        _attributeList.Add(def);
                }
            }
        }

        public bool CanContain(string name, SgmlDtd dtd)
        {
            if (_exclusions != null)
            {
                foreach (string excl in _exclusions)
                {
                    if (excl == name)
                        return false;
                }
            }
            if (_inclusions != null)
            {
                foreach (string incl in _inclusions)
                {
                    if (incl == name)
                        return true;
                }
            }
            return _contentModel.CanContain(name, dtd);
        }

        public AttributeDefinition FindAttribute(string name)
        {
            return _attributeList[name.ToUpper()];
        }
        #endregion
    }
}

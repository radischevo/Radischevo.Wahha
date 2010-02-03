using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using System.Xml;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Text
{
    public enum HtmlElementRenderMode 
    {
        Normal = 0,
        StartTag,
        EndTag
    }

    public sealed class HtmlElementBuilder
    {
        #region Static Fields
        private const string _elementFormatEndTag = "</{0}>";
        private const string _elementFormatSelfClosing = "<{0}{1} />";
        private const string _elementFormatNormal = "<{0}{1}>{2}</{0}>";
        private const string _elementFormatStartTag = "<{0}{1}>";
        #endregion

        #region Instance Fields
        private string _name;
        private string _content;
        private HtmlAttributeDictionary _attributes;
        #endregion

        #region Constructors
        public HtmlElementBuilder(string tagName) 
            : this(tagName, null, null)
        {   
        }

        public HtmlElementBuilder(string tagName, object attributes) 
            : this(tagName, attributes, null)
        {
        }

        public HtmlElementBuilder(string tagName, 
            object attributes, string content)
        {
            Precondition.Require(!String.IsNullOrEmpty(tagName), Error.ArgumentNull("tagName"));
            _name = tagName;
            _attributes = new HtmlAttributeDictionary(attributes);
            _content = content;
        }
        #endregion

        #region Instance Properties
        public string Name
        {
            get
            {
                return _name;
            }
        }

        public HtmlAttributeDictionary Attributes
        {
            get
            {
                return _attributes;
            }
        }

        public string InnerHtml
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
            }
        }

        public string InnerText
        {
            get
            {
                return HttpUtility.HtmlEncode(_content);
            }
            set
            {
                _content = HttpUtility.HtmlEncode(value);
            }
        }
        #endregion

        #region Instance Methods
        public void AddClassName(string cssClassName)
        {
            object currentValue;

            if (_attributes.TryGetValue("class", out currentValue))
                _attributes["class"] = String.Concat(currentValue, " ", cssClassName);
            else
                _attributes["class"] = cssClassName;
        }

        public override string ToString()
        {
            return ToString(HtmlElementRenderMode.Normal);
        }

        public string ToString(HtmlElementRenderMode renderMode)
        {
            switch (renderMode)
            {
                case HtmlElementRenderMode.StartTag:
                    return String.Format(CultureInfo.InvariantCulture, 
                        _elementFormatStartTag, _name, _attributes.ToString());

                case HtmlElementRenderMode.EndTag:
                    return String.Format(CultureInfo.InvariantCulture, 
                        _elementFormatEndTag, _name);

                default:
                    if (_content == null)
                        return String.Format(CultureInfo.InvariantCulture, 
                            _elementFormatSelfClosing, _name, _attributes.ToString());

                    return String.Format(CultureInfo.InvariantCulture, 
                        _elementFormatNormal, _name, _attributes.ToString(), _content);
            }
        }
        #endregion
    }
}

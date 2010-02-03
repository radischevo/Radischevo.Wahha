using System;
using System.Xml;

namespace Radischevo.Wahha.Web.Text
{
    public delegate XmlElement HtmlElementConverter(XmlElement element);
    
    public delegate XmlAttribute HtmlAttributeConverter(XmlAttribute attribute);

    public delegate string HtmlElementFormatter(HtmlElementBuilder element, HtmlElementRenderMode renderMode);
}

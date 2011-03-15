using System;
using System.Xml;

namespace Radischevo.Wahha.Web.Text
{
    public delegate XmlElement HtmlElementConverter(HtmlElementContext context, XmlElement element);
    
    public delegate XmlAttribute HtmlAttributeConverter(HtmlAttributeContext context, XmlAttribute attribute);

    public delegate string HtmlElementFormatter(HtmlElementBuilder element, HtmlElementRenderMode renderMode);
}

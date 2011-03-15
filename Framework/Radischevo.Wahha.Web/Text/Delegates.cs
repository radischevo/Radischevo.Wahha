using System;
using System.Xml;

namespace Radischevo.Wahha.Web.Text
{
    public delegate HtmlConverterResult<XmlElement> HtmlElementConverter(HtmlElementContext context);
    
    public delegate HtmlConverterResult<XmlAttribute> HtmlAttributeConverter(HtmlAttributeContext context);

    public delegate string HtmlElementFormatter(HtmlElementBuilder element, HtmlElementRenderMode renderMode);
}

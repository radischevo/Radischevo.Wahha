using System;

namespace Radischevo.Wahha.Web.Text.Sgml
{
    internal enum State : byte
    {
        Initial,
        Markup,
        EndTag,
        Attr,
        AttrValue,
        Text,
        PartialTag,
        AutoClose,
        CData,
        PartialText,
        PseudoStartTag,
        Eof
    }
}

using System;

namespace Radischevo.Wahha.Web.Text
{
    [Flags]
    public enum HtmlAttributeFlags : byte
    {
        Denied = 0,
        Allowed = 1,
        Default = 0x02,
        Required = 0x06
    }
}

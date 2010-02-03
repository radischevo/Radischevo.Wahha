using System;

namespace Radischevo.Wahha.Web.Text
{
    [Flags]
    public enum HtmlElementFlags : byte
    {
        Denied = 0,
        Allowed = 1,          // 0000001
        Recursive = 0x02,     // 0000010
        SelfClosing = 0x05,   // 0000101
        Container = 0x08,     // 0001000
        Text = 0x10,          // 0010000
        UseTypography = 0x30, // 0110000
        Preformatted = 0x50,  // 1010000
        AllowContent = 0x18   // 0011000
    }
}

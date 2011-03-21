using System;

namespace Radischevo.Wahha.Web.Text
{
    [Flags]
    public enum HtmlElementOptions : byte
    {
        Denied = 0,
        Allowed = 1,
        Global = 2,
        SelfClosing = 5,
        Container = 8,
        Text = 16,
		AllowContent = 24,
        UseTypography = 48,
        Preformatted = 80,
		Generated = 128
    }
}

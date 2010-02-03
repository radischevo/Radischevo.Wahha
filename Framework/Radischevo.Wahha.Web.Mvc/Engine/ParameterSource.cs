using System;

namespace Radischevo.Wahha.Web.Mvc
{
    public enum ParameterSource : byte
    {
        QueryString = 0x01,
        Form = 0x02,
        Url = 0x04,
        Cookie = 0x08,
        Header = 0x10,
        Session = 0x20,
        InputStream = 0x40,
        Default = 0x07, 
        Any = 0x3f
    }
}

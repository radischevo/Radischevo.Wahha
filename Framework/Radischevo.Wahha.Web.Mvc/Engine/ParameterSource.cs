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
		Custom = 0x40,
		InputStream = 0x80,
        Default = 0x47, 
        Any = 0x7f
    }
}

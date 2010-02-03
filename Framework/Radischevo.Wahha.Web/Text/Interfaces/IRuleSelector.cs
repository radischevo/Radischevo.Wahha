using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Text
{
    public interface IRuleSelector : IHideObjectMembers
    {
        IFluentElementRule Element(string name);

        IFluentAttributeRule Attribute(string name);

        IFluentElementRule Elements(params string[] names);

        IFluentAttributeRule Attributes(params string[] names);
    }
}

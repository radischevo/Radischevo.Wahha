using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Text
{
    public interface IFluentElementRule : IRuleAppender
    {
        IFluentElementRule As(HtmlElementFlags flags);

        IFluentElementRule Convert(HtmlElementConverter converter);
    }
}

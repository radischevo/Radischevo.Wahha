using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Text
{
    public interface IFluentElementRule : IRuleAppender
    {
        IFluentElementRule As(HtmlElementOptions options);

        IFluentElementRule Convert(HtmlElementConverter converter);
    }
}

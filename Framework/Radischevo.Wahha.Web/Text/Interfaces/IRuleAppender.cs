using System;

namespace Radischevo.Wahha.Web.Text
{
    public interface IRuleAppender : IRuleBuilder
    {
        IRuleAppender Treat(Func<IRuleSelector, IRuleBuilder> inner);
    }
}

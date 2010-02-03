using System;

namespace Radischevo.Wahha.Web.Text
{
    public interface IRuleAppender : IRuleBuilder
    {
        IRuleAppender With(Func<IRuleSelector, IRuleBuilder> inner);
    }
}

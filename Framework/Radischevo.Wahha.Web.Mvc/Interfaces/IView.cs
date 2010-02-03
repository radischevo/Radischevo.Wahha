using System;
using System.IO;

namespace Radischevo.Wahha.Web.Mvc
{
    public interface IView
    {
        void Render(ViewContext context, TextWriter writer);
    }
}

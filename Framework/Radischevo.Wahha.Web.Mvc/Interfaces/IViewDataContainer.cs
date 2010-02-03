using System;

namespace Radischevo.Wahha.Web.Mvc
{
    public interface IViewDataContainer
    {
        ViewDataDictionary ViewData
        {
            get;
            set;
        }
    }
}

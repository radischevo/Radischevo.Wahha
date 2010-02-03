using System;

namespace Radischevo.Wahha.Web.Mvc
{
    public interface IViewLocationCache
    {
        string GetVirtualPath(string cacheKey);
        void SetVirtualPath(string cacheKey, string virtualPath);
    }
}

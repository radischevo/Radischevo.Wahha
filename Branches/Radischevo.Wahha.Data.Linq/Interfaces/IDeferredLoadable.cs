using System;

namespace Jeltofiol.Wahha.Data.Linq
{
    /// <summary>
    /// Common interface for controlling defer-loadable types
    /// </summary>
    public interface IDeferredLoadable
    {
        bool IsLoaded { get; }
        void Load();
    }
}

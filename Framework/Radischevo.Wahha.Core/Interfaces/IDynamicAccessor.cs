using System;
using System.Reflection;

namespace Radischevo.Wahha.Core
{
    public interface IDynamicAccessor
    {
        object GetValue(object instance);
        void SetValue(object instance, object value);
    }
}

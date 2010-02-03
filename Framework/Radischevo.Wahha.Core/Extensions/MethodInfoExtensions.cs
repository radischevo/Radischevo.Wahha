using System;
using System.Reflection;

namespace Radischevo.Wahha.Core
{
    public static class MethodInfoExtensions
    {
        #region Static Extension Methods
        public static bool ArgumentsMatch(this MethodInfo method, 
            Type[] typeArguments, object[] arguments)
        {
            if ((!method.IsGenericMethodDefinition && method.IsGenericMethod)
                && method.ContainsGenericParameters)
                method = method.GetGenericMethodDefinition();

            if (method.IsGenericMethodDefinition)
            {
                if (typeArguments == null || typeArguments.Length == 0)
                    return false;

                if (method.GetGenericArguments().Length != typeArguments.Length)
                    return false;

                method = method.MakeGenericMethod(typeArguments);
            }
            else if (typeArguments != null && typeArguments.Length > 0)
                return false;

            return method.ArgumentsMatch(arguments);
        }
        #endregion
    }
}

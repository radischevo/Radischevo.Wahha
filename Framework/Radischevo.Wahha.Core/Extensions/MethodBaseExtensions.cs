using System;
using System.Reflection;

namespace Radischevo.Wahha.Core
{
    public static class MethodBaseExtensions
    {
        #region Static Extension Methods
        public static bool ArgumentsMatch(this MethodBase method, object[] arguments)
        {
            ParameterInfo[] parameters = method.GetParameters();
            if (arguments == null || parameters.Length != arguments.Length)
                return false;

            if (method.ContainsGenericParameters || method.IsGenericMethodDefinition)
                throw Error.MethodCouldNotBeGenericDefinition(method);

            for (int i = 0; i < arguments.Length; ++i)
            {
                Type type = parameters[i].ParameterType;
                if (type == null)
                    return false;

                object arg = arguments[i];
                if (arg == null)
                {
                    if (!type.IsNullable())
                        return false;
                }
                else
                {
                    Type argumentType = arg.GetType();
                    if (!type.IsAssignableFrom(argumentType))
                        return false;
                }
            }
            return true;
        }

        public static IMethodInvoker CreateInvoker(this MethodInfo method)
        {
            return new MethodInvoker(method);
        }

        public static IConstructorInvoker CreateInvoker(this ConstructorInfo constructor)
        {
            return new ConstructorInvoker(constructor);
        }
        #endregion
    }
}

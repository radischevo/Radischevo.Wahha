using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public sealed class MethodDispatcher
    {
        #region Nested Types
        private delegate object MethodExecutor(object instance, object[] parameters);
        private delegate void VoidMethodExecutor(object instance, object[] parameters);
        #endregion

        #region Instance Fields
        private MethodExecutor _methodExecutor;
        private MethodInfo _method;
        #endregion

        #region Constructors
        internal MethodDispatcher(MethodInfo method)
        {
            Precondition.Require(method, Error.ArgumentNull("method"));

            _methodExecutor = CreateExecutorDelegate(method);
            _method = method;
        }
        #endregion

        #region Instance Properties
        public MethodInfo Method
        {
            get
            {
                return _method;
            }
        }
        #endregion

        #region Static Methods
        private static MethodExecutor CreateVoidMethodWrapper(VoidMethodExecutor executor)
        {
            return delegate(object target, object[] parameters)
            {
                executor(target, parameters);
                return null;
            };
        }

        private static MethodExecutor CreateExecutorDelegate(MethodInfo method)
        {
            // Parameters to executor
            ParameterExpression targetParameter = Expression.Parameter(typeof(object), "target");
            ParameterExpression parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

            // Build parameter list
            List<Expression> parameters = new List<Expression>();
            ParameterInfo[] methodParams = method.GetParameters();
            
            for (int i = 0; i < methodParams.Length; i++)
            {
                ParameterInfo paramInfo = methodParams[i];
                BinaryExpression valueObj = Expression.ArrayIndex(parametersParameter, Expression.Constant(i));
                UnaryExpression valueCast = Expression.Convert(valueObj, paramInfo.ParameterType);

                parameters.Add(valueCast);
            }

            // Call method
            UnaryExpression targetCast = (method.IsStatic) ? null : Expression.Convert(targetParameter, method.ReflectedType);
            MethodCallExpression methodCall = Expression.Call(targetCast, method, parameters);

            if (methodCall.Type == typeof(void))
            {
                Expression<VoidMethodExecutor> lambda = 
                    Expression.Lambda<VoidMethodExecutor>(methodCall, targetParameter, parametersParameter);

                return CreateVoidMethodWrapper(lambda.Compile());
            }
            else
            {
                UnaryExpression castMethodCall = Expression.Convert(methodCall, typeof(object));
                Expression<MethodExecutor> lambda = 
                    Expression.Lambda<MethodExecutor>(castMethodCall, targetParameter, parametersParameter);

                return lambda.Compile();
            }
        }
        #endregion

        #region Instance Methods
        public object Execute(object target, object[] parameters)
        {
            return _methodExecutor(target, parameters);
        }
        #endregion
    }
}

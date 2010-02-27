using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Radischevo.Wahha.Core
{
    public interface IMethodInvoker
    {
        object Invoke(object instance, params object[] parameters);
    }

    public class MethodInvoker : IMethodInvoker
    {
        #region Instance Fields
        private Func<object, object[], object> _invoker;
        private MethodInfo _method;
        #endregion

        #region Constructors
        public MethodInvoker(MethodInfo method)
        {
			Precondition.Require(method, () => Error.ArgumentNull("method"));

            _method = method;
            _invoker = CreateInvoker(method);
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
        private static Func<object, object[], object> CreateInvoker(MethodInfo method)
        {
            ParameterExpression instanceParameter = Expression.Parameter(typeof(object), "instance");
            ParameterExpression parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

            List<Expression> parameterExpressions = new List<Expression>();
            ParameterInfo[] paramInfos = method.GetParameters();

            for (int i = 0; i < paramInfos.Length; i++)
            {
                BinaryExpression valueObj = Expression.ArrayIndex(
                    parametersParameter, Expression.Constant(i));
                UnaryExpression valueCast = Expression.Convert(
                    valueObj, paramInfos[i].ParameterType);

                parameterExpressions.Add(valueCast);
            }

            UnaryExpression instanceCast = (method.IsStatic) ? null :
                Expression.Convert(instanceParameter, method.ReflectedType);

            MethodCallExpression methodCall = Expression.Call(instanceCast, method, parameterExpressions);
            if (methodCall.Type == typeof(void))
            {
                Expression<Action<object, object[]>> lambda = 
                    Expression.Lambda<Action<object, object[]>>(
                        methodCall, instanceParameter, parametersParameter);

                Action<object, object[]> execute = lambda.Compile();
                return (instance, parameters) => {
                    execute(instance, parameters);
                    return null;
                };
            }
            else
            {
                UnaryExpression castMethodCall = Expression.Convert(methodCall, typeof(object));
                Expression<Func<object, object[], object>> lambda = 
                    Expression.Lambda<Func<object, object[], object>>(
                        castMethodCall, instanceParameter, parametersParameter);

                return lambda.Compile();
            }
        }
        #endregion

        #region Instance Methods
        public object Invoke(object instance, params object[] parameters)
        {
            return _invoker(instance, parameters);
        }

        public V Invoke<T, V>(T instance, params object[] parameters)
        {
            return (V)_invoker(instance, parameters);
        }
        #endregion
    }
}

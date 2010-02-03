using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Radischevo.Wahha.Core
{
    public interface IConstructorInvoker
    {
        object Invoke(params object[] parameters);
    }

    public class ConstructorInvoker : IConstructorInvoker
    {
        #region Instance Fields
        private Func<object[], object> _invoker;
        private ConstructorInfo _constructor;
        #endregion

        #region Constructors
        public ConstructorInvoker(ConstructorInfo constructor)
        {
            Precondition.Require(constructor, Error.ArgumentNull("constructor"));

            _constructor = constructor;
            _invoker = CreateInvoker(constructor);
        }
        #endregion

        #region Instance Properties
        public ConstructorInfo Constructor
        {
            get
            {
                return _constructor;
            }
        }
        #endregion

        #region Instance Methods
        private static Func<object[], object> CreateInvoker(ConstructorInfo constructor)
        {
            ParameterExpression parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

            List<Expression> parameterExpressions = new List<Expression>();
            ParameterInfo[] parameters = constructor.GetParameters();

            for (int i = 0; i < parameters.Length; i++)
            {
                BinaryExpression valueObj = Expression.ArrayIndex(parametersParameter, Expression.Constant(i));
                UnaryExpression valueCast = Expression.Convert(valueObj, parameters[i].ParameterType);

                parameterExpressions.Add(valueCast);
            }

            NewExpression createInstance = Expression.New(constructor, parameterExpressions);
            UnaryExpression createInstanceCast = Expression.Convert(createInstance, typeof(object));

            Expression<Func<object[], object>> lambda = 
                Expression.Lambda<Func<object[], object>>(createInstanceCast, parametersParameter);

            return lambda.Compile();
        }

        public object Invoke(params object[] parameters)
        {
            return _invoker(parameters);
        }
        #endregion
    }
}

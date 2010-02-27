using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Scripting.Serialization;

namespace Radischevo.Wahha.Web.Mvc
{
    public class JsonModelBinder : DefaultModelBinder
    {
        #region Nested Types
        private delegate object JavaScriptDeserializationExecutor(string value);
        #endregion

        #region Instance Methods
        public override object Bind(BindingContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            object value;
            context.TryGetValue(out value);

            return context.Model = Deserialize(context.ModelType, 
                Convert.ToString(value, CultureInfo.InvariantCulture));
        }

        private object Deserialize(Type type, string serializedString)
        {
            if (String.IsNullOrEmpty(serializedString))
                return null;

            try
            {
                ParameterExpression stringParameter = Expression.Parameter(typeof(string), "input");
                NewExpression newSerializer = Expression.New(typeof(JavaScriptSerializer));

                MethodInfo method = typeof(JavaScriptSerializer).GetMethod("Deserialize", 
                    BindingFlags.Instance | BindingFlags.Public, null, 
                    new Type[] { typeof(string) }, null).MakeGenericMethod(type);

                MethodCallExpression methodCall = Expression.Call(newSerializer, method, stringParameter);
                UnaryExpression castMethodCall = Expression.Convert(methodCall, typeof(object));

                Expression<JavaScriptDeserializationExecutor> lambda =
                    Expression.Lambda<JavaScriptDeserializationExecutor>(castMethodCall, stringParameter);

                JavaScriptDeserializationExecutor executor = lambda.Compile();
                return executor(serializedString);
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}

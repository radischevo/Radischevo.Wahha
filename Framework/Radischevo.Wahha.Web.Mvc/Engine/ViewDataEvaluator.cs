using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    internal sealed class ViewDataEvaluator
    {
        #region Nested Types
        private struct ExpressionPair
        {
            public readonly string Left;
            public readonly string Right;

            public ExpressionPair(string left, string right)
            {
                Left = left;
                Right = right;
            }
        }
        #endregion

        #region Instance Fields
        private ViewDataDictionary _dictionary;
        #endregion

        #region Constructors
        public ViewDataEvaluator(ViewDataDictionary dictionary)
        {
            _dictionary = dictionary;
        }
        #endregion

        #region Static Methods
        private static ViewDataInfo EvalComplexExpression(object instance, string expression)
        {
            foreach (ExpressionPair pair in GetRightToLeftExpressions(expression))
            {
                string subExpression = pair.Left;
                string postExpression = pair.Right;

                ViewDataInfo subTarget = GetPropertyValue(instance, subExpression);
                if (subTarget != null)
                {
                    if (String.IsNullOrEmpty(postExpression))
                        return subTarget;

                    if (subTarget.Value != null)
                    {
                        ViewDataInfo potential = 
                            EvalComplexExpression(subTarget.Value, postExpression);
                        if (potential != null)
                            return potential;
                    }
                }
            }
            return null;
        }

        private static IEnumerable<ExpressionPair> GetRightToLeftExpressions(string expression)
        {
            yield return new ExpressionPair(expression, String.Empty);

            int lastDot = expression.LastIndexOf('.');

            string subExpression = expression;
            string postExpression = string.Empty;

            while (lastDot > -1)
            {
                subExpression = expression.Substring(0, lastDot);
                postExpression = expression.Substring(lastDot + 1);
                yield return new ExpressionPair(subExpression, postExpression);

                lastDot = subExpression.LastIndexOf('.');
            }
        }

        private static ViewDataInfo GetPropertyValue(object container, string propertyName)
        {
            ViewDataInfo value = GetIndexedPropertyValue(container, propertyName);
            if (value != null)
                return value;

            ViewDataDictionary vdd = (container as ViewDataDictionary);
            if (vdd != null)
                container = vdd.Model;

            if (container == null)
                return null;

            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(container)
                .Find(propertyName, true);

            if (descriptor == null)
                return null;

            return new ViewDataInfo() {
                Container = container,
                Descriptor = descriptor,
                Value = descriptor.GetValue(container)
            };
        }

        private static ViewDataInfo GetIndexedPropertyValue(object instance, string key)
        {
            Type type = instance.GetType();

            // call into IDictionary implementations first
            IDictionary dict = (instance as IDictionary);
            if (dict != null)
            {
                object value;
                if (dict.TryGetValue(key, out value))
                {
                    return new ViewDataInfo() {
                        Container = instance,
                        Value = value
                    };
                }
            }

            MethodInfo containsKeyMethod = type.GetMethod("ContainsKey", 
                BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(string) }, null);

            if (containsKeyMethod != null)
                if (!(bool)containsKeyMethod.Invoke(instance, new object[] { key }))
                    return null;

            PropertyInfo info = type.GetProperty("Item", BindingFlags.Public | BindingFlags.Instance, 
                null, null, new Type[] { typeof(string) }, null);

            if (info != null)
                return new ViewDataInfo() {
                    Container = instance,
                    Value = info.GetValue(instance, new object[] { key })
                };

            PropertyInfo objectInfo = type.GetProperty("Item", BindingFlags.Public | BindingFlags.Instance, 
                null, null, new Type[] { typeof(object) }, null);

            if (objectInfo != null)
                return new ViewDataInfo() {
                    Container = instance,
                    Value = objectInfo.GetValue(instance, new object[] { key })
                };
            
            return null;
        }
        #endregion

        #region Instance Methods
        public ViewDataInfo Eval(string expression)
        {
            ViewDataInfo evaluated = EvalComplexExpression(
                _dictionary, expression);

            return evaluated;
        }
        #endregion
    }
}

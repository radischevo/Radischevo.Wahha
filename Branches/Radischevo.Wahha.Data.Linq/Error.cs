using System;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq
{
    internal static class Error
    {
        internal static Exception ArgumentNull(string name)
        {
            return new ArgumentNullException(name);
        }

        internal static Exception CouldNotEnumerateMoreThanOnce()
        {
            return new InvalidOperationException("The collection cannot be enumerated more than once.");
        }

        internal static Exception CycleInTopologicalSort()
        {
            return new InvalidOperationException("Cycle in topological sort.");
        }

        internal static Exception ExpressionTypeMustImplementIQueryable(string parameterName)
        {
            return new InvalidOperationException(String.Format(
                "The provided '{0}' parameter must implement the IQueryable interface.", parameterName));
        }

        internal static Exception CouldNotFindQueryProvider()
        {
            return new InvalidOperationException("Could not find a query provider.");
        }

        internal static Exception UnknownExpressionType(string parameterName, Expression expression)
        {
            return new ArgumentException(String.Format(
                "The '{0}' parameter has an unknown expression type '{1}'.", 
                parameterName, expression.NodeType));
        }

        internal static Exception InvalidBindingType(string parameterName, MemberBinding binding)
        {
            return new Exception(String.Format("The provided member binding '{0}' has an invalid type '{1}'.", 
                parameterName, binding.BindingType));
        }
    }
}

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public static class DbExpressionTypeExtensions
    {
        public static bool IsDbExpression(this ExpressionType type)
        {
            return (((int)type) >= 1024);
        }
    }
}

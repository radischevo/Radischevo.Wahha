using System;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public abstract class DbStatementExpression : DbExpression
    {
        protected DbStatementExpression(DbExpressionType expressionType, Type type)
            : base(expressionType, type)
        {
        }
    }
}

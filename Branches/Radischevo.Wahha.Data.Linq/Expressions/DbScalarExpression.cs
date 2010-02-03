using System;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public class DbScalarExpression : DbSubqueryExpression
    {
        public DbScalarExpression(Type type, DbSelectExpression select)
            : base(DbExpressionType.Scalar, type, select)
        {
        }
    }
}

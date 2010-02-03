using System;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public class DbExistsExpression : DbSubqueryExpression
    {
        public DbExistsExpression(DbSelectExpression select)
            : base(DbExpressionType.Exists, typeof(bool), select)
        {
        }
    }
}

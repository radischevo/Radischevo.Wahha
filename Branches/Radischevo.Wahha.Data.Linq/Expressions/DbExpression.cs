using System;
using System.Linq;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public abstract class DbExpression : Expression
    {
        #region Constructors
        protected DbExpression(DbExpressionType expressionType, Type type)
            : base((ExpressionType)expressionType, type)
        {
        }
        #endregion

        #region Instance Methods
        public override string ToString()
        {
            return "";
            //return DbExpressionWriter.WriteToString(this);
        }
        #endregion
    }
}

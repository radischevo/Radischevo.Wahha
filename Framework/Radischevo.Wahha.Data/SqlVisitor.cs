using System;

using Jeltofiol.Wahha.Data.Query.Expressions;

namespace Jeltofiol.Wahha.Data.Query
{
    public abstract class SqlVisitor
    {
        #region Instance Fields
        protected int _depth;
        #endregion

        #region Constructors
        protected SqlVisitor()
        {   }
        #endregion

        #region Instance Methods
        public virtual SqlExpression Visit(SqlExpression node)
        {
            if (node == null)
                return null;

            try
            {
                _depth++;
                switch (node.NodeType)
                {
                    case SqlExpressionType.Aggregate:
                        return VisitAggregate((SqlAggregate)node);
                    case SqlExpressionType.Alias:
                        return VisitAlias((SqlAlias)node);
                    case SqlExpressionType.Between:
                        return VisitBetween((SqlBetween)node);
                    case SqlExpressionType.Binary:
                        return VisitBinary((SqlBinary)node);
                    case SqlExpressionType.Case:
                        return VisitCase((SqlCase)node);
                    case SqlExpressionType.Constant:
                        return VisitConstant((SqlConstant)node);
                    case SqlExpressionType.Delete:
                        return VisitDelete((SqlDelete)node);
                    case SqlExpressionType.DoNotVisit:
                        return VisitDoNotVisit((SqlDoNotVisitExpression)node);
                    case SqlExpressionType.Exists:
                        return VisitExists((SqlExists)node);
                    case SqlExpressionType.ExprSet:
                        return VisitExprSet((SqlExpressionSet)node);
                    case SqlExpressionType.Field:
                        return VisitField((SqlField)node);
                    case SqlExpressionType.Function:
                        return VisitFunction((SqlFunctionCall)node);
                    case SqlExpressionType.Grouping:
                        return VisitGrouping((SqlGrouping)node);
                    case SqlExpressionType.Insert:
                        return VisitInsert((SqlInsert)node);
                    case SqlExpressionType.Join:
                        return VisitJoin((SqlJoin)node);
                    case SqlExpressionType.Limit:
                        return VisitLimit((SqlLimit)node);
                    case SqlExpressionType.OrderBy:
                        return VisitOrderBy((SqlOrderBy)node);
                    case SqlExpressionType.Parameter:
                        return VisitParameter((SqlParameter)node);
                    case SqlExpressionType.Raw:
                        return VisitRaw((SqlRawExpression)node);
                    case SqlExpressionType.Select:
                        return VisitSelect((SqlSelect)node);
                    case SqlExpressionType.Table:
                        return VisitTable((SqlTable)node);
                    case SqlExpressionType.TableValuedFunction:
                        return VisitTableValuedFunction((SqlTableValuedFunctionCall)node);
                    case SqlExpressionType.Type:
                        return VisitType((SqlType)node);
                    case SqlExpressionType.Unary:
                        return VisitUnary((SqlUnary)node);
                    case SqlExpressionType.Union:
                        return VisitUnion((SqlUnion)node);
                    case SqlExpressionType.Update:
                        return VisitUpdate((SqlUpdate)node);
                    case SqlExpressionType.Variable:
                        return VisitVariable((SqlVariable)node);
                    case SqlExpressionType.When:
                        return VisitWhen((SqlWhen)node);
                }
            }
            finally
            {
                _depth--;
            }
            return null;
        }

        protected virtual SqlExpression VisitWhen(SqlWhen expr)
        {
            return expr;
        }

        protected virtual SqlExpression VisitVariable(SqlVariable expr)
        {
            return expr;
        }

        protected virtual SqlExpression VisitUpdate(SqlUpdate expr)
        {
            return expr;
        }

        protected virtual SqlExpression VisitUnion(SqlUnion expr)
        {
            return expr;
        }

        protected virtual SqlExpression VisitUnary(SqlUnary expr)
        {
            return expr;
        }

        protected virtual SqlExpression VisitTableValuedFunction(
            SqlTableValuedFunctionCall expr)
        {
            return expr;
        }

        protected virtual SqlExpression VisitType(SqlType expr)
        {
            return expr;
        }

        protected virtual SqlExpression VisitTable(SqlTable expr)
        {
            return expr;
        }

        protected virtual SqlExpression VisitSelect(SqlSelect expr)
        {
            return expr;
        }

        protected virtual SqlExpression VisitRaw(SqlRawExpression expr)
        {
            return expr;
        }

        protected virtual SqlExpression VisitParameter(SqlParameter expr)
        {
            return expr;
        }

        protected virtual SqlExpression VisitOrderBy(SqlOrderBy expr)
        {
            return expr;
        }

        protected virtual SqlExpression VisitLimit(SqlLimit expr)
        {
            return expr;
        }

        protected virtual SqlExpression VisitJoin(SqlJoin expr)
        {
            return expr;
        }

        protected virtual SqlExpression VisitInsert(SqlInsert expr)
        {
            return expr;
        }

        protected virtual SqlExpression VisitGrouping(SqlGrouping expr)
        {
            return expr;
        }

        protected virtual SqlExpression VisitFunction(SqlFunctionCall expr)
        {
            return expr;
        }

        protected virtual SqlExpression VisitField(SqlField expr)
        {
            return expr;
        }

        protected virtual SqlExpression VisitExprSet(SqlExpressionSet expr)
        {
            return expr;
        }

        protected virtual SqlExpression VisitExists(SqlExists expr)
        {
            return expr;
        }

        protected virtual SqlExpression VisitDoNotVisit(SqlDoNotVisitExpression expr)
        {
            return expr;
        }

        protected virtual SqlExpression VisitDelete(SqlDelete expr)
        {
            return expr;
        }

        protected virtual SqlExpression VisitConstant(SqlConstant expr)
        {
            return expr;
        }

        protected virtual SqlExpression VisitCase(SqlCase expr)
        {
            return expr;
        }

        protected virtual SqlExpression VisitBinary(SqlBinary expr)
        {
            return expr;
        }

        protected virtual SqlExpression VisitBetween(SqlBetween expr)
        {
            return expr;
        }

        protected virtual SqlExpression VisitAlias(SqlAlias expr)
        {
            return expr;
        }

        protected virtual SqlExpression VisitAggregate(SqlAggregate expr)
        {
            return expr;
        }
        #endregion
    }
}

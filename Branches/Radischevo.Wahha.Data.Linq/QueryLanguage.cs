using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Jeltofiol.Wahha.Core;
using Jeltofiol.Wahha.Data.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq
{
    /// <summary>
    /// Defines the language rules for the query provider
    /// </summary>
    public abstract class QueryLanguage
    {
        public abstract DbDataTypeParser TypeSystem { get; }
        public abstract Expression GetGeneratedIdExpression(MemberInfo member);

        public virtual string Quote(string name)
        {
            return name;
        }

        public virtual bool AllowsMultipleCommands
        {
            get { return false; }
        }

        public virtual bool AllowSubqueryInSelectWithoutFrom
        {
            get { return false; }
        }

        public virtual bool AllowDistinctInAggregates
        {
            get { return false; }
        }

        public virtual Expression GetRowsAffectedExpression(Expression command)
        {
            return new DbFunctionExpression(typeof(int), "@@ROWCOUNT", null);
        }

        /// <summary>
        /// Determines whether the CLR type corresponds to a scalar data type in the query language
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool IsScalar(Type type)
        {
            type = type.MakeNonNullableType();
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Empty:
                case TypeCode.DBNull:
                    return false;
                case TypeCode.Object:
                    return
                        type == typeof(DateTimeOffset) ||
                        type == typeof(TimeSpan) ||
                        type == typeof(Guid) ||
                        type == typeof(byte[]);
                default:
                    return true;
            }
        }

        /// <summary>
        /// Determines whether the given expression can be represented as a column in a select expressionss
        /// </summary>
        public virtual bool CanBeColumn(Expression expression)
        {
            switch (expression.NodeType)
            {
                case (ExpressionType)DbExpressionType.Column:
                case (ExpressionType)DbExpressionType.Scalar:
                case (ExpressionType)DbExpressionType.Exists:
                case (ExpressionType)DbExpressionType.AggregateSubquery:
                case (ExpressionType)DbExpressionType.Aggregate:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Provides language specific query translation.  Use this to apply language specific rewrites or
        /// to make assertions/validations about the query.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual Expression Translate(Expression expression)
        {
            // remove redundant layers again before cross apply rewrite
            expression = UnusedColumnRemover.Remove(expression);
            expression = RedundantColumnRemover.Remove(expression);
            expression = RedundantSubqueryRemover.Remove(expression);

            // convert cross-apply and outer-apply joins into inner & left-outer-joins if possible
            expression = CrossApplyRewriter.Rewrite(expression);
            // convert cross joins into inner joins
            expression = CrossJoinRewriter.Rewrite(expression);

            // do final reduction
            expression = UnusedColumnRemover.Remove(expression);
            expression = RedundantSubqueryRemover.Remove(expression);
            expression = RedundantJoinRemover.Remove(expression);
            expression = RedundantColumnRemover.Remove(expression);

            return expression;
        }

        /// <summary>
        /// Converts the query expression into text of this query language
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual string Format(Expression expression)
        {
            // use common SQL formatter by default
            return SqlFormatter.Format(expression);
        }

        /// <summary>
        /// Determine which sub-expressions must be parameters
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual Expression Parameterize(Expression expression)
        {
            return Parameterizer.Parameterize(this, expression);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Jeltofiol.Wahha.Data.Linq.Mapping;
using Jeltofiol.Wahha.Data.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq
{
    /// <summary>
    /// Defines query execution & materialization policies. 
    /// </summary>
    public class QueryPolicy
    {
        public QueryPolicy()
        {
        }

        /// <summary>
        /// Determines if a relationship property is to be included in the results of the query
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public virtual bool IsIncluded(MemberInfo member)
        {
            return false;
        }

        /// <summary>
        /// Determines if a relationship property is included, but the query for the related data is 
        /// deferred until the property is first accessed.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public virtual bool IsDeferLoaded(MemberInfo member)
        {
            return false;
        }

        /// <summary>
        /// Provides policy specific query translations.  This is where choices about inclusion of related objects and how
        /// heirarchies are materialized affect the definition of the queries.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual Expression Translate(MetaMapping mapping, Expression expression)
        {
            // add included relationships to client projection
            expression = RelationshipIncluder.Include(mapping, this, expression);

            expression = UnusedColumnRemover.Remove(expression);
            expression = RedundantColumnRemover.Remove(expression);
            expression = RedundantSubqueryRemover.Remove(expression);
            expression = RedundantJoinRemover.Remove(expression);

            // convert any singleton (1:1 or n:1) projections into server-side joins (cardinality is preserved)
            expression = SingletonProjectionRewriter.Rewrite(mapping.Language, expression);

            expression = UnusedColumnRemover.Remove(expression);
            expression = RedundantColumnRemover.Remove(expression);
            expression = RedundantSubqueryRemover.Remove(expression);
            expression = RedundantJoinRemover.Remove(expression);

            // convert projections into client-side joins
            expression = ClientJoinedProjectionRewriter.Rewrite(mapping.Language, expression);

            expression = UnusedColumnRemover.Remove(expression);
            expression = RedundantColumnRemover.Remove(expression);
            expression = RedundantSubqueryRemover.Remove(expression);
            expression = RedundantJoinRemover.Remove(expression);

            return expression;
        }

        /// <summary>
        /// Converts a query into an execution plan.  The plan is an function that executes the query and builds the
        /// resulting objects.
        /// </summary>
        /// <param name="projection"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public virtual Expression BuildExecutionPlan(MetaMapping mapping, Expression query, Expression provider)
        {
            return ExecutionBuilder.Build(mapping, this, query, provider);
        }

        public static readonly QueryPolicy Default = new QueryPolicy();
    }
}

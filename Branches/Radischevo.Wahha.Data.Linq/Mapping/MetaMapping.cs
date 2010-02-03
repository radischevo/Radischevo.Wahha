using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Jeltofiol.Wahha.Core;
using Jeltofiol.Wahha.Data.Linq.Expressions;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq.Mapping
{
    /// <summary>
    /// Defines mapping information & rules for the query provider
    /// </summary>
    public abstract class MetaMapping
    {
        internal QueryLanguage Language;

        /// <summary>
        /// Get the meta entity directly 
        /// corresponding to the CLR type
        /// </summary>
        /// <param name="type"></param>
        public virtual MetaType GetMetaType(Type type)
        {
            return GetMetaType(type, type.Name, type);
        }

        /// <summary>
        /// Get the meta entity that maps between 
        /// the CLR type and the database table 
        /// </summary>
        public virtual MetaType GetMetaType(Type type, string mappingId)
        {
            return GetMetaType(type, mappingId, type);
        }

        /// <summary>
        /// Get the meta entity that maps between the CLR type 'entityType' 
        /// and the database table, yet is represented publicly as 'elementType'.
        /// </summary>
        public abstract MetaType GetMetaType(Type elementType, string mappingId, Type entityType);

        /// <summary>
        /// Get the meta entity represented by the IQueryable context member
        /// </summary>
        /// <param name="contextMember"></param>
        /// <returns></returns>
        public abstract MetaType GetMetaType(MemberInfo member);

        /// <summary>
        /// Determines if a property is mapped as a relationship
        /// </summary>
        public abstract bool IsAssociation(MetaType type, MemberInfo member);

        /// <summary>
        /// Determines if a relationship property refers to a single entity (as opposed to a collection.)
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public virtual bool IsSingletonAssociation(MetaType type, MemberInfo member)
        {
            if (!this.IsAssociation(type, member))
                return false;

            return (TypeExtensions.FindEnumerable(TypeHelper.GetMemberType(member)) == null);
        }

        /// <summary>
        /// Get a query expression that selects all entities from a table
        /// </summary>
        /// <param name="rowType"></param>
        /// <returns></returns>
        public abstract DbProjectionExpression CreateProjection(MetaType type);

        /// <summary>
        /// Gets an expression that constructs an entity instance relative to a root.
        /// The root is most often a TableExpression, but may be any other experssion such as
        /// a ConstantExpression.
        /// </summary>
        public abstract DbEntityExpression CreateEntityExpression(Expression root, MetaType type);

        /// <summary>
        /// Get an expression for a mapped property relative to a root expression. 
        /// The root is either a TableExpression or an expression defining an entity instance.
        /// </summary>
        public abstract Expression CreateMemberExpression(Expression root, MetaType type, MemberInfo member);

        /// <summary>
        /// Get an expression that represents the insert operation for the specified instance.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="instance">The instance to insert.</param>
        /// <param name="selector">A lambda expression that computes a return value from the operation.</param>
        /// <returns></returns>
        public abstract Expression CreateInsertExpression(MetaType type, 
            Expression instance, LambdaExpression selector);

        /// <summary>
        /// Get an expression that represents the update operation for the specified instance.
        /// </summary>
        public abstract Expression CreateUpdateExpression(MetaType type, Expression instance, 
            LambdaExpression updateCheck, LambdaExpression selector, Expression @else);

        /// <summary>
        /// Get an expression that represents the delete 
        /// operation for the specified instance.
        /// </summary>
        public abstract Expression CreateDeleteExpression(MetaType type, 
            Expression instance, LambdaExpression deleteCheck);

        /// <summary>
        /// Recreate the type projection with the 
        /// additional members included
        /// </summary>
        public abstract DbEntityExpression IncludeMembers(DbEntityExpression entity, Func<MemberInfo, bool> isIncluded);

        /// <summary>
        /// Apply mapping translations to the expression
        /// </summary>
        public virtual Expression Translate(Expression expression)
        {
            // convert references to LINQ operators into query specific nodes
            expression = QueryBinder.Bind(this, expression);

            // move aggregate computations so they occur in same select as group-by
            expression = AggregateRewriter.Rewrite(expression);

            // do reduction so duplicate association's are likely to be clumped together
            expression = UnusedColumnRemover.Remove(expression);
            expression = RedundantColumnRemover.Remove(expression);
            expression = RedundantSubqueryRemover.Remove(expression);
            expression = RedundantJoinRemover.Remove(expression);

            // convert references to association properties into correlated queries
            expression = RelationshipBinder.Bind(this, expression);

            // clean up after ourselves! (multiple references to same association property)
            expression = RedundantColumnRemover.Remove(expression);
            expression = RedundantJoinRemover.Remove(expression);

            return expression;
        }
    }
}

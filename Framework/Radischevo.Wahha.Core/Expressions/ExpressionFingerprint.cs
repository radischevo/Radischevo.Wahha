using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace Radischevo.Wahha.Core.Expressions
{
    /// <summary>
    /// Contains information used for generalizing, 
    /// comparing, and recreating Expression instances.
    /// </summary>
    /// <remarks>Since Expression objects are immutable and are 
    /// recreated for every invocation of an expression helper method, 
    /// they can't be compared directly. Fingerprinting Expression objects 
    /// allows information about them to be abstracted away, and the 
    /// fingerprints can be directly compared. Consider the process of 
    /// fingerprinting that all values (parameters, constants, etc.) are hoisted
    /// and replaced with dummies. What remains can be decomposed into a sequence 
    /// of operations on specific types and specific inputs.</remarks>
    internal abstract class ExpressionFingerprint
    {
        #region Instance Fields
        private ExpressionType _nodeType;
        private Type _type;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Radischevo.Wahha.Core.Expressions.ExpressionFingerprint"/> class.
        /// </summary>
        /// <param name="expression">An expression to create fingerprint for.</param>
        protected ExpressionFingerprint(Expression expression)
        {
            _nodeType = expression.NodeType;
            _type = expression.Type;
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets the type of the 
        /// expression node.
        /// </summary>
        public ExpressionType NodeType
        {
            get
            {
                return _nodeType;
            }
        }

        /// <summary>
        /// Gets the static type of the 
        /// expression.
        /// </summary>
        public Type Type
        {
            get
            {
                return _type;
            }
        }
        #endregion

        #region Static Methods
        public static ExpressionFingerprint Create(Expression expression, ParserContext context)
        {
            BinaryExpression binaryExpression = (expression as BinaryExpression);
            if (binaryExpression != null)
                return BinaryExpressionFingerprint.Create(binaryExpression, context);
        
            ConditionalExpression conditionalExpression = (expression as ConditionalExpression);
            if (conditionalExpression != null)
                return ConditionalExpressionFingerprint.Create(conditionalExpression, context);
       
            ConstantExpression constantExpression = (expression as ConstantExpression);
            if (constantExpression != null)
                return ConstantExpressionFingerprint.Create(constantExpression, context);
        
            MemberExpression memberExpression = (expression as MemberExpression);
            if (memberExpression != null)
                return MemberExpressionFingerprint.Create(memberExpression, context);
        
            MethodCallExpression methodCallExpression = (expression as MethodCallExpression);
            if (methodCallExpression != null)
                return MethodCallExpressionFingerprint.Create(methodCallExpression, context);
        
            ParameterExpression parameterExpression = (expression as ParameterExpression);
            if (parameterExpression != null)
                return ParameterExpressionFingerprint.Create(parameterExpression, context);
        
            UnaryExpression unaryExpression = (expression as UnaryExpression);
            if (unaryExpression != null)
                return UnaryExpressionFingerprint.Create(unaryExpression, context);
            
            return null;
        }

        public static ReadOnlyCollection<ExpressionFingerprint> Create(
            IEnumerable<Expression> expressions, ParserContext context)
        {
            List<ExpressionFingerprint> fingerprints = new List<ExpressionFingerprint>();
            foreach (Expression expression in expressions)
            {
                ExpressionFingerprint fingerprint = Create(expression, context);
                if (fingerprint == null && expression != null)
                    return null;
                
                fingerprints.Add(fingerprint);
            }
            return new ReadOnlyCollection<ExpressionFingerprint>(fingerprints);
        }

        protected static Expression ToExpression(ExpressionFingerprint fingerprint, ParserContext context)
        {
            return (fingerprint == null) ? null : fingerprint.ToExpression(context);
        }

        protected static IEnumerable<Expression> ToExpression(
            IEnumerable<ExpressionFingerprint> fingerprints, ParserContext context)
        {
            return fingerprints.Select(f => ToExpression(f, context));
        }
        #endregion

        #region Instance Methods
        public abstract Expression ToExpression(ParserContext context);

        public virtual void AddToHashCodeCombiner(HashCodeCombiner combiner)
        {
            combiner.AddObject(NodeType);
            combiner.AddObject(Type);
        }

        public override int GetHashCode()
        {
            HashCodeCombiner combiner = new HashCodeCombiner();
            combiner.AddObject(GetType());
            AddToHashCodeCombiner(combiner);

            return combiner.CombinedHash;
        }

        public override bool Equals(object obj)
        {
            ExpressionFingerprint other = (obj as ExpressionFingerprint);
            if (other == null)
                return false;

            return (_nodeType == other.NodeType && _type == other.Type 
                && GetType() == other.GetType());
        }
        #endregion
    }
}

using System;
using System.Linq.Expressions;

namespace Radischevo.Wahha.Core.Expressions
{
    public static class CachedExpressionCompiler
    {
        #region Nested Types
        private static class Processor<TClass, TMember>
        {
            #region Nested Types
            private sealed class Cache : ReaderWriterCache<ExpressionFingerprint, 
                CompiledExpressionDelegate<TClass, TMember>>
            {
                #region Static Methods
                private static CompiledExpressionDelegate<TClass, TMember> CreateDelegate(ParserContext context)
                {
                    Expression body = context.Fingerprint.ToExpression(context);
                    return Expression.Lambda<CompiledExpressionDelegate<TClass, TMember>>(body,
                        context.Instance, ParserContext.HoistedValuesParameter).Compile();
                }
                #endregion

                #region Instance Methods
                public CompiledExpressionDelegate<TClass, TMember> GetDelegate(ParserContext context)
                {
                    return GetOrCreate(context.Fingerprint, () => CreateDelegate(context));
                }
                #endregion
            }
            #endregion

            #region Static Fields
            private static readonly Cache _cache = new Cache();
            #endregion

            #region Static Methods
            public static Func<TClass, TMember> GetFunction(
                Expression<Func<TClass, TMember>> expression)
            {
                // look for common patterns that don't need to be fingerprinted
                Func<TClass, TMember> func = GetFunctionFastTrack(expression);
                if (func != null)
                    return func;

                // not a common pattern, so try 
                // fingerprinting (slower, but cached)
                func = GetFunctionFingerprinted(expression);
                if (func != null)
                    return func;

                // pattern not recognized by fingerprinting 
                // routine, so compile directly (slowest)
                return GetFunctionSlow(expression);
            }

            private static Func<TClass, TMember> GetFunctionFastTrack(
                Expression<Func<TClass, TMember>> expression)
            {
                ParameterExpression instance = expression.Parameters[0];
                Expression body = expression.Body;

                return FastTrack<TClass, TMember>.GetFunction(instance, body);
            }

            private static Func<TClass, TMember> GetFunctionFingerprinted(
                Expression<Func<TClass, TMember>> expression)
            {
                ParserContext context = ExpressionParser.Parse(expression);
                if (context.Fingerprint == null)
                    return null;

                var del = _cache.GetDelegate(context);
                object[] hoistedValues = context.HoistedValues.ToArray();
                
                return instance => del(instance, hoistedValues);
            }

            private static Func<TClass, TMember> GetFunctionSlow(
                Expression<Func<TClass, TMember>> expression)
            {
                return expression.Compile();
            }
            #endregion
        }
        #endregion

		#region Static Fields
		private static readonly ParameterExpression _unusedParameter = 
			Expression.Parameter(typeof(object), "_unused");
		#endregion

		#region Static Methods
		private static Func<object, object> Wrap(Expression arg)
		{
			Expression<Func<object, object>> lambda = Expression.Lambda<Func<object, object>>(
				Expression.Convert(arg, typeof(object)), _unusedParameter);

			return CachedExpressionCompiler.Compile(lambda);
		}

		/// <summary>
        /// This is the entry point to the cached expression tree compiler. 
        /// The processor will perform a series of checks and optimizations 
        /// in order to return a fully-compiled func as quickly as possible 
        /// to the caller. If the input expression is particularly obscure, 
        /// the system will fall back to a slow but correct compilation step.
        /// </summary>
        /// <typeparam name="TClass">The type of the container instance.</typeparam>
        /// <typeparam name="TMember">The type of the instance member, the expression 
        /// should get value for.</typeparam>
        /// <param name="expression">An expression to process.</param>
        public static Func<TClass, TMember> Compile<TClass, TMember>(
            Expression<Func<TClass, TMember>> expression)
        {
            return Processor<TClass, TMember>.GetFunction(expression);
		}

		/// <summary>
		/// Evaluates an expression (not a LambdaExpression), e.g. 2 + 2.
		/// </summary>
		/// <param name="arg">The expression to be evaluated.</param>
		public static object Evaluate(Expression arg)
		{
			Precondition.Require(arg, Error.ArgumentNull("arg"));

			Func<object, object> func = Wrap(arg);
			return func(null);
		}
		#endregion
	}
}

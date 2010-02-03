using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

using Jeltofiol.Wahha.Core;

namespace Jeltofiol.Wahha.Data.Linq
{
    /// <summary>
    /// Creates a reusable, parameterized representation of 
    /// a query that caches the execution plan
    /// </summary>
    public static class QueryCompiler
    {
        #region Nested Types
        private class CompiledQuery
        {
            #region Instance Fields
            private LambdaExpression _expression;
            private Delegate _query;
            #endregion

            #region Constructors
            public CompiledQuery(LambdaExpression query)
            {
                _expression = query;
            }
            #endregion

            #region Instance Methods
            private void Compile(params object[] args)
            {
                if (_query == null)
                {
                    // first identify the query provider being used
                    Expression body = _expression.Body;

                    // ask the query provider to compile the query by 'executing' the lambda expression
                    IQueryProvider provider = FindProvider(body, args);
                    Precondition.Require(provider, Error.CouldNotFindQueryProvider());

                    Delegate result = (Delegate)provider.Execute(_expression);
                    Interlocked.CompareExchange(ref _query, result, null);
                }
            }

            private IQueryProvider FindProvider(Expression expression, object[] args)
            {
                ConstantExpression root = TypedSubtreeFinder.Find(expression, 
                    typeof(IQueryProvider)) as ConstantExpression;

                if (root == null)
                    root = TypedSubtreeFinder.Find(expression, typeof(IQueryable)) as ConstantExpression;
                
                if (root == null && args != null && args.Length > 0)
                {
                    Expression replaced = ExpressionReplacer.ReplaceAll(
                            expression, _expression.Parameters.ToArray(),
                            args.Select((a, i) => Expression.Constant(a, _expression.Parameters[i].Type)).ToArray()
                        );
                    Expression partial = PartialEvaluator.Eval(replaced);
                    return FindProvider(partial, null);
                }

                if (root != null)
                {
                    IQueryProvider provider = (root.Value as IQueryProvider);
                    if (provider == null)
                    {
                        IQueryable query = (root.Value as IQueryable);
                        if (query != null)
                            provider = query.Provider;
                    }
                    return provider;
                }
                return null;
            }

            public object Invoke(object[] args)
            {
                Compile(args);
                try
                {
                    return _query.DynamicInvoke(args);
                }
                catch (TargetInvocationException tie)
                {
                    throw tie.InnerException;
                }
            }

            public TResult Invoke<TResult>()
            {
                Compile(null);
                return ((Func<TResult>)_query)();
            }

            public TResult Invoke<T1, TResult>(T1 arg)
            {
                Compile(arg);
                return ((Func<T1, TResult>)_query)(arg);
            }

            public TResult Invoke<T1, T2, TResult>(T1 arg1, T2 arg2)
            {
                Compile(arg1, arg2);
                return ((Func<T1, T2, TResult>)_query)(arg1, arg2);
            }

            public TResult Invoke<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3)
            {
                Compile(arg1, arg2, arg3);
                return ((Func<T1, T2, T3, TResult>)_query)(arg1, arg2, arg3);
            }

            public TResult Invoke<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            {
                Compile(arg1, arg2, arg3, arg4);
                return ((Func<T1, T2, T3, T4, TResult>)_query)(arg1, arg2, arg3, arg4);
            }
            #endregion
        }
        #endregion

        #region Static Methods
        public static Delegate Compile(LambdaExpression query)
        {
            CompiledQuery cq = new CompiledQuery(query);
            Type dt = query.Type;
            
            MethodInfo method = dt.GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public);
            ParameterInfo[] parameters = method.GetParameters();
            ParameterExpression[] pexprs = parameters.Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToArray();

            NewArrayExpression args = Expression.NewArrayInit(typeof(object), 
                pexprs.Select(p => Expression.Convert(p, typeof(object))).ToArray());
            Expression body = Expression.Convert(Expression.Call(Expression.Constant(cq), "Invoke", 
                Type.EmptyTypes, args), method.ReturnType);
            LambdaExpression e = Expression.Lambda(dt, body, pexprs);

            return e.Compile();
        }

        public static T Compile<T>(Expression<T> query)
        {
            return (T)(object)Compile((LambdaExpression)query);
        }

        public static Func<TResult> Compile<TResult>(Expression<Func<TResult>> query)
        {
            return new CompiledQuery(query).Invoke<TResult>;
        }

        public static Func<T1, TResult> Compile<T1, TResult>(Expression<Func<T1, TResult>> query)
        {
            return new CompiledQuery(query).Invoke<T1, TResult>;
        }

        public static Func<T1, T2, TResult> Compile<T1, T2, TResult>(Expression<Func<T1, T2, TResult>> query)
        {
            return new CompiledQuery(query).Invoke<T1, T2, TResult>;
        }

        public static Func<T1, T2, T3, TResult> Compile<T1, T2, T3, TResult>(Expression<Func<T1, T2, T3, TResult>> query)
        {
            return new CompiledQuery(query).Invoke<T1, T2, T3, TResult>;
        }

        public static Func<T1, T2, T3, T4, TResult> Compile<T1, T2, T3, T4, TResult>(Expression<Func<T1, T2, T3, T4, TResult>> query)
        {
            return new CompiledQuery(query).Invoke<T1, T2, T3, T4, TResult>;
        }

        public static Func<IEnumerable<T>> Compile<T>(this IQueryable<T> source)
        {
            return Compile<IEnumerable<T>>(
                Expression.Lambda<Func<IEnumerable<T>>>(
                    ((IQueryable)source).Expression));
        }
        #endregion
    }
}

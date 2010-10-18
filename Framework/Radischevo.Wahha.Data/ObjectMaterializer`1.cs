using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Core.Expressions;

namespace Radischevo.Wahha.Data
{
	public abstract class ObjectMaterializer<TEntity> : IDbMaterializer<TEntity>
	{
		#region Constructors
		protected ObjectMaterializer()
		{
		}
		#endregion

		#region Instance Methods
		public TEntity Materialize(IValueSet source)
		{
			return Materialize(CreateInstance(source), source);
		}

		public virtual TEntity Materialize(TEntity entity, IValueSet source)
		{
			Precondition.Require(entity, () => Error.ArgumentNull("entity"));
			Precondition.Require(source, () => Error.ArgumentNull("source"));

			return Execute(entity, source);
		}

		protected abstract TEntity Execute(TEntity entity, IValueSet source);

		protected abstract TEntity CreateInstance(IValueSet source);

		protected virtual Link<TAssociation> Associate<TAssociation, TKey>(
			Link<TAssociation> link, TKey key, IValueSet source, string prefix,
			params string[] keys)
			where TAssociation : class
		{
			Precondition.Require(link, () => new ArgumentNullException("link"));
			Precondition.Require(source, () => new ArgumentNullException("source"));

			int prefixLength = prefix.Length;
			IValueSet subset = source.Subset(k => k.StartsWith(prefix))
				.Transform(k => k.Substring(prefixLength));

			link.Source = CreateAssociationSource(link, key);
			link.Tag = key;

			if (subset.ContainsAll(keys))
				link.Value = MaterializeAssociation(link, subset);

			return link;
		}

		protected TAssociation MaterializeAssociation<TAssociation>(Link<TAssociation> link, IValueSet source)
			where TAssociation : class
		{
			Func<IValueSet, TAssociation> materializer = CreateAssociationMaterializer<TAssociation>();
			return materializer(source);
		}

		protected virtual Func<IValueSet, TAssociation> CreateAssociationMaterializer<TAssociation>()
		{
			Type materializerType = typeof(IDbMaterializer<>).MakeGenericType(typeof(TAssociation));
			BindingFlags instanceFlags = BindingFlags.Instance | BindingFlags.Public;

			MethodInfo resolveMethod = typeof(IServiceProvider).GetMethod("GetService",
				instanceFlags, null, new Type[] { typeof(Type) }, null);
			MethodInfo materializeMethod = materializerType.GetMethod("Materialize",
				instanceFlags, null, new Type[] { typeof(IValueSet) }, null);

			ParameterExpression values = Expression.Parameter(typeof(IValueSet), "values");
			ConstantExpression service = Expression.Constant(ServiceLocator.Instance, typeof(IServiceLocator));
			ConstantExpression serviceType = Expression.Constant(materializerType, typeof(Type));

			MethodCallExpression resolver = Expression.Call(service, resolveMethod, serviceType);
			UnaryExpression conversion = Expression.Convert(resolver, materializerType);
			MethodCallExpression invocation = Expression.Call(conversion, materializeMethod, values);
			Expression<Func<IValueSet, TAssociation>> func = Expression.Lambda<Func<IValueSet,
				TAssociation>>(invocation, values);

			return CachedExpressionCompiler.Compile(func);
		}

		protected virtual Func<TAssociation> CreateAssociationSource<TAssociation, TKey>(
			Link<TAssociation> reference, TKey key)
			where TAssociation : class
		{
			Type selectorType = typeof(IRepository<,>)
				.MakeGenericType(typeof(TAssociation), typeof(TKey));
			BindingFlags instanceFlags = BindingFlags.Instance | BindingFlags.Public;

			MethodInfo resolveMethod = typeof(IServiceProvider).GetMethod("GetService",
				instanceFlags, null, new Type[] { typeof(Type) }, null);
			MethodInfo singleMethod = selectorType.GetMethod("Single",
				instanceFlags, null, new Type[] { typeof(TKey) }, null);

			ConstantExpression keyParameter = Expression.Constant(key, typeof(TKey));
			ConstantExpression service = Expression.Constant(ServiceLocator.Instance, typeof(IServiceLocator));
			ConstantExpression serviceType = Expression.Constant(selectorType, typeof(Type));

			MethodCallExpression resolver = Expression.Call(service, resolveMethod, serviceType);
			UnaryExpression conversion = Expression.Convert(resolver, selectorType);
			MethodCallExpression single = Expression.Call(conversion, singleMethod, keyParameter);
			Expression<Func<TAssociation>> func = Expression.Lambda<Func<TAssociation>>(single);

			return func.Compile();
		}
		#endregion
	}
}

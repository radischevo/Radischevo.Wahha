using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Radischevo.Wahha.Core
{
	public static class ObjectFactoryBuilder
	{
		#region Static Methods
		private static ConstructorInfo GetConstructor<TObject>(params Type[] parameterTypes)
			where TObject : class
		{
			ConstructorInfo constructor = typeof(TObject).GetConstructor(
				BindingFlags.Instance | BindingFlags.Public, null, parameterTypes, null);

			return constructor;
		}

		private static NewExpression CreateExpression<TObject>(params Type[] parameterTypes)
			where TObject : class
		{
			ConstructorInfo constructor = GetConstructor<TObject>(parameterTypes);
			Precondition.Require(constructor, () => Error.CouldNotFindAppropriateConstructor(typeof(TObject)));

			IEnumerable<Expression> parameters = parameterTypes
				.Select((t, i) => Expression.Parameter(t, String.Format("param{0}", i)))
				.Cast<Expression>();

			return Expression.New(constructor, parameters);
		}

		public static Func<TObject> CreateFactory<TObject>()
			where TObject : class
		{
			NewExpression invoker = CreateExpression<TObject>();
			return Expression.Lambda<Func<TObject>>(invoker).Compile();
		}

		public static Func<TParameter, TObject> CreateFactory<TParameter, TObject>()
			where TObject : class
		{
			NewExpression invoker = CreateExpression<TObject>(typeof(TParameter));
			return Expression.Lambda<Func<TParameter, TObject>>(invoker, 
				Expression.Parameter(typeof(TParameter), "param0")).Compile();
		}

		public static Func<TParameter1, TParameter2, TObject> CreateFactory<TParameter1, TParameter2, TObject>()
			where TObject : class
		{
			NewExpression invoker = CreateExpression<TObject>(typeof(TParameter1), typeof(TParameter2));
			return Expression.Lambda<Func<TParameter1, TParameter2, TObject>>(invoker,
				Expression.Parameter(typeof(TParameter1), "param0"), 
				Expression.Parameter(typeof(TParameter2), "param1")).Compile();
		}

		public static Func<TParameter1, TParameter2, TParameter3, TObject> CreateFactory<TParameter1, 
			TParameter2, TParameter3, TObject>()
			where TObject : class
		{
			NewExpression invoker = CreateExpression<TObject>(typeof(TParameter1), 
				typeof(TParameter2), typeof(TParameter3));

			return Expression.Lambda<Func<TParameter1, TParameter2, TParameter3, TObject>>(invoker,
				Expression.Parameter(typeof(TParameter1), "param0"),
				Expression.Parameter(typeof(TParameter2), "param1"), 
				Expression.Parameter(typeof(TParameter3), "param2")).Compile();
		}

		public static Func<TParameter1, TParameter2, TParameter3, TParameter4, TObject> CreateFactory<TParameter1,
			TParameter2, TParameter3, TParameter4, TObject>()
			where TObject : class
		{
			NewExpression invoker = CreateExpression<TObject>(typeof(TParameter1),
				typeof(TParameter2), typeof(TParameter3), typeof(TParameter4));

			return Expression.Lambda<Func<TParameter1, TParameter2, TParameter3, TParameter4, TObject>>(invoker,
				Expression.Parameter(typeof(TParameter1), "param0"),
				Expression.Parameter(typeof(TParameter2), "param1"),
				Expression.Parameter(typeof(TParameter3), "param2"),
				Expression.Parameter(typeof(TParameter4), "param3")).Compile();
		}
		#endregion
	}
}

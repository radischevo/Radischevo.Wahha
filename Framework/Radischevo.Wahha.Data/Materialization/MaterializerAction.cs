using System;
using System.Linq.Expressions;
using System.Reflection;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Core.Expressions;

namespace Radischevo.Wahha.Data
{
	public class MaterializerAction<TAssociation> : AssociatorAction<TAssociation>
		where TAssociation : class
	{
		#region Nested Types
		private class NullValueSetTransformer : IValueSetTransformer
		{
			#region Constructors
			public NullValueSetTransformer()
			{
			}
			#endregion

			#region Instance Methods
			public IValueSet Transform(IValueSet source)
			{
				return source;
			}
			#endregion
		}

		private class NullValueSetValidator : IValueSetValidator
		{
			#region Constructors
			public NullValueSetValidator()
			{
			}
			#endregion

			#region Instance Methods
			public bool Valid(IValueSet source)
			{
				return source.ContainsAny();
			}
			#endregion
		}
		#endregion

		#region Instance Fields
		private IValueSet _source;
		private IValueSetTransformer _transformer;
		private IValueSetValidator _validator;
		#endregion

		#region Constructors
		public MaterializerAction(IValueSet source)
			: base()
		{
			Precondition.Require(source, () => Error.ArgumentNull("source"));
			_source = source;
		}
		#endregion

		#region Instance Properties
		public IValueSetTransformer Transformer
		{
			get
			{
				if (_transformer == null)
					_transformer = new NullValueSetTransformer();

				return _transformer;
			}
			set
			{
				_transformer = value;
			}
		}

		public IValueSetValidator Validator
		{
			get
			{
				if (_validator == null)
					_validator = new NullValueSetValidator();

				return _validator;
			}
			set
			{
				_validator = value;
			}
		}

		public IValueSet Source
		{
			get
			{
				return _source;
			}
		}
		#endregion

		#region Static Methods
		private static Link<TAssociation> ConvertLink(ILink<TAssociation> link)
		{
			Link<TAssociation> result = (link as Link<TAssociation>);
			Precondition.Require(result, () => Error.CouldNotMaterializeCollectionLink("link"));

			return result;
		}
		#endregion

		#region Instance Methods
		protected Func<IValueSet, TAssociation> CreateMaterializer(Type type)
		{
			BindingFlags instanceFlags = BindingFlags.Instance | BindingFlags.Public;
			MethodInfo resolveMethod = typeof(IServiceProvider).GetMethod("GetService",
				instanceFlags, null, new Type[] { typeof(Type) }, null);
			MethodInfo materializeMethod = type.GetMethod("Materialize",
				instanceFlags, null, new Type[] { typeof(IValueSet) }, null);

			Precondition.Require(materializeMethod, () => Error.IncompatibleMaterializerType(type));

			ParameterExpression values = Expression.Parameter(typeof(IValueSet), "values");
			ConstantExpression service = Expression.Constant(ServiceLocator.Instance, typeof(IServiceLocator));
			ConstantExpression serviceType = Expression.Constant(type, typeof(Type));

			MethodCallExpression resolver = Expression.Call(service, resolveMethod, serviceType);
			UnaryExpression conversion = Expression.Convert(resolver, type);
			MethodCallExpression invocation = Expression.Call(conversion, materializeMethod, values);
			Expression<Func<IValueSet, TAssociation>> func = Expression.Lambda<Func<IValueSet,
				TAssociation>>(invocation, values);

			return CachedExpressionCompiler.Compile(func);
		}

		protected virtual Func<IValueSet, TAssociation> CreateMaterializer()
		{
			Type type = typeof(IDbMaterializer<>).MakeGenericType(typeof(TAssociation));
			return CreateMaterializer(type);			
		}

		public override void Execute(ILink<TAssociation> link)
		{
			Link<TAssociation> association = ConvertLink(link);

			Func<IValueSet, TAssociation> materializer = CreateMaterializer();
			IValueSet subset = Transformer.Transform(Source);

			if (Validator.Valid(subset) && materializer != null)
				association.Value = materializer(subset);
		}
		#endregion
	}

	public class MaterializerAction<TAssociation, TMaterializer> : MaterializerAction<TAssociation>
		where TAssociation : class
		where TMaterializer : IDbMaterializer<TAssociation>
	{
		#region Constructors
		public MaterializerAction(IValueSet source)
			: base(source)
		{
		}
		#endregion

		#region Instance Methods
		protected override Func<IValueSet, TAssociation> CreateMaterializer()
		{
			return CreateMaterializer(typeof(TMaterializer));
		}
		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	[Serializable]
	internal abstract class RepositoryAssociationLoader<TAssociation> 
		: IAssociationLoader<TAssociation>, ISerializable
		where TAssociation : class
	{
		#region Static Fields
		private static readonly BindingFlags _staticFlags = BindingFlags.Static | BindingFlags.Public;
		private static readonly MethodInfo _serviceResolveMethod = typeof(IServiceProvider).GetMethod("GetService",
			BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(Type) }, null);
		private static readonly MemberInfo _locatorInstance = typeof(ServiceLocator).GetProperty("Instance",
			BindingFlags.Static | BindingFlags.Public);
		#endregion

		#region Instance Fields
		private Type _type;
		private MethodInfo _method;
		private object[] _arguments;
		private Func<TAssociation> _selector;
		#endregion

		#region Constructors
		protected RepositoryAssociationLoader(Type type, MethodInfo method, object[] arguments)
		{
			Precondition.Require(type, () => Error.ArgumentNull("type"));
			Precondition.Require(method, () => Error.ArgumentNull("method"));
			Precondition.Require(arguments, () => Error.ArgumentNull("arguments"));

			_type = type;
			_method = method;
			_arguments = arguments;
		}

		protected RepositoryAssociationLoader(SerializationInfo info, StreamingContext context)
		{
			_type = (Type)info.GetValue("type", typeof(Type));
			_method = (MethodInfo)info.GetValue("method", typeof(MethodInfo));
			_arguments = (object[])info.GetValue("arguments", typeof(object[]));
		}
		#endregion

		#region Instance Methods
		private void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("type", _type);
			info.AddValue("method", _method);
			info.AddValue("arguments", _arguments);
		}

		private IEnumerable<Expression> CreateMethodParameters()
		{
			List<Expression> list = new List<Expression>();
			ParameterInfo[] parameters = _method.GetParameters();

			if (parameters.Length > 0)
			{
				for (int i = 0; i < parameters.Length; i++)
					list.Add(Expression.Constant(_arguments[i], parameters[i].ParameterType));
			}
			return list;
		}

		private Func<TAssociation> CreateDelegate()
		{
			MemberExpression service = Expression.MakeMemberAccess(null, _locatorInstance);
			ConstantExpression serviceType = Expression.Constant(_type, typeof(Type));

			MethodCallExpression resolver = Expression.Call(service, _serviceResolveMethod, serviceType);
			UnaryExpression conversion = Expression.Convert(resolver, _type);

			MethodCallExpression method = Expression.Call(conversion, _method, CreateMethodParameters());
			Expression<Func<TAssociation>> func = Expression.Lambda<Func<TAssociation>>(method);

			return func.Compile();
		}

		public TAssociation Load()
		{
			if (_selector == null)
				_selector = CreateDelegate();

			return _selector();
		}
		#endregion

		#region Serialization Methods
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			GetObjectData(info, context);
		}
		#endregion
	}

	[Serializable]
	internal sealed class SingleAssociationLoader<TRepository, TAssociation>
		: RepositoryAssociationLoader<TAssociation>
		where TAssociation : class
		where TRepository : IRepository<TAssociation>
	{
		#region Constructors
		public SingleAssociationLoader(MethodInfo method, object[] arguments)
			: base(typeof(TRepository), method, arguments)
		{
		}

		private SingleAssociationLoader(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
		#endregion
	}

	[Serializable]
	internal sealed class CollectionAssociationLoader<TRepository, TAssociation>
		: RepositoryAssociationLoader<IEnumerable<TAssociation>>
		where TAssociation : class
		where TRepository : IRepository<TAssociation>
	{
		#region Constructors
		public CollectionAssociationLoader(MethodInfo method, object[] arguments)
			: base(typeof(TRepository), method, arguments)
		{
		}

		private CollectionAssociationLoader(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
		#endregion
	}
}

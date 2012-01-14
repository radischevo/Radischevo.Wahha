using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	[Serializable]
	internal abstract class MethodBasedAssociationLoader<TAssociation>
		: IAssociationLoader<TAssociation>, ISerializable
	{
		#region Static Fields
		private static readonly MethodInfo _serviceResolveMethod = typeof(IServiceProvider).GetMethod("GetService",
			BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(Type) }, null);
		private static readonly MemberInfo _locatorInstance = typeof(ServiceLocator).GetProperty("Instance",
			BindingFlags.Static | BindingFlags.Public);
		#endregion

		#region Instance Fields
		private Type _declaringType;
		private MethodInfo _method;
		private object[] _arguments;
		private Func<TAssociation> _factory;
		#endregion

		#region Constructors
		protected MethodBasedAssociationLoader(Type declaringType, MethodInfo method, object[] arguments)
		{
			Precondition.Require(declaringType, () => Error.ArgumentNull("factoryType"));
			Precondition.Require(method, () => Error.ArgumentNull("method"));
			Precondition.Require(arguments, () => Error.ArgumentNull("arguments"));

			_declaringType = declaringType;
			_method = method;
			_arguments = arguments;
		}

		protected MethodBasedAssociationLoader(SerializationInfo info, StreamingContext context)
		{
			_declaringType = (Type)info.GetValue("type", typeof(Type));
			_method = (MethodInfo)info.GetValue("method", typeof(MethodInfo));
			_arguments = (object[])info.GetValue("arguments", typeof(object[]));
		}
		#endregion

		#region Instance Properties
		protected Type DeclaringType
		{
			get
			{
				return _declaringType;
			}
		}

		protected MethodInfo Method
		{
			get
			{
				return _method;
			}
		}

		protected object[] Arguments
		{
			get
			{
				return _arguments;
			}
		}
		#endregion

		#region Instance Methods
		private void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("type", _declaringType);
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

		protected virtual MethodCallExpression CreateMethodInvocation()
		{
			MemberExpression service = Expression.MakeMemberAccess(null, _locatorInstance);
			ConstantExpression serviceType = Expression.Constant(_declaringType, typeof(Type));

			MethodCallExpression resolver = Expression.Call(service, _serviceResolveMethod, serviceType);
			UnaryExpression conversion = Expression.Convert(resolver, _declaringType);

			return Expression.Call(conversion, _method, CreateMethodParameters());
		}

		protected virtual Func<TAssociation> CreateValueFactory()
		{
			Expression expression = CreateMethodInvocation();
			Expression<Func<TAssociation>> invoker = 
				Expression.Lambda<Func<TAssociation>>(expression);

			return invoker.Compile();
		}

		public TAssociation Load()
		{
			if (_factory == null)
				_factory = CreateValueFactory();

			return _factory();
		}
		#endregion

		#region Serialization Methods
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			GetObjectData(info, context);
		}
		#endregion
	}
}

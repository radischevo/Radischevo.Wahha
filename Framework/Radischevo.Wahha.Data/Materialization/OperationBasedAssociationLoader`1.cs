using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	[Serializable]
	internal class OperationBasedAssociationLoader<TAssociation>
		: IAssociationLoader<TAssociation>, ISerializable
		where TAssociation : class
	{
		#region Static Fields
		private static readonly MethodInfo _executeOperationMethod =
			typeof(ScopedDbOperationExecutor<>).MakeGenericType(typeof(TAssociation))
			.GetMethod("Execute", BindingFlags.Static | BindingFlags.Public);
		#endregion

		#region Instance Fields
		private ConstructorInfo _constructor;
		private object[] _arguments;
		private Func<TAssociation> _factory;
		#endregion

		#region Constructors
		public OperationBasedAssociationLoader(ConstructorInfo constructor, object[] arguments)
		{
			Precondition.Require(constructor, () => Error.ArgumentNull("constructor"));
			Precondition.Require(arguments, () => Error.ArgumentNull("arguments"));

			_constructor = constructor;
			_arguments = arguments;
		}

		protected OperationBasedAssociationLoader(SerializationInfo info, StreamingContext context)
		{
			_constructor = (ConstructorInfo)info.GetValue("constructor", typeof(ConstructorInfo));
			_arguments = (object[])info.GetValue("arguments", typeof(object[]));
		}
		#endregion

		#region Instance Properties
		protected ConstructorInfo Constructor
		{
			get
			{
				return _constructor;
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
			info.AddValue("constructor", _constructor);
			info.AddValue("arguments", _arguments);
		}

		private IEnumerable<Expression> CreateMethodParameters()
		{
			List<Expression> list = new List<Expression>();
			ParameterInfo[] parameters = _constructor.GetParameters();

			if (parameters.Length > 0)
			{
				for (int i = 0; i < parameters.Length; i++)
					list.Add(Expression.Constant(_arguments[i], parameters[i].ParameterType));
			}
			return list;
		}

		protected virtual Func<TAssociation> CreateValueFactory()
		{
			Expression instance = Expression.New(_constructor, CreateMethodParameters());
			MethodCallExpression execute = Expression.Call(_executeOperationMethod, instance);
			Expression<Func<TAssociation>> invoker = Expression.Lambda<Func<TAssociation>>(execute);
			
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

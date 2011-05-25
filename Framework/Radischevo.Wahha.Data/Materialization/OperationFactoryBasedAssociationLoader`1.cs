using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace Radischevo.Wahha.Data
{
	[Serializable]
	internal class OperationFactoryBasedAssociationLoader<TAssociation> 
		: MethodBasedAssociationLoader<TAssociation>
		where TAssociation : class
	{
		#region Static Fields
		private static readonly MethodInfo _executeOperationMethod =
			typeof(ScopedDbOperationExecutor<>).MakeGenericType(typeof(TAssociation))
			.GetMethod("Execute", BindingFlags.Static | BindingFlags.Public);
		#endregion

		#region Constructors
		public OperationFactoryBasedAssociationLoader(Type declaringType, MethodInfo method, object[] arguments)
			: base(declaringType, method, arguments)
		{
			Type operationType = typeof(IDbOperation<>).MakeGenericType(typeof(TAssociation));
			if (!operationType.IsAssignableFrom(method.ReturnType))
				throw Error.InvalidMethodReturnType("method", method.ReturnType, operationType);
		}

		protected OperationFactoryBasedAssociationLoader(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
		#endregion

		#region Instance Methods
		protected override MethodCallExpression CreateMethodInvocation()
		{
			Expression instance = base.CreateMethodInvocation();
			MethodCallExpression execute = Expression.Call(
				_executeOperationMethod, instance);

			return execute;
		}
		#endregion
	}
}

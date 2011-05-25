using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Radischevo.Wahha.Data
{
	[Serializable]
	internal class RepositoryBasedAssociationLoader<TAssociation> 
		: MethodBasedAssociationLoader<TAssociation>
		where TAssociation : class
	{
		#region Constructors
		public RepositoryBasedAssociationLoader(Type declaringType, MethodInfo method, object[] arguments)
			: base(declaringType, method, arguments)
		{
			if (!typeof(TAssociation).IsAssignableFrom(method.ReturnType))
				throw Error.InvalidMethodReturnType("method", method.ReturnType, typeof(TAssociation));
		}

		protected RepositoryBasedAssociationLoader(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
		#endregion
	}
}

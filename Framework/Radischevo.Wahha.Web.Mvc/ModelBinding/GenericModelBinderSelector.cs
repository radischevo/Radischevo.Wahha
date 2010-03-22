using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public abstract class GenericModelBinderSelector : ModelBinderSelector
	{
		#region Instance Fields
		private Type _type;
		private Func<Type[], IModelBinder> _factory;
		#endregion

		#region Constructors
		protected GenericModelBinderSelector(Type type, IModelBinder binder) 
		{
			Precondition.Require(type, () => Error.ArgumentNull("type"));
			Precondition.Require(binder, () => Error.ArgumentNull("binder"));

            ValidateParameters(type, null);

			_type = type;
            _factory = (args) => binder;
        }

        protected GenericModelBinderSelector(Type type, Type binderType) 
		{
			Precondition.Require(type, () => Error.ArgumentNull("type"));
			Precondition.Require(binderType, () => Error.ArgumentNull("binderType"));

            ValidateParameters(type, binderType);
            bool isOpenGeneric = binderType.IsGenericTypeDefinition;

            _type = type;
            _factory = (args) => {
                Type closedType = (isOpenGeneric) ? binderType.MakeGenericType(args) : binderType;
				return (IModelBinder)Activator.CreateInstance(closedType);
            };
        }

		protected GenericModelBinderSelector(Type type, Func<Type[], IModelBinder> factory)
		{
			Precondition.Require(type, () => Error.ArgumentNull("type"));
			Precondition.Require(factory, () => Error.ArgumentNull("factory"));

            ValidateParameters(type, null);

            _type = type;
            _factory = factory;
        }
		#endregion

		#region Instance Properties
		public Type Type
		{
			get
			{
				return _type;
			}
		}
		#endregion

		#region Static Methods
		private static void ValidateParameters(Type modelType, Type binderType)
		{
			if (!modelType.IsGenericTypeDefinition)
				throw Error.UnsupportedModelType(modelType);
			
			if (binderType != null)
			{
				if (!typeof(IModelBinder).IsAssignableFrom(binderType))
					throw Error.IncompatibleModelBinderType(binderType);
				
				if (binderType.IsGenericTypeDefinition)
				{
					if (modelType.GetGenericArguments().Length != binderType.GetGenericArguments().Length)
						throw Error.TypeArgumentCountMismatch(modelType, binderType);
				}
			}
		}

		public static Type[] GetTypeArgumentsIfMatch(Type closedType, Type matchingOpenType)
		{
			if (!closedType.IsGenericType)
				return null;

			Type openType = closedType.GetGenericTypeDefinition();
			return (matchingOpenType == openType) ? closedType.GetGenericArguments() : null;
		}
		#endregion

		#region Instance Methods
		public override IModelBinder GetBinder(Type modelType)
		{
			Type[] args = null;

			if (Type.IsInterface)
			{
				Type matchingClosedInterface = modelType.GetGenericInterface(Type);
				if (matchingClosedInterface != null)
					args = matchingClosedInterface.GetGenericArguments();
			}
			else
				args = GetTypeArgumentsIfMatch(modelType, Type);

			if (args != null)
				return _factory(args);

			return null;
		}
		#endregion
	}
}

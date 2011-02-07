using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public abstract class ActionDescriptor : ICustomAttributeProvider
	{
		#region Static Fields
		private static readonly ActionSelector[] _emptySelectors = new ActionSelector[0];
		private static readonly MethodDispatcherCache _staticDispatcherCache = new MethodDispatcherCache();
        #endregion

		#region Instance Fields
		private MethodDispatcherCache _dispatcherCache;
		#endregion

		#region Constructors
		protected ActionDescriptor()
        {   }
        #endregion

        #region Instance Properties
        public abstract ControllerDescriptor Controller 
        { 
            get; 
        }

        public abstract string Name 
        { 
            get; 
        }

        public abstract MethodInfo Method
        {
            get;
        }

		public MethodDispatcherCache DispatcherCache
		{
			get
			{
				if (_dispatcherCache == null)
					_dispatcherCache = _staticDispatcherCache;
				
				return _dispatcherCache;
			}
			set
			{
				_dispatcherCache = value;
			}
		}
        #endregion

		#region Static Methods
		protected static void ValidateActionMethod(MethodInfo method)
		{
			if (!method.IsStatic && !typeof(ControllerBase).IsAssignableFrom(method.ReflectedType))
				throw Error.CannotCallInstanceMethodOnNonControllerType(method);

			if (method.ContainsGenericParameters)
				throw Error.MvcActionCannotBeGeneric(method);

			foreach (ParameterInfo info in method.GetParameters())
				if (info.IsOut || info.ParameterType.IsByRef)
					throw Error.ReferenceActionParametersNotSupported(method, info.Name);
		}
		
		protected static object ExtractParameter(ParameterInfo parameter,
			IDictionary<string, object> parameters, MethodInfo action)
		{
			object value;
			if (!parameters.TryGetValue(parameter.Name, out value))
				throw Error.MissingActionParameter(action, parameter);

			if (value == null && !parameter.ParameterType.IsNullable())
				throw Error.ParameterCannotBeNull(action, parameter);

			if (value != null && !parameter.ParameterType.IsInstanceOfType(value))
				throw Error.ParameterValueHasWrongType(action, parameter);

			return value;
		}

		protected static TDescriptor[] FetchOrCreateDescriptors<TReflection, TDescriptor>(ref TDescriptor[] cache,
			Func<TReflection[]> initializer, Func<TReflection, TDescriptor> converter)
		{
			TDescriptor[] local = Interlocked.CompareExchange<TDescriptor[]>(ref cache, null, null);
			if (local != null)
				return local;

			local = initializer().Select<TReflection, TDescriptor>(converter)
				.Where<TDescriptor>(d => d != null).ToArray<TDescriptor>();

			return (Interlocked.CompareExchange<TDescriptor[]>(ref cache, local, null) ?? local);
		}
		#endregion

		#region Instance Methods
		public abstract object Execute(ControllerContext context, IDictionary<string, object> parameters);

        public abstract IEnumerable<ParameterDescriptor> GetParameters();
 
        public virtual IEnumerable<ActionSelector> GetSelectors()
        {
            return _emptySelectors;
        }

        public virtual IEnumerable<FilterAttribute> GetFilters(bool useCache)
        {
			return GetCustomAttributes(typeof(FilterAttribute), true).Cast<FilterAttribute>();
        }

        public virtual object[] GetCustomAttributes(bool inherit)
        {
            return GetCustomAttributes(typeof(object), inherit);
        }

        public virtual object[] GetCustomAttributes(Type type, bool inherit)
        {
            Precondition.Require(type, () => Error.ArgumentNull("type"));
            return (object[])Array.CreateInstance(type, 0);
        }

        public virtual bool IsDefined(Type type, bool inherit)
        {
            Precondition.Require(type, () => Error.ArgumentNull("type"));
            return false;
        }
        #endregion
    }
}

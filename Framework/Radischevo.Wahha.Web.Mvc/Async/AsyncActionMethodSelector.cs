using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Async
{
	public class AsyncActionMethodSelector : ActionMethodSelector
	{
		#region Constructors
		public AsyncActionMethodSelector(Type controllerType)
			: base(controllerType)
		{
		}
		#endregion

		#region Static Methods
		private static bool IsAsyncSuffixedMethod(MethodInfo method)
		{
			return method.Name.EndsWith("Async", 
				StringComparison.OrdinalIgnoreCase);
		}

		private static bool IsCompletedSuffixedMethod(MethodInfo method)
		{
			return method.Name.EndsWith("Completed", 
				StringComparison.OrdinalIgnoreCase);
		}
		#endregion

		#region Instance Methods
		private MethodInfo GetMethod(string methodName)
		{
			List<MethodInfo> methods = ControllerType
				.GetMember(methodName, MemberTypes.Method, BindingFlags.Instance | 
					BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase)
				.Cast<MethodInfo>()
				.Where(m => IsActionMethod(m, false))
				.ToList();

			switch (methods.Count)
			{
				case 0:
					return null;
				case 1:
					return methods[0];
				default:
					throw Error.AmbiguousActionName(ControllerType, methodName);
			}
		}

		private ActionDescriptorCreator CreateDescriptor(MethodInfo method)
		{
			if (IsAsyncSuffixedMethod(method))
			{
				string completionMethodName = method.Name.Substring(0,
					method.Name.Length - "Async".Length) + "Completed";

				MethodInfo completionMethod = GetMethod(completionMethodName);
				if (completionMethod != null)
					return (name, descriptor) => new ReflectedAsyncActionDescriptor(method, completionMethod, name, descriptor);

				throw Error.CouldNotFindAsyncCompletionMethod(ControllerType, completionMethodName);
			}
			return (name, descriptor) => new ReflectedActionDescriptor(method, name, descriptor);
		}

		protected override bool IsActionMethod(MethodInfo method)
		{
			return base.IsActionMethod(method) && IsActionMethod(method, true);
		}

		protected virtual bool IsActionMethod(MethodInfo method, 
			bool ignoreInfrastructureMethods)
		{
			if (method.GetBaseDefinition().DeclaringType.IsAssignableFrom(typeof(AsyncController)))
				return false;

			if (ignoreInfrastructureMethods && IsCompletedSuffixedMethod(method))
				return false;
			
			return true;
		}

		protected override string GetCanonicalMethodName(MethodInfo method)
		{
			Precondition.Require(method, Error.ArgumentNull("method"));

			ActionNameAttribute[] attrs =
				(ActionNameAttribute[])method.GetCustomAttributes(typeof(ActionNameAttribute), true);
			if (attrs.Length > 0)
				return attrs[0].Name;

			string methodName = method.Name;
			return (IsAsyncSuffixedMethod(method))
				? methodName.Substring(0, methodName.Length - "Async".Length)
				: methodName;
		}

		public virtual ActionDescriptorCreator GetAsyncDescriptor(
			ControllerContext context, string actionName)
		{
			actionName = (actionName.EndsWith("Async", StringComparison.OrdinalIgnoreCase))
				? actionName.Substring(0, actionName.Length - "Async".Length)
				: actionName;

			return CreateDescriptor(GetActionMethod(context, actionName));
		}
		#endregion
	}
}

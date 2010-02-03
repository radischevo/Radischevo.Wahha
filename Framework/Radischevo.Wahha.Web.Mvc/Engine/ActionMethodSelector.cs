using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public class ActionMethodSelector
    {
        #region Instance Fields
        private Type _controllerType;
        private ILookup<string, ActionMethodCacheEntry> _methods;
        #endregion

        #region Constructors
        public ActionMethodSelector(Type controllerType)
        {
            _controllerType = controllerType;
            InitMethodCache();
        }
        #endregion

        #region Instance Properties
		public Type ControllerType
		{
			get
			{
				return _controllerType;
			}
		}

        public IEnumerable<MethodInfo> Methods
        {
            get
            {
                return _methods.SelectMany(g => g.Select(i => i.Method));
            }
        }
        #endregion

		#region Static Methods
		public static string GetNameOrAlias(MethodInfo method)
		{
			Precondition.Require(method, Error.ArgumentNull("method"));

			ActionNameAttribute[] attrs =
				(ActionNameAttribute[])method.GetCustomAttributes(typeof(ActionNameAttribute), true);
			if (attrs.Length > 0)
				return attrs[0].Name;

			return method.Name;
		}
		#endregion

		#region Instance Methods
		private void InitMethodCache()
        {
            MethodInfo[] methods = Array.FindAll(_controllerType.GetMethods(BindingFlags.Instance | 
                BindingFlags.Public | BindingFlags.InvokeMethod), IsActionMethod);

            _methods = methods.ToLookup(m => GetCanonicalMethodName(m), 
                m => new ActionMethodCacheEntry(m), StringComparer.OrdinalIgnoreCase);
        }

		protected virtual bool IsActionMethod(MethodInfo method)
        {
            Precondition.Require(method, Error.ArgumentNull("method"));

            if (method.IsDefined(typeof(IgnoreActionAttribute), false))
                return false;

            return (!method.IsAbstract && !method.IsSpecialName && !method.ContainsGenericParameters &&
                typeof(Controller).IsAssignableFrom(method.GetBaseDefinition().DeclaringType));
        }

		protected virtual string GetCanonicalMethodName(MethodInfo method)
		{
			return GetNameOrAlias(method);
		}

        public virtual MethodInfo GetActionMethod(ControllerContext context, string actionName)
        {
            Precondition.Require(context, Error.ArgumentNull("context"));
            Precondition.Require(!String.IsNullOrEmpty(actionName), Error.ArgumentNull("actionName"));

            List<ActionMethodCacheEntry> matchingMethods = _methods[actionName].Where(m => m.IsValidFor(context)).ToList();
            switch (matchingMethods.Count)
            {
                case 0:
                    return null;
                case 1: 
                    return matchingMethods[0].Method;
            }
            throw Error.AmbiguousActionName(context.Controller.GetType(), actionName);
        }
        #endregion
    }
}
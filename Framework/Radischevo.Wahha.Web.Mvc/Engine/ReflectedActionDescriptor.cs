﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public class ReflectedActionDescriptor : ActionDescriptor
	{
		#region Static Fields
		private static readonly ActionFilterCache _filterCache = new ActionFilterCache();
		#endregion

		#region Instance Fields
		private string _name;
        private MethodInfo _method;        
        private ControllerDescriptor _controller;
        private ParameterDescriptor[] _parameterCache;
        #endregion

        #region Constructors
        public ReflectedActionDescriptor(MethodInfo method, string name, 
            ControllerDescriptor controller)
            : this(method, name, controller, true)
        {
        }

        internal ReflectedActionDescriptor(MethodInfo method, string name, 
            ControllerDescriptor controller, bool validateMethod)
        {
            Precondition.Require(method, () => Error.ArgumentNull("method"));
            Precondition.Defined(name, () => Error.ArgumentNull("name"));
            Precondition.Require(controller, () => Error.ArgumentNull("controller"));

            if (validateMethod)
                ValidateActionMethod(method);

            _method = method;
            _name = name;
            _controller = controller;
        }
        #endregion

        #region Instance Properties
        public override ControllerDescriptor Controller
        {
            get 
            {
                return _controller;
            }
        }

        public override string Name
        {
            get 
            {
                return _name;
            }
        }

        public override MethodInfo Method
        {
            get 
            {
                return _method;
            }
        }
        #endregion

		#region Static Methods
		public static ReflectedActionDescriptor CreateDescriptor(MethodInfo method,
			string name, ControllerDescriptor controller)
		{
			ReflectedActionDescriptor descriptor = new ReflectedActionDescriptor(method, name, controller, false);
			ValidateActionMethod(method);

			return descriptor;
		}
		#endregion

        #region Instance Methods
        public override object[] GetCustomAttributes(bool inherit)
        {
            return _method.GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(Type type, bool inherit)
        {
            return _method.GetCustomAttributes(type, inherit);
        }

        public override bool IsDefined(Type type, bool inherit)
        {
            return _method.IsDefined(type, inherit);
        }

        public override IEnumerable<ActionSelector> GetSelectors()
        {
            ActionSelectorAttribute[] attributes = _method.GetCustomAttributes<ActionSelectorAttribute>(true).ToArray();
            return Array.ConvertAll<ActionSelectorAttribute, ActionSelector>(attributes, attr => {
                return context => attr.IsValid(context, _method);
            });
        }

        public override IEnumerable<FilterAttribute> GetFilters(bool useCache)
        {
			if (useCache)
				return _filterCache.GetFilters(_method);
			
			return base.GetFilters(useCache);
        }

        public override IEnumerable<ParameterDescriptor> GetParameters()
        {
            MethodInfo method = _method;
            return (ParameterDescriptor[])FetchOrCreateDescriptors<ParameterInfo, ParameterDescriptor>(
                ref _parameterCache, method.GetParameters, parameter => {
                    return new ReflectedParameterDescriptor(parameter, this);
                }).Clone();
        }

        public override object Execute(ControllerContext context, IDictionary<string, object> parameters)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            Precondition.Require(parameters, () => Error.ArgumentNull("parameters"));

            object[] parameterValues = _method.GetParameters()
                .Select(p => ExtractParameter(p, parameters, _method))
                .ToArray();

			return DispatcherCache.GetDispatcher(_method).Execute(context.Controller, parameterValues);
        }
        #endregion
    }
}

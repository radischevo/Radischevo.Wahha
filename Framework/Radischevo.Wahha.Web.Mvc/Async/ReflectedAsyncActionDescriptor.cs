using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Core.Async;

namespace Radischevo.Wahha.Web.Mvc.Async
{
	public class ReflectedAsyncActionDescriptor : AsyncActionDescriptor
	{
		#region Static Fields
		private static readonly ActionFilterCache _filterCache = new ActionFilterCache();
		#endregion

		#region Instance Fields
		private readonly object _executeTag;
		private string _name;
		private MethodInfo _entryMethod;
		private MethodInfo _completedMethod;
		private ControllerDescriptor _controller;
		private ParameterDescriptor[] _parameterCache;
		#endregion

		#region Constructors
		public ReflectedAsyncActionDescriptor(MethodInfo entryMethod, MethodInfo completedMethod, 
			string actionName, ControllerDescriptor controller)
			: this(entryMethod, completedMethod, actionName, controller, true)
		{
		}

		internal ReflectedAsyncActionDescriptor(MethodInfo entryMethod, MethodInfo completedMethod, 
			string actionName, ControllerDescriptor controller, bool validateMethods)
		{
			Precondition.Require(entryMethod, () => Error.ArgumentNull("entryMethod"));
			Precondition.Require(completedMethod, () => Error.ArgumentNull("completedMethod"));
			Precondition.Defined(actionName, () => Error.ArgumentNull("actionName"));
			Precondition.Require(controller, () => Error.ArgumentNull("controller"));
			
			if (validateMethods)
			{
				ValidateActionMethod(entryMethod);
				ValidateActionMethod(completedMethod);
			}

			_executeTag = new object();
			_entryMethod = entryMethod;
			_completedMethod = completedMethod;
			_name = actionName;
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
				return _entryMethod;
			}
		}

		public virtual MethodInfo CompletedMethod
		{
			get
			{
				return _completedMethod;
			}
		}
		#endregion

		#region Instance Methods
		public override IEnumerable<ParameterDescriptor> GetParameters()
		{
			MethodInfo method = _entryMethod;
			return (ParameterDescriptor[])FetchOrCreateDescriptors<ParameterInfo, ParameterDescriptor>(
				ref _parameterCache, method.GetParameters, parameter => {
					return new ReflectedParameterDescriptor(parameter, this);
				}).Clone();
		}

		public override IAsyncResult BeginExecute(ControllerContext context,
			IDictionary<string, object> parameters, AsyncCallback callback, object state)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));
			Precondition.Require(parameters, () => Error.ArgumentNull("parameters"));

			AsyncManager asyncManager = GetAsyncManager(context.Controller);

			BeginInvokeDelegate beginDelegate = delegate(AsyncCallback asyncCallback, object asyncState) {
				object[] parameterValues = _entryMethod.GetParameters()
					.Select(p => ExtractParameter(p, parameters, _entryMethod))
					.ToArray();

				TriggerListener listener = new TriggerListener();
				MvcAsyncResult asyncResult = new MvcAsyncResult(asyncState);

				Trigger finishTrigger = listener.CreateTrigger();
				asyncManager.Finished += delegate {
					finishTrigger.Fire();
				};
				asyncManager.OutstandingOperations.Increment();
				listener.SetContinuation(() => ThreadPool.QueueUserWorkItem(_ => 
					asyncResult.MarkCompleted(false, asyncCallback)));

				DispatcherCache.GetDispatcher(_entryMethod)
					.Execute(context.Controller, parameterValues);

				asyncManager.OutstandingOperations.Decrement();
				listener.Activate();
				
				return asyncResult;
			};

			EndInvokeDelegate<object> endDelegate = delegate(IAsyncResult asyncResult) {
				object[] parameterValues = _completedMethod.GetParameters()
					.Select(p => ExtractParameter(p, parameters, _completedMethod))
					.ToArray();

				return DispatcherCache.GetDispatcher(_completedMethod)
					.Execute(context.Controller, parameterValues);
			};

			return AsyncResultWrapper.Begin(callback, state, beginDelegate, 
				endDelegate, _executeTag, asyncManager.Timeout);
		}

		public override object EndExecute(IAsyncResult result)
		{
			return AsyncResultWrapper.End<object>(result, _executeTag);
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return _entryMethod.GetCustomAttributes(inherit);
		}

		public override object[] GetCustomAttributes(Type type, bool inherit)
		{
			return _entryMethod.GetCustomAttributes(type, inherit);
		}

		public override bool IsDefined(Type type, bool inherit)
		{
			return _entryMethod.IsDefined(type, inherit);
		}

		public override IEnumerable<FilterAttribute> GetFilters(bool useCache)
		{
			if (useCache)
				return _filterCache.GetFilters(_entryMethod);
			
			return base.GetFilters(useCache);
		}

		public override IEnumerable<ActionSelector> GetSelectors()
		{
			ActionSelectorAttribute[] attributes = _entryMethod.GetCustomAttributes<ActionSelectorAttribute>(true).ToArray();
			return Array.ConvertAll<ActionSelectorAttribute, ActionSelector>(attributes, attr => {
				return context => attr.IsValid(context, _entryMethod);
			});
		}
		#endregion
	}
}

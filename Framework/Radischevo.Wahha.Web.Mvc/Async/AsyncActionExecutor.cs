using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Core.Async;

namespace Radischevo.Wahha.Web.Mvc.Async
{
	public class AsyncActionExecutor : ActionExecutor, IAsyncActionExecutor
	{
		#region Static Fields
		private static readonly object _invokeTag = new object();
		private static readonly object _invokeMethodTag = new object();
		private static readonly object _invokeMethodFiltersTag = new object();
		#endregion

		#region Constructors
		public AsyncActionExecutor(ControllerContext context)
			: base(context)
		{
		}
		#endregion

		#region Static Methods
		protected static Func<ActionExecutedContext> InvokeActionFilterAsynchronously(IActionFilter filter,
			ActionExecutionContext preContext, Func<Func<ActionExecutedContext>> next)
		{
			filter.OnExecuting(preContext);
			if (preContext.Result != null)
			{
				ActionExecutedContext shortCircuitedPostContext = new ActionExecutedContext(preContext, null) {
					Result = preContext.Result
				};
				return () => shortCircuitedPostContext;
			}
			try
			{
				Func<ActionExecutedContext> continuation = next();
				return () => {
					ActionExecutedContext postContext;
					bool wasError = true;
					try
					{
						postContext = continuation();
						wasError = false;
					}
					catch (ThreadAbortException)
					{
						postContext = new ActionExecutedContext(preContext, null);
						filter.OnExecuted(postContext);

						throw;
					}
					catch (Exception ex)
					{
						postContext = new ActionExecutedContext(preContext, ex);
						filter.OnExecuted(postContext);

						if (!postContext.ExceptionHandled)
							throw;
					}
					if (!wasError)
						filter.OnExecuted(postContext);

					return postContext;
				};
			}
			catch (ThreadAbortException)
			{
				ActionExecutedContext postContext =
					new ActionExecutedContext(preContext, null);

				filter.OnExecuted(postContext);
				throw;
			}
			catch (Exception ex)
			{
				ActionExecutedContext postContext = new ActionExecutedContext(preContext, ex);
				filter.OnExecuted(postContext);
				if (postContext.ExceptionHandled)
					return () => postContext;

				throw;
			}
		}

		private static IAsyncResult ActionNotFound(AsyncCallback callback, object state)
		{
			BeginInvokeDelegate beginDelegate = MakeSynchronousAsyncResult;
			EndInvokeDelegate<bool> endDelegate = delegate(IAsyncResult asyncResult) {
				return false;
			};
			return AsyncResultWrapper.Begin(callback, state, beginDelegate, endDelegate, _invokeTag);
		}

		private static IAsyncResult MakeSynchronousAsyncResult(AsyncCallback callback, object state)
		{
			MvcAsyncResult asyncResult = new MvcAsyncResult(state);
			asyncResult.MarkCompleted(true, callback);

			return asyncResult;
		}
		#endregion

		#region Instance Methods
		public override bool InvokeAction(ControllerContext context,
			string actionName, IDictionary<string, object> values)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));
			Precondition.Defined(actionName, () => Error.ArgumentNull("actionName"));

			IAsyncResult result = BeginInvokeAction(context,
				actionName, null, null, values);
			return EndInvokeAction(result);
		}

		public IAsyncResult BeginInvokeAction(ControllerContext context,
			string actionName, AsyncCallback callback, object state)
		{
			return BeginInvokeAction(context, actionName, callback, state, new ValueDictionary());
		}

		public IAsyncResult BeginInvokeAction(ControllerContext context,
			string actionName, AsyncCallback callback, object state,
			IDictionary<string, object> values)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));
			Precondition.Defined(actionName, () => Error.ArgumentNull("actionName"));

			ControllerDescriptor controller = GetControllerDescriptor(context);
			ActionDescriptor action = controller.FindAction(context, actionName);

			if (action == null)
				return ActionNotFound(callback, state);

			context.Parameters.Merge(GetParameterValues(context, action))
				.Merge(values);

			ActionFilterInfo filters = GetFilters(context, action);
			Action continuation = null;

			BeginInvokeDelegate beginDelegate = delegate(AsyncCallback asyncCallback, object asyncState) {
				try
				{
					AuthorizationContext authContext = InvokeAuthorizationFilters(context, action, filters.AuthorizationFilters);
					if (authContext.Cancel)
					{
						continuation = () => InvokeActionResult(context, authContext.Result ?? EmptyResult.Instance);
					}
					else
					{
						if (context.Controller.ValidateRequest)
							ValidateRequest(context.Context.Request);

						IAsyncResult asyncResult = BeginInvokeActionFilters(context, action,
							filters.ActionFilters, context.Parameters, asyncCallback, asyncState);

						continuation = () => {
							ActionExecutedContext postContext = EndInvokeActionFilters(asyncResult);
							InvokeActionResultFilters(context, filters.ResultFilters, postContext.Result);
						};
						return asyncResult;
					}
				}
				catch (ThreadAbortException)
				{
					throw;
				}
				catch (Exception ex)
				{
					ExceptionContext ctx = InvokeExceptionFilters(context, ex, filters.ExceptionFilters);
					if (!ctx.Handled)
						throw;

					continuation = () => InvokeActionResult(context, ctx.Result);
				}
				return MakeSynchronousAsyncResult(asyncCallback, asyncState);
			};

			EndInvokeDelegate<bool> endDelegate = delegate(IAsyncResult asyncResult) {
				try
				{
					continuation();
				}
				catch (ThreadAbortException)
				{
					throw;
				}
				catch (Exception ex)
				{
					ExceptionContext ctx = InvokeExceptionFilters(context, ex, filters.ExceptionFilters);
					if (!ctx.Handled)
						throw;

					InvokeActionResult(context, ctx.Result);
				}
				return true;
			};
			return AsyncResultWrapper.Begin(callback, state, beginDelegate, endDelegate, _invokeTag);
		}

		public bool EndInvokeAction(IAsyncResult result)
		{
			return AsyncResultWrapper.End<bool>(result, _invokeTag);
		}

		protected override ControllerDescriptor GetControllerDescriptor(ControllerContext context)
		{
			Type type = context.Controller.GetType();
			return Descriptors.GetDescriptor(type, () => new ReflectedAsyncControllerDescriptor(type));
		}

		protected virtual IAsyncResult BeginInvokeActionMethod(ControllerContext context, ActionDescriptor action,
			IDictionary<string, object> parameters, AsyncCallback callback, object state)
		{
			AsyncActionDescriptor asyncAction = (action as AsyncActionDescriptor);
			if (asyncAction != null)
				return BeginInvokeAsynchronousActionMethod(context, asyncAction, parameters, callback, state);

			return BeginInvokeSynchronousActionMethod(context, action, parameters, callback, state);
		}

		protected virtual IAsyncResult BeginInvokeActionFilters(ControllerContext context,
			ActionDescriptor action, ICollection<IActionFilter> filters, IDictionary<string, object> parameters,
			AsyncCallback callback, object state)
		{
			Func<ActionExecutedContext> endContinuation = null;

			BeginInvokeDelegate beginDelegate = delegate(AsyncCallback asyncCallback, object asyncState) {
				ActionExecutionContext preContext = new ActionExecutionContext(context, action);
				IAsyncResult innerAsyncResult = null;

				Func<Func<ActionExecutedContext>> beginContinuation = () => {
					innerAsyncResult = BeginInvokeActionMethod(context, action, parameters, asyncCallback, asyncState);
					return () => new ActionExecutedContext(preContext, null) {
						Result = EndInvokeActionMethod(innerAsyncResult)
					};
				};
				Func<Func<ActionExecutedContext>> thunk = filters.Reverse().Aggregate(beginContinuation,
					(next, filter) => () => InvokeActionFilterAsynchronously(filter, preContext, next));
				endContinuation = thunk();

				if (innerAsyncResult != null)
				{
					return innerAsyncResult;
				}
				else
				{
					MvcAsyncResult newAsyncResult = new MvcAsyncResult(asyncState);
					newAsyncResult.MarkCompleted(true, asyncCallback);

					return newAsyncResult;
				}
			};
			EndInvokeDelegate<ActionExecutedContext> endDelegate = delegate(IAsyncResult asyncResult) {
				return endContinuation();
			};
			return AsyncResultWrapper.Begin(callback, state, beginDelegate, endDelegate, _invokeMethodFiltersTag);
		}

		protected virtual ActionExecutedContext EndInvokeActionFilters(IAsyncResult result)
		{
			return AsyncResultWrapper.End<ActionExecutedContext>(result, _invokeMethodFiltersTag);
		}

		protected virtual ActionResult EndInvokeActionMethod(IAsyncResult result)
		{
			return AsyncResultWrapper.End<ActionResult>(result, _invokeMethodTag);
		}

		private IAsyncResult BeginInvokeAsynchronousActionMethod(ControllerContext context,
			AsyncActionDescriptor action, IDictionary<string, object> parameters,
			AsyncCallback callback, object state)
		{
			BeginInvokeDelegate beginDelegate = delegate(AsyncCallback asyncCallback, object asyncState) {
				return action.BeginExecute(context, parameters, asyncCallback, asyncState);
			};
			EndInvokeDelegate<ActionResult> endDelegate = delegate(IAsyncResult asyncResult) {
				object returnValue = action.EndExecute(asyncResult);
				ActionResult result = CreateActionResult(context, action, returnValue);

				return result;
			};
			return AsyncResultWrapper.Begin(callback, state,
				beginDelegate, endDelegate, _invokeMethodTag);
		}

		private IAsyncResult BeginInvokeSynchronousActionMethod(ControllerContext context,
			ActionDescriptor action, IDictionary<string, object> parameters,
			AsyncCallback callback, object state)
		{
			return AsyncResultWrapper.BeginSynchronous(callback, state,
				() => InvokeSynchronousActionMethod(context, action, parameters),
				_invokeMethodTag);
		}

		private ActionResult InvokeSynchronousActionMethod(ControllerContext context,
			ActionDescriptor action, IDictionary<string, object> parameters)
		{
			return base.InvokeActionMethod(context, action, parameters);
		}
		#endregion
	}
}

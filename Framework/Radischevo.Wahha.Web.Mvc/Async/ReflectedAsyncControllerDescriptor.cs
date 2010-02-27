using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Async
{
	public class ReflectedAsyncControllerDescriptor : ControllerDescriptor
	{
		#region Static Fields
		private static readonly ActionDescriptor[] _emptyActions = new ActionDescriptor[0];
		#endregion

		#region Instance Fields
		private Type _controllerType;
		private AsyncActionMethodSelector _selector;
		#endregion

		#region Constructors
		public ReflectedAsyncControllerDescriptor(Type controllerType)
			: base()
		{
			Precondition.Require(controllerType, () => Error.ArgumentNull("controllerType"));

			_controllerType = controllerType;
			_selector = new AsyncActionMethodSelector(_controllerType);
		}
		#endregion

		#region Instance Properties
		public sealed override Type Type
		{
			get
			{
				return _controllerType;
			}
		}
		#endregion

		#region Instance Methods
		public override ActionDescriptor FindAction(ControllerContext context, string actionName)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));
			Precondition.Defined(actionName, () => Error.ArgumentNull("actionName"));

			ActionDescriptorCreator creator = _selector.GetAsyncDescriptor(context, actionName);
			if (creator == null)
				return null;

			return creator(actionName, this);
		}

		public override IEnumerable<ActionDescriptor> GetCanonicalActions()
		{
			return _emptyActions;
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return _controllerType.GetCustomAttributes(inherit);
		}

		public override object[] GetCustomAttributes(Type type, bool inherit)
		{
			return _controllerType.GetCustomAttributes(type, inherit);
		}

		public override bool IsDefined(Type type, bool inherit)
		{
			return _controllerType.IsDefined(type, inherit);
		}
		#endregion
	}
}

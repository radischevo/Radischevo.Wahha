using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public sealed class ChildContextOperator : IDisposable
	{
		#region Instance Fields
		private ControllerContext _context;
		#endregion

		#region Constructors
		public ChildContextOperator(ControllerContext context)
		{
			Precondition.Require(context, () => 
				Error.ArgumentNull("context"));

			_context = context;
			Init();
		}
		#endregion

		#region Instance Properties
		public ControllerContext Context
		{
			get
			{
				return _context;
			}
		}
		#endregion

		#region Instance Methods
		private void Init()
		{
			int counter = _context.RouteData.GetValue<int>(ControllerContext.ParentCounterKey, 0);
			
			_context.RouteData.Tokens[ControllerContext.ParentContextKey] = _context;
			_context.RouteData.Tokens[ControllerContext.ParentCounterKey] = ++counter;
		}

		public void Dispose()
		{
			int counter = _context.RouteData.Tokens.GetValue<int>(
				ControllerContext.ParentCounterKey, 0);

			if (--counter == 0)
			{
				_context.RouteData.Tokens.Remove(ControllerContext.ParentContextKey);
				_context.RouteData.Tokens.Remove(ControllerContext.ParentCounterKey);
			}
			else
				_context.RouteData.Tokens[ControllerContext.ParentCounterKey] = counter;
		}
		#endregion
	}
}
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
			_context.RouteData.Tokens[ControllerContext.ParentContextKey] = context;
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
		public void Dispose()
		{
			_context.RouteData.Tokens.Remove(ControllerContext.ParentContextKey);
		}
		#endregion
	}
}

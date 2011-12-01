using System;
using System.Collections.Generic;
using System.Linq;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Templates
{
	/// <summary>
	/// Template stacks store a stack of template files. 
	/// The stack can be queried to identify properties of 
	/// the current executing file such as the virtual path of the file.
	/// </summary>
	public static class TemplateStack
	{
		#region Static Fields
		private static readonly object _contextKey = new object();
		#endregion

		#region Static Methods
		public static ITemplateFile CurrentTemplate(HttpContextBase context)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));
			return Stack(context).FirstOrDefault();
		}

		public static ITemplateFile Pop(HttpContextBase context)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));
			return Stack(context).Pop();
		}

		public static void Push(HttpContextBase context, ITemplateFile template)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));
			Precondition.Require(template, () => Error.ArgumentNull("template"));

			Stack(context).Push(template);
		}

		private static Stack<ITemplateFile> Stack(HttpContextBase context)
		{
			Stack<ITemplateFile> stack = (context.Items[_contextKey] as Stack<ITemplateFile>);
			if (stack == null)
			{
				stack = new Stack<ITemplateFile>();
				context.Items[_contextKey] = stack;
			}
			return stack;
		}
		#endregion
	}
}
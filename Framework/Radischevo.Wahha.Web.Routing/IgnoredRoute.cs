using System;
using Radischevo.Wahha.Core;
using System.Collections.Generic;

namespace Radischevo.Wahha.Web.Routing
{
	public sealed class IgnoredRoute : Route
	{
		#region Constructors
		public IgnoredRoute(string url)
			: base(url, new StopRoutingHandler())
		{
		}
		#endregion

		#region Instance Methods
		public override VirtualPathData GetVirtualPath(
			RequestContext context, ValueDictionary values)
		{
			return null;
		}
		#endregion
	}
}

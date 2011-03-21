using System;
using System.Collections.Generic;
using System.IO;

namespace Radischevo.Wahha.Web.Routing.Scripting
{
	public interface IScriptRouteTableWriter
	{
		#region Instance Methods
		void Write(IEnumerable<RouteDefinition> routes, TextWriter output);
		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.IO;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Scripting.Serialization;

namespace Radischevo.Wahha.Web.Routing.Scripting
{
	public class JavaScriptRouteTableWriter : IScriptRouteTableWriter
	{
		#region Instance Fields
		private JavaScriptSerializer _serializer;
		#endregion

		#region Constructors
		public JavaScriptRouteTableWriter()
		{
			_serializer = new JavaScriptSerializer();
		}
		#endregion

		#region Instance Methods
		private void WriteCommonScript(TextWriter output)
		{
			Stream stream = GetType().Assembly.GetManifestResourceStream(
				"Radischevo.Wahha.Web.Routing.Resources.RouteTable.js");
			char[] buffer = new char[4096];
			int count = 0;

			using (StreamReader reader = new StreamReader(stream))
			{
				while ((count = reader.Read(buffer, 0, 4096)) > 0)
					output.Write(buffer, 0, count);
			}
		}

		private void WriteConstraints(IEnumerable<RouteConstraintDefinition> constraints, TextWriter output)
		{
			int count = 0;
			foreach (RouteConstraintDefinition constraint in constraints)
			{
				if (count > 0)
					output.Write(',');

				WriteConstraint(constraint, output);
				count++;
			}
		}

		protected virtual void WriteDefinition(
			RouteDefinition route, TextWriter output)
		{
			output.Write(@"RouteTable.mapRoute(""");
			output.Write(route.Name);
			output.Write(@""",""");
			output.Write(route.Url);
			output.Write(@""",");
			output.Write(_serializer.Serialize(route.Defaults));
			output.Write(",{");
			WriteConstraints(route.Constraints, output);
			output.Write("});");
		}

		protected virtual void WriteConstraint(RouteConstraintDefinition constraint, TextWriter output)
		{
			output.Write(@"""");
			output.Write(constraint.ParameterName);
			output.Write(@""":");
			output.Write(constraint.GetExpression());
		}

		public void Write(IEnumerable<RouteDefinition> routes, TextWriter output)
		{
			Precondition.Require(routes, () => Error.ArgumentNull("routes"));
			Precondition.Require(output, () => Error.ArgumentNull("output"));

			WriteCommonScript(output);

			foreach (RouteDefinition route in routes)
				WriteDefinition(route, output);
		}
		#endregion
	}
}

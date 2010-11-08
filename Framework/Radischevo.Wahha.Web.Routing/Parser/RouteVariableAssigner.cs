using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Routing
{
	internal static class RouteVariableAssigner
	{
		#region Static Methods
		public static void Assign(ParsedRoute route, ValueDictionary variables)
		{
			Precondition.Require(route, () => Error.ArgumentNull("route"));
			Precondition.Require(variables, () => Error.ArgumentNull("variables"));

			foreach (PathSegment segment in route.Segments)
			{
				ContentSegment content = (segment as ContentSegment);
				if (content == null)
					continue;

				VisitSegment(content, variables);
			}
		}

		private static void VisitSegment(ContentSegment segment, ValueDictionary variables)
		{
			foreach (PathSubsegment subsegment in segment.Segments)
			{
				VariableSubsegment variable = (subsegment as VariableSubsegment);
				CompositeSubsegment composite = (subsegment as CompositeSubsegment);

				if (variable != null)
					VisitVariable(variable, variables);

				else if (composite != null)
					VisitComposite(composite, variables);
			}
		}

		private static void VisitComposite(CompositeSubsegment segment, ValueDictionary variables)
		{
			foreach (LiteralSubsegment subsegment in segment.Segments)
			{
				VariableSubsegment variable = (subsegment as VariableSubsegment);
				if (variable == null)
					continue;

				VisitVariable(variable, variables);
			}
		}

		private static void VisitVariable(VariableSubsegment segment, ValueDictionary variables)
		{
			object value;
			if (!variables.TryGetValue(segment.VariableName, out value))
				throw Error.UndefinedRouteVariable(segment.VariableName);

			segment.Value = value;
		}
		#endregion
	}
}

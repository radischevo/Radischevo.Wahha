using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Web.Routing
{
	internal static class LiteralComposer
	{
		#region Static Methods
		public static IList<PathSubsegment> Compose(IList<PathSubsegment> collection)
		{
			if (collection.Count < 2)
				return collection;

			List<PathSubsegment> segments = new List<PathSubsegment>();
			CompositeSubsegment composite = null;

			foreach (PathSubsegment segment in collection)
			{
				LiteralSubsegment literal = (segment as LiteralSubsegment);
				if (literal != null)
				{
					(composite = composite ?? new CompositeSubsegment())
						.Segments.Add(literal);
				}
				else
				{
					if (composite != null)
						segments.Add(composite);

					composite = null;
					segments.Add(segment);
				}
			}
			if (composite != null)
				segments.Add(composite);

			return segments;
		}
		#endregion
	}
}

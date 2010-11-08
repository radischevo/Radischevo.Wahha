using System;
using System.Collections.Generic;
using System.Text;

namespace Radischevo.Wahha.Web.Routing
{
	internal sealed class CompositeSubsegment : LiteralSubsegment
	{
		#region Instance Fields
		private List<LiteralSubsegment> _segments;
		#endregion

		#region Constructors
		public CompositeSubsegment()
			: base("composite")
		{
			_segments = new List<LiteralSubsegment>();
		}
		#endregion

		#region Instance Properties
		public List<LiteralSubsegment> Segments
		{
			get
			{
				return _segments;
			}
		}

		public override string Literal
		{
			get
			{
				return Combine();
			}
		}
		#endregion

		#region Instance Methods
		private string Combine()
		{
			StringBuilder builder = new StringBuilder();
			foreach (LiteralSubsegment segment in _segments)
				builder.Append(segment.Literal);
			
			return builder.ToString();
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			foreach (LiteralSubsegment segment in _segments)
				builder.Append(segment.ToString());

			return builder.ToString();
		}
		#endregion
	}
}

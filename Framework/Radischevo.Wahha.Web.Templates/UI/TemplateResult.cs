using System;
using System.Globalization;
using System.IO;
using System.Web;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Templates
{
	public class TemplateResult
	{
		#region Instance Fields
		private readonly Action<TextWriter> _action;
		#endregion

		#region Constructors
		public TemplateResult(Action<TextWriter> action)
		{
			Precondition.Require(action, () => Error.ArgumentNull("action"));
			_action = action;
		}
		#endregion

		#region Instance Methods
		public override string ToString()
		{
			using (StringWriter writer = 
				new StringWriter(CultureInfo.InvariantCulture))
			{
				_action(writer);
				return writer.ToString();
			}
		}

		public void WriteTo(TextWriter writer)
		{
			_action(writer);
		}
		#endregion
	}
}

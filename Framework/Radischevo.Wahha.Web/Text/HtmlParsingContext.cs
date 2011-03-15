using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Text
{
	public abstract class HtmlParsingContext
	{
		#region Instance Fields
		private IValueSet _parameters;
		#endregion

		#region Constructors
		protected HtmlParsingContext()
			: this(null)
		{
		}

		protected HtmlParsingContext(IValueSet parameters)
		{
			_parameters = parameters;
		}
		#endregion

		#region Instance Properties
		public IValueSet Parameters
		{
			get
			{
				if (_parameters == null)
					_parameters = new ValueDictionary();

				return _parameters;
			}
		}
		#endregion
	}
}

using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Text
{
	public abstract class HtmlParsingContext
	{
		#region Instance Fields
		private IValueSet _parameters;
		private bool _cancelled;
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

		public virtual bool Cancelled
		{
			get
			{
				return _cancelled;
			}
			set
			{
				_cancelled = value;
			}
		}
		#endregion
	}
}

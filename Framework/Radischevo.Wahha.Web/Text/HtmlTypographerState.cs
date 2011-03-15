using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Web.Text
{
	public sealed class HtmlTypographerState
	{
		#region Instance Fields
		private Stack<char> _quotes;
		private bool _withinNoBreak;
		#endregion

		#region Constructors
		public HtmlTypographerState()
		{
			_quotes = new Stack<char>();
		}
		#endregion

		#region Instance Properties
		public Stack<char> Quotes
		{
			get
			{
				return _quotes;
			}
		}

		public bool WithinNoBreak
		{
			get
			{
				return _withinNoBreak;
			}
			set
			{
				_withinNoBreak = value;
			}
		}
		#endregion
	}
}

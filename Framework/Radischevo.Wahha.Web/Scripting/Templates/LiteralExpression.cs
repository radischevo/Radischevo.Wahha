using System;

namespace Radischevo.Wahha.Web.Scripting.Templates
{
	public class LiteralExpression : TemplateExpression
	{
		#region Instance Fields
		private string _literal;
		#endregion

		#region Constructors
		public LiteralExpression(string literal)
			: base(TemplateExpressionType.Literal)
		{
			_literal = literal;
		}
		#endregion

		#region Instance Properties
		public string Literal
		{
			get
			{
				return _literal ?? String.Empty;
			}
			set
			{
				_literal = value;
			}
		}
		#endregion

		#region Instance Methods
		public override string ToString()
		{
			return String.Format("{{ Literal: {0} }}", Literal);
		}
		#endregion
	}
}

using System;

namespace Radischevo.Wahha.Web.Scripting.Templates
{
	public abstract class TemplateExpression
	{
		#region Instance Fields
		private TemplateExpressionType _type;
		#endregion

		#region Constructors
		protected TemplateExpression(TemplateExpressionType type)
		{
			_type = type;
		}
		#endregion

		#region Instance Properties
		public TemplateExpressionType Type
		{
			get
			{
				return _type;
			}
		}
		#endregion
	}
}

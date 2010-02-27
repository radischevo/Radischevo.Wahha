using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Scripting.Templates
{
	public class DirectiveExpression : TemplateExpression
	{
		#region Instance Fields
		private string _name;
		private ParsedAttributeCollection _attributes;
		#endregion

		#region Constructors
		public DirectiveExpression(string directiveName)
			: base(TemplateExpressionType.Directive)
		{
			Precondition.Defined(directiveName,
				() => Error.ArgumentNull("directiveName"));

			_name = directiveName;
			_attributes = new ParsedAttributeCollection();
		}
		#endregion

		#region Instance Properties
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				Precondition.Defined(value, () => Error.ArgumentNull("value"));
				_name = value;
			}
		}

		public ParsedAttributeCollection Attributes
		{
			get
			{
				return _attributes;
			}
		}
		#endregion

		#region Instance Methods
		public override string ToString()
		{
			return String.Format("{{ Directive: {0}, [ {1} ] }}", Name, Attributes);
		}
		#endregion
	}
}

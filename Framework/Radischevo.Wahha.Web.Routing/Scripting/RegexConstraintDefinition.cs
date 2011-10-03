using System;
using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data.Serialization;

namespace Radischevo.Wahha.Web.Routing.Scripting
{
	public class RegexConstraintDefinition : RouteConstraintDefinition
	{
		#region Static Fields
		private static readonly JavaScriptSerializer _serializer = new JavaScriptSerializer(); 
		#endregion

		#region Instance Fields
		private string _pattern;
		#endregion

		#region Constructors
		public RegexConstraintDefinition(RegexConstraint constraint)
			: base(GetConstraint(constraint).ParameterName)
		{
			_pattern = constraint.Pattern.ToString();
		}
		#endregion

		#region Instance Properties
		public string Pattern
		{
			get
			{
				return _pattern;
			}
		}
		#endregion

		#region Static Methods
		private static RegexConstraint GetConstraint(RegexConstraint constraint)
		{
			Precondition.Require(constraint, () => 
				Error.ArgumentNull("constraint"));

			return constraint;
		}
		#endregion

		#region Instance Methods
		public override string GetExpression()
		{
			return _serializer.Serialize(_pattern);
		}
		#endregion
	}
}

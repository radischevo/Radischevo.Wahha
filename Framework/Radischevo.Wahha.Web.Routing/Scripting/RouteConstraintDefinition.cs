using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Routing.Scripting
{
	public abstract class RouteConstraintDefinition
	{
		#region Instance Fields
		private string _parameterName;
		#endregion

		#region Constructors
		protected RouteConstraintDefinition(string parameterName)
		{
			Precondition.Defined(parameterName, () => 
				Error.ArgumentNull("parameterName"));

			_parameterName = parameterName;
		}
		#endregion

		#region Instance Properties
		public string ParameterName
		{
			get
			{
				return _parameterName;
			}
		}
		#endregion

		#region Instance Methods
		public abstract string GetExpression();
		#endregion
	}
}

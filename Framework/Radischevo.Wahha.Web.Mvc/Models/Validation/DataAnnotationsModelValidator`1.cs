using System;
using System.ComponentModel.DataAnnotations;

namespace Radischevo.Wahha.Web.Mvc
{
	public class DataAnnotationsModelValidator<TAttribute> : DataAnnotationsModelValidator
		where TAttribute : ValidationAttribute
	{
		#region Constructors
		public DataAnnotationsModelValidator (TAttribute attribute)
			: base (attribute)
		{
		}
		#endregion
		
		#region Instance Properties
		protected new TAttribute Attribute 
		{
			get
			{
				return (TAttribute)base.Attribute;
			}
		}
		#endregion
	}
}


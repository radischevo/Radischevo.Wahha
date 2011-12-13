using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public class ModelValidationContext
	{
		#region Instance Fields
		private string _member;
		private object _container;
		private object _model;
		#endregion
		
		#region Constructors
		public ModelValidationContext (object model)
			: this (null, model, model)
		{
		}
		
		public ModelValidationContext(string member, object model)
			: this (member, model, model)
		{
		}
		
		public ModelValidationContext(string member, object container, object model)
		{
			Precondition.Require(container, () => Error.ArgumentNull("container"));
			Precondition.Require(model, () => Error.ArgumentNull("model"));
			
			_container = container;
			_member = member;
			_model = model;
		}
		#endregion
	
		#region Instance Properties
		public string Member 
		{
			get
			{
				return _member;
			}
		}
		
		public object Container
		{
			get
			{
				return _container;
			}
		}
		
		public object Model 
		{
			get
			{
				return _model;
			}
		}
		#endregion
	}
}


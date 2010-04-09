using System;
using System.Globalization;
using System.Linq;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Mvc.Configurations;

namespace Radischevo.Wahha.Web.Mvc
{
	public abstract class ModelBinderBase : IModelBinder
	{
		#region Constants
		public const char ValueDelimiter = '-';
		#endregion

		#region Instance Fields
		private string _resourceClassKey;
		private ModelBinderCollection _binders;
		#endregion

		#region Constructors
		protected ModelBinderBase()
		{
		}
		#endregion

		#region Instance Properties
		public string ResourceClassKey
		{
			get
			{
				return _resourceClassKey ?? String.Empty;
			}
			set
			{
				_resourceClassKey = value;
			}
		}

		protected virtual ModelBinderCollection Binders
		{
			get
			{
				if (_binders == null)
					_binders = Configuration.Instance.Models.Binders;

				return _binders;
			}
		}
		#endregion

		#region Static Methods
		private static bool HasBindingData(BindingContext context)
		{
			if(String.IsNullOrEmpty(context.ModelName))
				return (context.Data.Count > 0);

			return context.Contains(context.ModelName);
		}

		protected static string CreateSubMemberName(string prefix, string propertyName)
		{
			if (String.IsNullOrEmpty(prefix))
				return propertyName;

			if (String.IsNullOrEmpty(propertyName))
				return prefix;

			return String.Concat(prefix, ValueDelimiter, propertyName);
		}
		#endregion

		#region Instance Methods
		private string GetResourceString(ControllerContext context, string resourceName)
		{
			if (!String.IsNullOrEmpty(ResourceClassKey) &&
				context != null && context.Context != null)
				return context.Context.GetGlobalResourceObject(ResourceClassKey,
					resourceName, CultureInfo.CurrentUICulture) as string;

			return null;
		}

		private string GetValueInvalidResource(ControllerContext context)
		{
			return GetResourceString(context, "PropertyValueInvalid") ??
				Resources.Resources.Error_BinderValueInvalid;
		}

		private string GetValueRequiredResource(ControllerContext context)
		{
			return GetResourceString(context, "PropertyValueRequired") ??
				Resources.Resources.Error_BinderValueRequired;
		}

		protected virtual object CreateModelInstance(BindingContext context)
		{
			object model = context.Model;
			if (model == null)
			{
				if (context.ModelType.GetConstructor(Type.EmptyTypes) == null)
				{
					context.Errors.Add(context.ModelName,
						Error.MissingParameterlessConstructor(context.ModelType));
					return null;
				}
				else
				{
					model = Activator.CreateInstance(context.ModelType);
				}
			}
			return model;
		}

		protected virtual bool VerifyValueUsability(BindingContext context, string elementKey, 
			Type elementType, object value)
		{
			if (value == null && !elementType.IsNullable() && context.Errors.IsValid(elementKey))
			{
				string message = GetValueRequiredResource(context);
				context.Errors.Add(elementKey, new ValidationError(message, value, null));

				return false;
			}
			return context.Errors.IsValid(elementKey);
		}

		public object Bind(BindingContext context)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));
			if (!HasBindingData(context))
			{
				if (context.FallbackToEmptyPrefix)
					context.ModelName = String.Empty;
				else
					return null;
			}
			return ExecuteBind(context);
		}

		protected abstract object ExecuteBind(BindingContext context);
		#endregion
	}
}

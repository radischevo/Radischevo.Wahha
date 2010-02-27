using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Mvc.UI;
using Radischevo.Wahha.Web.Mvc.Validation;

namespace Radischevo.Wahha.Web.Mvc
{
    public class ValidationHelper
	{
		#region Instance Fields
		private ModelValidatorProviderCollection _providers;
        private ViewContext _context;
        #endregion

        #region Constructors
        public ValidationHelper(ViewContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            _context = context;
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets the current 
        /// <see cref="Radischevo.Wahha.Web.Mvc.ViewContext"/>
        /// </summary>
        public ViewContext Context
        {
            get
            {
                return _context;
            }
        }

        /// <summary>
        /// Gets the <see cref="Radischevo.Wahha.Web.Mvc.ValidationErrorCollection"/> object 
        /// containing the binding validation errors.
        /// </summary>
        public ValidationErrorCollection Errors
        {
            get
            {
                return _context.Controller.Errors;
            }
        }

        public ModelValidatorProviderCollection Providers
        {
            get
            {
                if (_providers == null)
                    _providers = Configuration.Configuration.Instance.Models.ValidatorProviders;

                return _providers;
            }
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Returns a list of validation messages from the 
        /// <see cref="Radischevo.Wahha.Web.Mvc.ValidationErrorCollection">Errors collection</see>, 
        /// using the specified <paramref name="action"/> to render each of them.
        /// </summary>
        /// <param name="action">The action used to render an error message.</param>
        public void Summary(Action<ValidationError> action)
        {
            Precondition.Require(action, () => Error.ArgumentNull("action"));
            
            foreach (ValidationError error in Errors)
                action(error);
        }

        /// <summary>
        /// Displays a validation message if the specified field contains an error in 
        /// the <see cref="Radischevo.Wahha.Web.Mvc.ValidationErrorCollection">Errors collection</see>.
        /// </summary>
        /// <param name="key">The name of the field.</param>
        /// <param name="action">The action used to render an error message.</param>
        public void Messages(string key, Action<IEnumerable<ValidationError>> action)
        {
            Precondition.Require(action, () => Error.ArgumentNull("action"));
			IEnumerable<ValidationError> errors = Errors[key];

			if(errors != null && errors.Any())
				action(errors);
        }

		/// <summary>
		/// Displays a validation message if the specified field contains an error in 
		/// the <see cref="Radischevo.Wahha.Web.Mvc.ValidationErrorCollection">Errors collection</see>.
		/// </summary>
		/// <param name="key">The name of the field.</param>
		/// <param name="action">The action used to render an error message.</param>
		public void Message(string key,
			Action<ValidationError> action)
		{
			Precondition.Require(action, () => Error.ArgumentNull("action"));
			foreach (ValidationError error in Errors[key])
				action(error);
		}

		/// <summary>
		/// Gets the value indicating whether the 
		/// <see cref="Radischevo.Wahha.Web.Mvc.ValidationErrorCollection"/> 
		/// contains no errors.
		/// </summary>
		public bool Valid()
		{
			return Errors.IsValid();
		}

        /// <summary>
        /// Gets the value indicating whether the
        /// <see cref="Radischevo.Wahha.Web.Mvc.ValidationErrorCollection">Errors collection</see> 
        /// contains an error with the specified field name.
        /// </summary>
        /// <param name="key">The name of the field.</param>
        public bool Valid(string key)
        {
			return Errors.IsValid(key);
        }
        #endregion

        #region Helper Methods
        protected ModelValidatorSet GetValidatorsFromExpression(string expression)
        {
			if (String.IsNullOrEmpty(expression))
				return new ModelValidatorSet(Context.ViewData.Validator);
			
            ViewDataInfo vdi = Context.ViewData.GetViewDataInfo(expression);
			if (vdi == null)
				return new ModelValidatorSet();

            Type containerType = null;
            Type modelType = null;
            string propertyName = null;
            object model = vdi.Value;

            if (vdi.Container != null)
                containerType = vdi.Container.GetType();

            if (vdi.Descriptor != null)
            {
                propertyName = vdi.Descriptor.Name;
                modelType = vdi.Descriptor.PropertyType;
            }

            if (model != null && modelType == null)
                modelType = model.GetType();

            return GetValidatorsFromProvider(containerType, modelType ?? typeof(string), propertyName);
        }

		protected ModelValidatorSet GetValidatorsFromExpression<TModel, TValue>(
			Expression<Func<TModel, TValue>> expression)
			where TModel : class
		{
			Precondition.Require(expression, () => Error.ArgumentNull("expression"));

			Type modelType = typeof(TValue);
			Type parentModelType = null;
			string propertyName = null;

			switch (expression.Body.NodeType)
			{
				case ExpressionType.Parameter:
					break;

				case ExpressionType.MemberAccess:
					MemberExpression memberExpression = (MemberExpression)expression.Body;
					propertyName = (memberExpression.Member is PropertyInfo) ? memberExpression.Member.Name : null;
					parentModelType = memberExpression.Member.DeclaringType;
					break;

				default:
					throw Error.TemplateExpressionLimitations();
			}
			return GetValidatorsFromProvider(parentModelType, modelType, propertyName);
		}

        protected virtual ModelValidatorSet GetValidatorsFromProvider(Type containerType,
            Type modelType, string propertyName)
        {
			ModelValidator model = Providers.GetProvider(modelType).GetValidator(modelType);
			ModelValidator container = (containerType == null || String.IsNullOrEmpty(propertyName))
				? null : Providers.GetProvider(containerType).GetValidator(containerType)
					.GetPropertyValidator(propertyName);

			return new ModelValidatorSet(model, container);
        }

        protected virtual IEnumerable<ClientModelValidationRule> CreateClientRules(
			IEnumerable<ModelValidationRule> rules)
		{
			Precondition.Require(rules, () => Error.ArgumentNull("rules"));

			return rules.Where(r => r.SupportsClientValidation)
				.Select(r => new ClientModelValidationRule(r));
		}

		private IEnumerable<ModelValidationRule> GetRulesInternal(string expression)
		{
			return GetValidatorsFromExpression(expression).GetValidationRules();
		}

		protected static string CreateFieldName(TemplateDescriptor descriptor, 
			string expression, string member)
		{
			return (String.Equals(expression, member, StringComparison.OrdinalIgnoreCase))
				? descriptor.Prefix
				: descriptor.GetHtmlElementName(member);
		}
        #endregion

        #region Client Validation
        /// <summary>
        /// Gets the collection of client validation rules, 
        /// which are applied to the current model.
        /// </summary>
        public FormValidationMetadata Rules()
        {
            return Rules(null, null);
        }

        /// <summary>
        /// Gets the collection of client validation rules, 
        /// which are applied to the current model.
        /// </summary>
        /// <param name="modelName">Specifies the name of the model, 
        /// which will prefix the validation rule member name.</param>
        public FormValidationMetadata Rules(string modelName)
        {
            return Rules(modelName, null);
        }

        /// <summary>
        /// Gets the collection of client validation rules, 
        /// which are applied to the property of the 
        /// model, accessed through the specified <paramref name="expression"/>.
        /// </summary>
        /// <param name="modelName">Specifies the name of the model, 
        /// which will prefix the validation rule member name.</param>
        /// <param name="expression">A string expression to access the 
        /// model property.</param>
        public FormValidationMetadata Rules(string modelName, string expression)
        {
            TemplateDescriptor descriptor = new TemplateDescriptor(
                Context.ViewData.Template.Type) { Prefix = modelName };
            descriptor.Prefix = descriptor.GetHtmlElementName(expression);
			
            return new FormValidationMetadata(
                CreateClientRules(GetRulesInternal(expression))
					.Each(r => {
						r.Field = CreateFieldName(descriptor, expression, r.Field);
					}));
        }
        #endregion
    }

    public class ValidationHelper<TModel> : ValidationHelper
        where TModel : class
    {
        #region Constructors
        public ValidationHelper(ViewContext context)
            : base(context)
        {
        }
        #endregion

        #region Instance Methods
		private IEnumerable<ModelValidationRule> GetRulesInternal<TValue>(
			Expression<Func<TModel, TValue>> expression)
		{
			return GetValidatorsFromExpression(expression).GetValidationRules();
		}

        /// <summary>
        /// Gets the collection of client validation rules, 
        /// which are applied to the property of the 
        /// model, accessed through the specified <paramref name="expression"/>.
        /// </summary>
        /// <param name="expression">A lambda expression accessor for the 
        /// model property.</param>
        public FormValidationMetadata Rules<TValue>(
            Expression<Func<TModel, TValue>> expression)
        {
            return Rules(null, expression);
        }

        /// <summary>
        /// Gets the collection of client validation rules, 
        /// which are applied to the property of the 
        /// model, accessed through the specified <paramref name="expression"/>.
        /// </summary>
        /// <param name="modelName">Specifies the name of the model, 
        /// which will prefix the validation rule member name.</param>
        /// <param name="expression">A lambda expression accessor for the 
        /// model property.</param>
        public FormValidationMetadata Rules<TValue>(string modelName, 
            Expression<Func<TModel, TValue>> expression)
        {
            TemplateDescriptor descriptor = new TemplateDescriptor(
                Context.ViewData.Template.Type) { Prefix = modelName };

			string stringExpression = LinqHelper.GetExpressionText(expression);
            descriptor.Prefix = descriptor.GetHtmlElementName(stringExpression);

			return new FormValidationMetadata(
                CreateClientRules(GetRulesInternal(expression))
                    .Each(r => {
						r.Field = CreateFieldName(descriptor, stringExpression, r.Field);
                    }));
        }
        #endregion
    }
}
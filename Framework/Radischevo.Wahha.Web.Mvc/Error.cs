using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Runtime.Serialization;

using Res = Radischevo.Wahha.Web.Mvc.Resources.Resources;

namespace Radischevo.Wahha.Web.Mvc
{
    internal static class Error
    {
        internal static Exception ArgumentNull(string argumentName)
        {
            return new ArgumentNullException(argumentName);
        }

        internal static Exception InvalidArgument(string parameter)
        {
            return new ArgumentException(parameter);
        }

        internal static Exception ArgumentOutOfRange(string parameterName)
        {
            return new ArgumentOutOfRangeException(parameterName);
        }

        internal static Exception TypeMustDeriveFromType(Type type, Type baseType)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_TypeMustDeriveFromType,
                type.FullName, baseType.FullName));
        }

        internal static Exception InvalidMvcAction(Type controller, string actionName)
        {
            return new InvalidOperationException(
                String.Format(Res.Error_InvalidMvcAction, actionName, controller.Name));
        }

        internal static Exception MvcActionCannotBeGeneric(MethodInfo method)
        {
            return new NotImplementedException(
                String.Format(Res.Error_MvcActionCannotBeGeneric, 
                    method.Name, method.ReflectedType.FullName));
        }

        internal static Exception MissingParameterlessConstructor(Type type)
        {
            return new MissingMethodException(String.Format(
                Res.Error_MissingParameterlessConstructor, type.FullName));
        }

        internal static Exception InvalidControllerType(string controllerName)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_InvalidControllerType,
                controllerName, typeof(IController).Name));
        }

        internal static Exception ControllerMustHaveDefaultConstructor(Type type)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_ControllerMustHaveDefaultConstructor, type.FullName));
        }

        internal static Exception TargetMustSubclassController(string controllerName)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_TargetMustSubclassController,
                controllerName, typeof(Controller).Name));
        }

        internal static Exception ExpressionMustBeAMethodCall(string parameterName)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_ExpressionMustBeAMethodCall, parameterName));
        }

        internal static Exception MethodCallMustTargetLambdaArgument(string parameterName)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_MethodCallMustTargetLambdaArgument, parameterName));
        }

        internal static Exception ReferenceActionParametersNotSupported(MethodInfo method, string parameterName)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_ReferenceActionParametersNotSupported,
                parameterName, method.Name, method.ReflectedType.FullName));
        }

        internal static Exception MissingActionParameter(MethodInfo action, ParameterInfo parameter)
        {
            return new InvalidOperationException(
                String.Format(Res.Error_MissingActionParameter,
                parameter.Name, parameter.ParameterType.Name, action.Name, action.ReflectedType.Name));
        }

        internal static Exception IncompatibleControllerFactoryType(Type type)
        {
            return new ArgumentException(String.Format(
                Res.Error_IncompatibleControllerFactoryType, type.Name));
        }

		internal static Exception IncompatibleControllerActivatorType(Type type)
		{
			return new ArgumentException(String.Format(
				Res.Error_IncompatibleControllerActivatorType, type.Name));
		}

        internal static Exception DuplicateControllerName(string name)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_DuplicateControllerName, name));
        }

        internal static Exception ViewNotFound(string viewName)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_ViewNotFound, viewName));
        }

        internal static Exception CouldNotCreateView(string location)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_CouldNotCreateView, location));
        }

        internal static Exception CouldNotCreateController(string controllerName)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_CouldNotCreateController, controllerName));
        }

        internal static Exception ViewLocationFormatsAreEmpty()
        {
            return new InvalidOperationException(Res.Error_ViewLocationFormatsAreEmpty);
        }

        internal static Exception InvalidViewDataType(Type viewDataType, Type type)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_InvalidViewDataType,
                type.Name, viewDataType.Name));
        }

        internal static Exception SessionStateDisabled()
        {
            return new InvalidOperationException(
                Res.Error_SessionStateDisabled);
        }

        internal static Exception AmbiguousActionName(Type controllerType, string methodName)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_AmbiguousActionName, methodName, controllerType.Name));
        }

        internal static Exception MultipleModelBinderAttributes(Type type)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_MultipleModelBinderAttributes, type.FullName));
        }

        internal static Exception MultipleModelBinderAttributes(ParameterInfo parameter)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_MultipleModelBinderAttributes_Parameter, parameter.Name));
        }

        internal static Exception UnsupportedDictionaryType(Type type)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_UnsupportedDictionaryType, type.FullName));
        }

        internal static Exception UnableToLoadConfiguration(Exception inner)
        {
            return new InvalidOperationException(
                Res.Error_UnableToLoadConfiguration, inner);
        }

        internal static Exception IncompatibleViewEngineType(Type type)
        {
            return new ArgumentException(String.Format(
                Res.Error_IncompatibleViewEngineType, type.Name));
        }

        internal static Exception CannotCallInstanceMethodOnNonControllerType(MethodInfo method)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_CannotCallInstanceMethodOnNonControllerType, 
                method.Name, method.ReflectedType.FullName));
        }

        internal static Exception ParameterCannotBeNull(MethodInfo action, ParameterInfo parameter)
        {
            return new ArgumentException(String.Format(
                Res.Error_ParameterCannotBeNull,
                parameter.Name, action.Name));
        }

        internal static Exception ParameterValueHasWrongType(MethodInfo action, ParameterInfo parameter)
        {
            return new ArgumentException(
                String.Format(Res.Error_ParameterValueHasWrongType,
                parameter.Name, action.Name, action.ReflectedType.Name, parameter.ParameterType.Name));
        }

        internal static Exception IncompatibleModelBinderType(Type type)
        {
            return new ArgumentException(String.Format(
                Res.Error_IncompatibleModelBinderType, type.Name));
        }

        internal static Exception RequestValidationError()
        {
            return new RequestValidationException(Res.Error_RequestValidationError);
        }

        internal static Exception InvalidReferrerUrl(Uri referrer)
        {
            return new HttpException((int)HttpStatusCode.BadRequest, String.Format(
				Res.Error_InvalidReferrerUrl, (referrer == null) ? String.Empty : referrer.OriginalString));
        }

        internal static Exception RequestValidationError(Exception inner)
        {
            return new RequestValidationException(Res.Error_RequestValidationError, inner);
        }

        internal static Exception UnsupportedModelType(Type type)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_UnsupportedModelType, type.FullName));
        }

        internal static Exception TemplateExpressionLimitations()
        {
            return new InvalidOperationException(
				Res.Error_TemplateExpressionLimitations);
        }

        internal static Exception MatchingRouteCouldNotBeLocated()
        {
            return new InvalidOperationException(Res.Error_MatchingRouteCouldNotBeLocated);
        }

        internal static Exception PropertyNotFound(Type type, string propertyName)
        {
            return new InvalidOperationException(String.Format(
				Res.Error_PropertyNotFound, type.FullName, propertyName));
        }

        internal static Exception UnknownProperty(Type type, string propertyName)
        {
            return new InvalidOperationException(String.Format(
				Res.Error_UnknownProperty, type.FullName, propertyName));
        }

        internal static Exception UnreadableProperty(Type type, string propertyName)
        {
            return new InvalidOperationException(String.Format(
				Res.Error_UnreadableProperty, type.FullName, propertyName));
        }

        internal static Exception IncompatibleModelValidatorProviderType(Type type)
        {
            return new ArgumentException(String.Format(
                Res.Error_IncompatibleModelValidatorProviderType, type.Name));
        }

        internal static Exception IncompatibleModelMetadataProviderType(Type type)
        {
            return new ArgumentException(String.Format(
                Res.Error_IncompatibleModelMetadataProviderType, type.Name));
        }

        internal static Exception InvalidDataAnnotationsValidationRuleConstructor(Type adapterType, 
            Type validatorType, Type attributeType)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_InvalidDataAnnotationsValidationRuleConstructor,
                adapterType.FullName, validatorType.FullName, attributeType.FullName));
        }

		internal static Exception ChildRequestExecutionError(HttpException ex)
		{
			return new HttpException(500, Res.Error_ChildRequestExecutionError, ex);
		}

		internal static Exception ControllerCannotHandleMultipleRequests(Type type)
		{
			return new InvalidOperationException(String.Format(
				Res.Error_ControllerCannotHandleMultipleRequests,
				type.FullName));
		}

		internal static Exception ControllerMustImplementAsyncManagerContainer(Type type)
		{
			return new InvalidOperationException(String.Format(
				Res.Error_ControllerMustImplementAsyncManagerContainer, type.FullName));
		}

		internal static Exception InvalidTimeout(string parameterName)
		{
			return new ArgumentOutOfRangeException(parameterName, 
				Res.Error_InvalidTimeout);
		}

		internal static Exception SynchronizationContextError(Exception error)
		{
			return new InvalidOperationException(
				Res.Error_SynchronizationContextError, error);
		}

		internal static Exception InvalidAsyncResult(string parameterName)
		{
			return new ArgumentException(
				Res.Error_InvalidAsyncResult, parameterName);
		}

		internal static Exception AsyncResultAlreadyConsumed()
		{
			return new InvalidOperationException(
				Res.Error_AsyncResultAlreadyConsumed);
		}

		internal static Exception AsyncTimeout()
		{
			return new TimeoutException();
		}

		internal static Exception CannotRedirectFromChildAction()
		{
			return new InvalidOperationException(
				Res.Error_CannotRedirectFromChildAction);
		}

		internal static Exception CouldNotFindAsyncCompletionMethod(Type type, 
			string completionMethodName)
		{
			return new InvalidOperationException(String.Format(
				Res.Error_CouldNotFindAsyncCompletionMethod, completionMethodName, type.FullName));
		}

		internal static Exception SecureConnectionRequired()
		{
			throw new InvalidOperationException(Res.Error_SecureConnectionRequired);
		}

		internal static Exception CollectionTypeMustBeEnumerable(Type type)
		{
			return new InvalidOperationException(String.Format(
				Res.Error_TypeMustImplementIEnumerable, type.FullName));
		}

		internal static Exception InvalidIndexerExpression(Expression expression, ParameterExpression parameter)
		{
			return new InvalidOperationException(String.Format(
				Res.Error_InvalidIndexerExpression, 
				expression, parameter.Name));
		}

		internal static Exception HttpPostedFileSetTypeLimitations(Type type)
		{
			return new InvalidOperationException(String.Format(
				Res.Error_HttpPostedFileSetTypeLimitations, type.FullName));
		}

		internal static Exception TypeArgumentCountMismatch(Type modelType, Type binderType)
		{
			return new ArgumentException(String.Format(Res.Error_TypeArgumentCountMismatch,
				modelType.FullName, modelType.GetGenericArguments().Length, 
				binderType.FullName, binderType.GetGenericArguments().Length));
		}

		internal static Exception CouldNotDeserializeModelState(Exception inner)
		{
			return new SerializationException(
				Res.Error_CouldNotDeserializeModelState, inner);
		}

		internal static Exception InvalidModelStateSerializationMode(SerializationMode mode)
		{
			return new ArgumentOutOfRangeException("mode", String.Format(
				Res.Error_InvalidModelStateSerializationMode, mode));
		}

		internal static Exception MustOverrideGetBinderToUseEmptyType()
		{
			return new InvalidOperationException(Res.Error_MustOverrideGetBinderToUseEmptyType);
		}

		internal static Exception IncompatibleValueProviderFactoryType(Type type)
		{
			return new ArgumentException(String.Format(
				Res.Error_IncompatibleValueProviderFactoryType, type.Name));
		}

		internal static Exception IncompatibleFilterProviderType(Type type)
		{
			return new ArgumentException(String.Format(
				Res.Error_IncompatibleFilterProviderType, type.Name));
		}

		internal static Exception CannotExecuteResultInChildAction()
		{
			return new InvalidOperationException(
				Res.Error_CannotExecuteResultInChildAction);
		}
	}
}
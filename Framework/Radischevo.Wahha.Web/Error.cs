using System;
using Radischevo.Wahha.Web.Text.Sgml;
using System.Globalization;
using System.Web;

namespace Radischevo.Wahha.Web
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

        internal static Exception ParameterMustBeGreaterThan(string parameterName, object lowerBound, object actualValue)
        {
            return new ArgumentOutOfRangeException(parameterName, actualValue,
                String.Format(Resources.Resources.Error_ParameterMustBeGreaterThan, parameterName, lowerBound));
        }

        internal static Exception ParameterMustBeGreaterThanOrEqual(string parameterName, object lowerBound, object actualValue)
        {
            return new ArgumentOutOfRangeException(parameterName, actualValue,
                String.Format(Resources.Resources.Error_ParameterMustBeGreaterThanOrEqual, parameterName, lowerBound));
        }

        internal static Exception InvalidAttributeDefinitionType(AttributeType type)
        {
            return new ArgumentException(String.Format(CultureInfo.CurrentUICulture,
                Resources.Resources.Error_InvalidAttributeDefinitionType, type));
        }

        internal static Exception InvalidAttributeDefinitionPresenceValue(string value)
        {
            return new SgmlParseException(String.Format(CultureInfo.CurrentUICulture,
                Resources.Resources.Error_InvalidAttributeDefinitionPresenceValue, value));
        }

        internal static Exception InvalidAttributeDefinitionTypeValue(string value)
        {
            return new SgmlParseException(String.Format(
                Resources.Resources.Error_InvalidAttributeDefinitionTypeValue, value));
        }

        internal static Exception InvalidDeclaredContentTypeValue(string value)
        {
            return new SgmlParseException(String.Format(
                Resources.Resources.Error_InvalidDeclaredContentTypeValue, value));
        }

        internal static Exception InvalidLiteralTypeValue(string value)
        {
            return new SgmlParseException(String.Format(
                Resources.Resources.Error_InvalidLiteralTypeValue, value));
        }

        internal static Exception UnresolvableSgmlEntity(string name)
        {
            return new ArgumentException(String.Format(
                Resources.Resources.Error_UnresolvableSgmlEntity, name));
        }

        internal static Exception InvalidNameStartCharacter(char ch)
        {
            return new SgmlParseException(String.Format(CultureInfo.CurrentUICulture, 
                Resources.Resources.Error_InvalidNameStartCharacter, ch));
        }

        internal static Exception InvalidNameCharacter(char ch)
        {
            return new SgmlParseException(String.Format(CultureInfo.CurrentUICulture,
                Resources.Resources.Error_InvalidNameCharacter, ch));
        }

        internal static Exception UnclosedElement(string element, int line)
        {
            return new ArgumentException(String.Format(
                Resources.Resources.Error_UnclosedElement, element, line));
        }

        internal static Exception CouldNotParseEntityReference(char ch)
        {
            return new ArgumentException(String.Format(
                Resources.Resources.Error_CouldNotParseEntityReference, ch));
        }

        internal static Exception MissingTokenBeforeConnector(char ch)
        {
            return new SgmlParseException(String.Format(
                Resources.Resources.Error_MissingTokenBeforeConnector, ch));
        }

        internal static Exception InconsistentConnector(char ch, GroupType type)
        {
            return new SgmlParseException(String.Format(
                Resources.Resources.Error_InconsistentConnector, ch, type.ToString()));
        }

        internal static Exception UnexpectedCharacter(char ch)
        {
            return new SgmlParseException(String.Format(
                Resources.Resources.Error_UnexpectedCharacter, ch));
        }

        internal static Exception ExpectingDeclaration(char ch)
        {
            return new SgmlParseException(String.Format(
                Resources.Resources.Error_ExpectingDeclaration, ch));
        }

        internal static Exception ExpectingComment(char ch)
        {
            return new SgmlParseException(String.Format(
                Resources.Resources.Error_ExpectingComment, ch));
        }

        internal static Exception ExpectingCommentDelimiter(char ch)
        {
            return new SgmlParseException(String.Format(
                Resources.Resources.Error_ExpectingCommentDelimiter, ch));
        }

        internal static Exception InvalidDeclaration(string token)
        {
            return new SgmlParseException(String.Format(
                Resources.Resources.Error_InvalidDeclaration, token));
        }

        internal static Exception InvalidMarkedSectionType(string name)
        {
            return new SgmlParseException(String.Format(
                Resources.Resources.Error_InvalidMarkedSectionType, name));
        }

        internal static Exception CouldNotParseConditionalSection(char ch)
        {
            return new SgmlParseException(String.Format(
                Resources.Resources.Error_CouldNotParseConditionalSection, ch));
        }

        internal static Exception UnsupportedExternalParameterEntityResolution()
        {
            return new NotSupportedException(Resources.Resources
                .Error_UnsupportedExternalParameterEntityResolution);
        }

        internal static Exception UndefinedParameterEntityReference(string name)
        {
            return new SgmlParseException(String.Format(
                Resources.Resources.Error_UndefinedParameterEntityReference, name));
        }

        internal static Exception ExpectingPublicIdentifierLiteral(char ch)
        {
            return new SgmlParseException(String.Format(
                Resources.Resources.Error_ExpectingPublicIdentifierLiteral, ch));
        }

        internal static Exception ExpectingSystemIdentifierLiteral(char ch)
        {
            return new SgmlParseException(String.Format(
                Resources.Resources.Error_ExpectingSystemIdentifierLiteral, ch));
        }

        internal static Exception InvalidExternalIdentifierLiteral(string value)
        {
            return new SgmlParseException(String.Format(
                Resources.Resources.Error_InvalidExternalIdentifierLiteral, value));
        }

        internal static Exception ExpectingEndOfEntityDeclaration(char ch)
        {
            return new SgmlParseException(String.Format(
                Resources.Resources.Error_ExpectingEndOfEntityDeclaration, ch));
        }

        internal static Exception InvalidDeclarationSyntax(char ch)
        {
            return new SgmlParseException(String.Format(
                Resources.Resources.Error_InvalidDeclarationSyntax, ch));
        }

        internal static Exception ExpectingInclusionsNameGroup(char ch)
        {
            return new SgmlParseException(String.Format(
                Resources.Resources.Error_ExpectingInclusionsNameGroup, ch));
        }

        internal static Exception ExpectingEndOfElementDeclaration(char ch)
        {
            return new SgmlParseException(String.Format(
                Resources.Resources.Error_ExpectingEndOfElementDeclaration, ch));
        }

        internal static Exception ContentModelWasNotClosed()
        {
            return new SgmlParseException(
                Resources.Resources.Error_ContentModelWasNotClosed);
        }

        internal static Exception ParameterEntityClosedOutsideTheScope()
        {
            return new SgmlParseException(Resources.Resources
                .Error_ParameterEntityClosedOutsideTheScope);
        }

        internal static Exception AttributeListReferencesToAnUndefinedElement(string name)
        {
            return new SgmlParseException(String.Format(
                Resources.Resources.Error_AttributeListReferencesToAnUndefinedElement, name));
        }

        internal static Exception ExpectingNameGroup(char ch)
        {
            return new SgmlParseException(String.Format(
                Resources.Resources.Error_ExpectingNameGroup, ch));
        }

        internal static Exception NotOnAnEntityReference()
        {
            return new InvalidOperationException(Resources.Resources
                .Error_NotOnAnEntityReference);
        }

        internal static Exception NotOnAnAttribute()
        {
            return new InvalidOperationException(Resources.Resources
                .Error_NotOnAnAttribute);
        }

        internal static Exception InputStreamIsUndefined()
        {
            return new InvalidOperationException(Resources.Resources
                .Error_InputStreamIsUndefined);
        }

        internal static Exception UnexpectedEndOfFileParsingStartTag(string name)
        {
            return new SgmlParseException(String.Format(
                Resources.Resources.Error_UnexpectedEndOfFileParsingStartTag, name));
        }

        internal static Exception DtdDoesNotMatchDocumentType()
        {
            return new InvalidOperationException(Resources.Resources
                .Error_DtdDoesNotMatchDocumentType);
        }

        internal static Exception InvalidCharacterInEncoding(uint code)
        {
            return new ArgumentException(String.Format(
                Resources.Resources.Error_InvalidCharacterInEncoding, code.ToString("x")));
        }

        internal static Exception UnsupportedElementRule(string paramName)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_UnsupportedElementRule, paramName));
        }

		internal static Exception TemplateDirectiveCannotBeEmpty(string path, string code, int lineNumber)
		{
			return new HttpParseException(Resources.Resources
				.Error_TemplateDirectiveCannotBeEmpty, null, path, code, lineNumber);
		}

		internal static Exception DuplicateMainDirective(string name, string path, string code, int lineNumber)
		{
			return new HttpParseException(String.Format(
				Resources.Resources.Error_DuplicateMainDirective, name), null, path, code, lineNumber);
		}

		internal static Exception CouldNotParseNestedTemplate(Exception inner, string path, int lineNumber)
		{
			return new HttpParseException(Resources.Resources
				.Error_CouldNotParseNestedTemplate, inner, path, null, lineNumber);
		}

		internal static Exception EmptyCodeRenderExpression(string path, string code, int lineNumber)
		{
			return new HttpParseException(Resources.Resources.Error_EmptyCodeRenderExpression, 
				null, path, code, lineNumber);
		}

		internal static Exception RuleNameCannotDifferFromKey(string key, string actualValue)
		{
			return new ArgumentException(String.Format(Resources.Resources
				.Error_RuleNameCannotDifferFromKey, key, actualValue));
		}
		
		internal static Exception ObjectDisposed(string objectName)
		{
			return new ObjectDisposedException(objectName);
		}
	}
}

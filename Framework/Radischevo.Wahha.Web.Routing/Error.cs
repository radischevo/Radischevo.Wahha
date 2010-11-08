using System;
using Res = Radischevo.Wahha.Web.Routing.Resources.Resources;

namespace Radischevo.Wahha.Web.Routing
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

        internal static Exception IncompatibleRouteTableProvider(Type providerType)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_IncompatibleRouteTableProvider,
                providerType.Name, typeof(IRouteTableProvider).Name));
        }

        internal static Exception NoRouteHandlerFound()
        {
            return new InvalidOperationException(
                Res.Error_NoRouteHandlerFound);
        }

        internal static Exception NoHttpHandlerFound(Type handlerType)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_NoHttpHandlerFound, handlerType.FullName));
        }

        internal static Exception PersistenceProviderNotInitialized()
        {
            return new InvalidOperationException(
                Res.Error_PersistenceProviderNotInitialized);
        }

        internal static Exception InvalidRouteHandlerType(Type handlerType)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_InvalidRouteHandlerType,
                handlerType.Name, typeof(IRouteHandler).Name));
        }

        internal static Exception InvalidRouteParameterName(string parameterName)
        {
            return new ArgumentException(String.Format(
                Res.Error_InvalidRouteParameterName, parameterName));
        }

		internal static Exception InvalidRouteVariableName(string variableName)
		{
			return new ArgumentException(String.Format(
				Res.Error_InvalidRouteVariableName, variableName));
		}

        internal static Exception DuplicateRouteParameterName(string parameterName)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_DuplicateRouteParameterName, parameterName));
        }

		internal static Exception UndefinedRouteVariable(string name)
		{
			return new ArgumentException(String.Format(
				Res.Error_UndefinedRouteVariable, name));
		}

        internal static Exception RequiredValueNotFound(string key)
        {
            return new ArgumentException(String.Format(
                Res.Error_RequiredValueNotFound, key));
        }

        internal static Exception UnexpectedSymbolInRoute(char symbol, string segment)
        {
            return new ArgumentException(String.Format(
                Res.Error_UnexpectedSymbolInRoute, symbol, segment), "url");
        }

		internal static Exception IncompleteEscapeSequenceInRoute(string segment)
		{
			return new ArgumentException(String.Format(
				Res.Error_IncompleteEscapeSequenceInRoute, segment), "url");
		}

        internal static Exception ConsecutiveRouteParameters()
        {
            return new ArgumentException(
                Res.Error_ConsecutiveRouteParameters, "url");
        }

        internal static Exception CatchAllInMultiSegment()
        {
            return new ArgumentException(
                Res.Error_CatchAllInMultiSegment, "url");
        }

        internal static Exception CatchAllMustBeLast()
        {
            return new ArgumentException(
                Res.Error_CatchAllMustBeLast, "url");
        }

        internal static Exception ConsecutiveSeparators()
        {
            return new ArgumentException(
                Res.Error_ConsecutiveSeparators, "url");
        }

        internal static Exception NoRouteMatched()
        {
            return new InvalidOperationException(
                Res.Error_NoRouteMatched);
        }

        internal static Exception ConstraintTypeCannotBeEmpty()
        {
            return new InvalidOperationException(
				Res.Error_ConstraintTypeCannotBeEmpty);
        }

        internal static Exception CouldNotCreateHttpHandler(Type type)
        {
            return new InvalidOperationException(String.Format(
                Res.Error_CouldNotInstantiateHttpHandler, type.FullName));
        }

        internal static Exception MatchingRouteCouldNotBeLocated()
        {
            return new InvalidOperationException(
				Res.Error_MatchingRouteCouldNotBeLocated);
        }

        internal static Exception RoutableControlRequiresRoutablePage()
        {
            return new InvalidOperationException(
                Res.Error_RoutableControlRequiresRoutablePage);
        }
    }
}

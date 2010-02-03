using System;

namespace Radischevo.Wahha.Core.Expressions
{
    internal delegate TMember CompiledExpressionDelegate<TClass, TMember>(TClass instance, object[] hoistedValues);
}

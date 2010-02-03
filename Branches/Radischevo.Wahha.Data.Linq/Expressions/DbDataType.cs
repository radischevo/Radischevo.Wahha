using System;
using System.Data;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public abstract class DbDataType
    {
        public abstract DbType DbType { get; }
        public abstract bool NotNull { get; }
        public abstract int Length { get; }
        public abstract short Precision { get; }
        public abstract short Scale { get; }
    }
}

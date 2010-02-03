using System;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public abstract class DbDataTypeParser
    {
        public abstract DbDataType Parse(string typeDeclaration);
        public abstract DbDataType GetColumnType(Type type);
        public abstract string GetVariableDeclaration(DbDataType type, bool suppressSize);
    }
}

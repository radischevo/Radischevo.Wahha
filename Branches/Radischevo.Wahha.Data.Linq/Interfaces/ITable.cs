using System;
using System.Linq;

namespace Jeltofiol.Wahha.Data.Linq
{
    public interface ITable : IQueryable
    {
        string EntityID { get; }
        Type EntityType { get; }
    }

    public interface ITable<T> : ITable, IQueryable<T>
    {
    }
}

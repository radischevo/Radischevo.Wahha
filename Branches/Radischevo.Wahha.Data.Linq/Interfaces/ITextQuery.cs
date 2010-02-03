using System;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq
{
    public interface ITextQuery
    {
        string GetQueryText(Expression expression);
    }
}

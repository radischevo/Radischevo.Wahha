using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jeltofiol.Wahha.Data.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq
{
    public class QueryParameter
    {
        string name;
        Type type;
        DbDataType _dbType;

        public QueryParameter(string name, Type type, DbDataType dbType)
        {
            this.name = name;
            this.type = type;
            this._dbType = dbType;
        }

        public string Name
        {
            get { return this.name; }
        }

        public Type Type
        {
            get { return this.type; }
        }

        public DbDataType DbType
        {
            get { return this._dbType; }
        }
    }
}

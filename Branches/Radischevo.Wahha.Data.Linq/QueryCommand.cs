using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using Jeltofiol.Wahha.Core;
using Jeltofiol.Wahha.Data.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq
{
    public class QueryCommand
    {
        string commandText;
        ReadOnlyCollection<QueryParameter> parameters;
        ReadOnlyCollection<DbColumnExpression> columns;

        public QueryCommand(string commandText, IEnumerable<QueryParameter> parameters, 
            IEnumerable<DbColumnExpression> columns)
        {
            this.commandText = commandText;
            this.parameters = parameters.AsReadOnly();
            this.columns = columns.AsReadOnly();
        }

        public string CommandText
        {
            get { return this.commandText; }
        }

        public ReadOnlyCollection<QueryParameter> Parameters
        {
            get { return this.parameters; }
        }

        public ReadOnlyCollection<DbColumnExpression> Columns
        {
            get { return this.columns; }
        }
    }
}

using System;
using System.Collections.Generic;

using Radischevo.Wahha.Data;

namespace ConsoleTester
{
	public class TextCommandOperation : DbCommandOperation<DbQueryResult>
	{
		private string _text;
		private object _parameters;

		public TextCommandOperation(string text, object parameters)
		{
			_text = text;
			_parameters = parameters;
		}

		protected override DbCommandDescriptor CreateCommand()
		{
			return new DbCommandDescriptor(_text, _parameters);
		}

		protected override DbQueryResult ExecuteCommand(IDbDataProvider provider, DbCommandDescriptor command)
		{
			return provider.Execute(command).AsDataReader(reader => new DbQueryResult(reader));
		}
	}
}

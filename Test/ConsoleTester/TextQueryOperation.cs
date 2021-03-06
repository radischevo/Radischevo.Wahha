﻿using System;
using System.Collections.Generic;

using Radischevo.Wahha.Data;

namespace ConsoleTester
{
	public class TextQueryOperation : CacheableDbCommandOperation<DbQueryResult>
	{
		private string _text;
		private object _parameters;

		public TextQueryOperation(string text, object parameters, params string[] tags)
		{
			_text = text;
			_parameters = parameters;

			foreach (string tag in tags)
				Tags.Add(tag);
		}

		protected override void Validate(ValidationContext context)
		{
			if (String.IsNullOrEmpty(_text))
				context.Errors.Add("text", "Command text is not defined");
		}

		protected override DbCommandDescriptor CreateCommand()
		{
			return new DbCommandDescriptor(_text, _parameters);
		}

		protected override DbQueryResult ExecuteCommand(DbOperationContext context, DbCommandDescriptor command)
		{
			return context.Provider.Execute(command).AsDataReader(reader => new DbQueryResult(reader));
		}
	}
}

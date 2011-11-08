using System;
using System.Collections.Generic;

using Radischevo.Wahha.Data;

namespace ConsoleTester
{
	public class TextModifyOperation : InvalidatingDbCommandOperation<int>
	{
		private string _text;
		private object _parameters;

		public TextModifyOperation(string text, object parameters, params string[] tags)
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

		protected override int ExecuteCommand(DbOperationContext context, DbCommandDescriptor command)
		{
			return context.Provider.Execute(command).AsNonQuery();
		}
	}
}

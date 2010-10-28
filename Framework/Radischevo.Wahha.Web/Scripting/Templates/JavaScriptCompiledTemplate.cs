using System;
using System.IO;

namespace Radischevo.Wahha.Web.Scripting.Templates
{
	public class JavaScriptCompiledTemplate : CompiledTemplate
	{
		#region Constructors
		public JavaScriptCompiledTemplate()
			: this(null)
		{
		}

		public JavaScriptCompiledTemplate(string source)
			: base()
		{
			Source = source;
		}
		#endregion

		#region Instance Methods
		protected override void ExecuteInternal(TemplateContext context)
		{
		}
		#endregion
	}
}

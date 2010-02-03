using System;
using System.Web;

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
		protected override void ExecuteInternal(HttpContext context, object[] parameters)
		{
		}
		#endregion
	}
}

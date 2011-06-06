using System;
using System.Security.Principal;

using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc.Html.Razor
{
	public abstract class WebPageRenderingBase : WebPageExecutingBase, ITemplateFile
	{
		#region Instance Fields
		private TemplateFileInfo _templateInfo;
		#endregion

		#region Instance Properties
		public abstract string Layout
		{
			get;
			set;
		}

		public virtual HttpRequestBase Request
		{
			get
			{
				if (Context == null)
					return null;

				return Context.Request;
			}
		}

		public virtual HttpResponseBase Response
		{
			get
			{
				if (Context == null)
					return null;
				
				return Context.Response;
			}
		}

		public virtual HttpServerUtilityBase Server
		{
			get
			{
				if (Context == null)
					return null;

				return Context.Server;
			}
		}

		public virtual HttpSessionStateBase Session
		{
			get
			{
				if (Context == null)
					return null;
				
				return Context.Session;
			}
		}

		public virtual IPrincipal User
		{
			get
			{
				if (Context == null)
					return null;

				return Context.User;
			}
		}

		public virtual TemplateFileInfo TemplateInfo
		{
			get
			{
				if (_templateInfo == null)
					_templateInfo = new TemplateFileInfo(VirtualPath);
				
				return _templateInfo;
			}
		}
		#endregion

		#region Instance Methods
		public abstract HelperResult Render(string path, params object[] data);
		#endregion
	}
}
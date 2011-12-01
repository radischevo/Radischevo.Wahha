using System;
using System.IO;
using System.Security.Principal;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Templates
{
	public abstract class TemplateRenderingBase : TemplateExecutingBase, ITemplateFile
	{
		#region Instance Fields
		private string _layout;
		private TemplateFileInfo _templateInfo;
		private TemplateContext _templateContext;
		#endregion

		#region Constructors
		protected TemplateRenderingBase()
			: base()
		{
		}
		#endregion

		#region Instance Properties
		public string Layout
		{
			get
			{
				return _layout;
			}
			set
			{
				_layout = value;
			}
		}

		public TextWriter Output
		{
			get
			{
				if (TemplateContext == null)
					return null;

				return TemplateContext.Output.Peek();
			}
		}

		public virtual TemplateContext TemplateContext
		{
			get
			{
				return _templateContext;
			}
			set
			{
				_templateContext = value;
			}
		}

		public virtual ValueDictionary Data
		{
			get
			{
				if (TemplateContext == null)
					return null;

				return TemplateContext.Data;
			}
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
		public abstract TemplateResult Render(string path, params object[] data);
		#endregion
	}
}
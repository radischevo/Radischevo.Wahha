using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;

using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Templates
{
	public abstract class TemplateExecutingBase
	{
		#region Instance Fields
		private string _virtualPath;
		private HttpContextBase _context;
		#endregion

		#region Constructors
		protected TemplateExecutingBase()
		{
		}
		#endregion

		#region Instance Properties
		public virtual HttpContextBase Context
		{
			get
			{
				return _context;
			}
			set
			{
				_context = value;
			}
		}

		public virtual HttpApplicationStateBase Application
		{
			get
			{
				if (Context == null)
					return null;

				return Context.Application;
			}
		}

		public virtual string VirtualPath
		{
			get
			{
				return _virtualPath;
			}
			set
			{
				_virtualPath = value;
			}
		}
		#endregion

		#region Static Methods
		public static void WriteTo(TextWriter writer, TemplateResult content)
		{
			if (content != null)
				content.WriteTo(writer);
		}

		public static void WriteTo(TextWriter writer, object content)
		{
			writer.Write(content);
		}
		#endregion

		#region Instance Methods
		public abstract void Execute();

		protected virtual string GetDirectory(string virtualPath)
		{
			return System.Web.VirtualPathUtility.GetDirectory(virtualPath);
		}

		protected virtual bool FileExists(string path)
		{
			return FileExists(path, false);
		}

		protected virtual bool FileExists(string path, bool useCache)
		{
			return VirtualPathFactoryManager.Instance.PageExists(path, useCache);
		}

		protected virtual Func<object> GetObjectFactory(string virtualPath)
		{
			return () => VirtualPathFactoryManager.Instance.CreateInstance<object>(virtualPath);
		}

		protected virtual string NormalizePath(string path)
		{
			return System.Web.VirtualPathUtility.Combine(VirtualPath, path);
		}

		protected abstract void Write(TemplateResult result);

		protected abstract void Write(object value);
		#endregion
	}
}

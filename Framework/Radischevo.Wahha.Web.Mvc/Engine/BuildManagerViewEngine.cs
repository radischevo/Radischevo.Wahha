using System;
using System.Net;
using System.Web;

namespace Radischevo.Wahha.Web.Mvc
{
	public abstract class BuildManagerViewEngine : VirtualPathViewEngine
	{
		#region Instance Fields
		private IBuildManager _buildManager;
		#endregion

		#region Constructors
		protected BuildManagerViewEngine()
			: base()
		{
		}

		protected BuildManagerViewEngine(params string[] locationFormats)
			: base(locationFormats)
		{
		}
		#endregion

		#region Instance Properties
		public IBuildManager BuildManager
		{
			get
			{
				if (_buildManager == null)
					_buildManager = new BuildManagerWrapper();
				
				return _buildManager;
			}
			set
			{
				_buildManager = value;
			}
		}
		#endregion

		#region Instance Methods
		protected sealed override bool FileExists(ControllerContext context, string virtualPath)
		{
			try
			{
				object instance = BuildManager.CreateInstanceFromVirtualPath(virtualPath, typeof(object));
				return (instance != null);
			}
			catch (HttpException he)
			{
				if (he is HttpParseException)
					throw;
				
				if (he.GetHttpCode() == (int)HttpStatusCode.NotFound)
					if (!base.FileExists(context, virtualPath))
						return false;
				
				throw;
			}
		}

		protected sealed override bool? IsValidPath(ControllerContext context, string virtualPath)
		{
			Type type = BuildManager.GetCompiledType(virtualPath);
			if (type != null)
				return IsValidCompiledType(context, virtualPath, type);
			
			return null;
		}

		protected abstract bool IsValidCompiledType(ControllerContext context, string virtualPath, Type type);
		#endregion
	}
}

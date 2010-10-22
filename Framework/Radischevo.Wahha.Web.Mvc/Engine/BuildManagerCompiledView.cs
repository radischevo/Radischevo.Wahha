using System;
using System.IO;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public abstract class BuildManagerCompiledView : IView
	{
		#region Instance Fields
		private IBuildManager _buildManager;
		private IViewActivator _activator;
		private string _viewPath;
		#endregion

		#region Constructors
		protected BuildManagerCompiledView(string viewPath, 
			IBuildManager buildManager, IViewActivator activator)
        {
            Precondition.Defined(viewPath, () => Error.ArgumentNull("viewPath"));
			Precondition.Require(buildManager, () => Error.ArgumentNull("buildManager"));
			Precondition.Require(activator, () => Error.ArgumentNull("activator"));

            _viewPath = viewPath;
			_buildManager = buildManager;
			_activator = activator;
        }
		#endregion

		#region Instance Properties
		public IViewActivator Activator
		{
			get
			{
				return _activator;
			}
		}

		public IBuildManager BuildManager
		{
			get
			{
				return _buildManager;
			}
		}

		public string ViewPath
		{
			get
			{
				return _viewPath;
			}
		}
		#endregion

		#region Instance Methods
		public void Render(ViewContext context, TextWriter writer)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));
			Type type = BuildManager.GetCompiledType(ViewPath);
			object instance = null;

			if (type != null)
				instance = Activator.Create(context, type);

			Precondition.Require(instance, () => Error.CouldNotCreateView(ViewPath));
			RenderView(context, writer, instance);
		}

		protected abstract void RenderView(ViewContext context, TextWriter writer, object instance);
		#endregion
	}
}

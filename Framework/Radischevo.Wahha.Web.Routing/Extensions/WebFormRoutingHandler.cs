using System;
using System.Web;
using System.Web.UI;
using System.Web.Security;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Routing
{
    public class WebFormRoutingHandler : RoutingHandlerBase
    {
        #region Instance Fields
        private string _virtualPath;
        private bool _validateAccessRights;
        private IBuildManager _buildManager;
        #endregion

        #region Constructors
        public WebFormRoutingHandler(string virtualPath)
            : this(virtualPath, false, null)
        {
        }

        public WebFormRoutingHandler(string virtualPath,
            bool validateAccessRights)
            : this(virtualPath, validateAccessRights, null)
        {
        }

        public WebFormRoutingHandler(string virtualPath, 
            bool validateAccessRights, IBuildManager buildManager)
            : base()
        {
            Precondition.Defined(virtualPath, () => Error.ArgumentNull("virtualPath"));

            _virtualPath = virtualPath;
            _validateAccessRights = validateAccessRights;
            _buildManager = buildManager;
        }
        #endregion

        #region Instance Properties
        public string VirtualPath
        {
            get
            {
                return _virtualPath;
            }
        }

        public IBuildManager BuildManager
        {
            get
            {
                if (_buildManager == null)
                    _buildManager = new BuildManagerWrapper();

                return _buildManager;
            }
        }

        public bool ValidateAccessRights
        {
            get
            {
                return _validateAccessRights;
            }
            set
            {
                _validateAccessRights = value;
            }
        }
        #endregion

        #region Instance Methods
        protected override IHttpHandler GetHttpHandler(RequestContext context)
        {
            if (ValidateAccessRights && !UrlAuthorizationModule
                .CheckUrlAccessForPrincipal(_virtualPath, context.Context.User, 
                context.Context.Request.HttpMethod.ToString()))
                throw new HttpException(403, "Access denied.");

            return (BuildManager.CreateInstanceFromVirtualPath(_virtualPath, typeof(Page)) as IHttpHandler);
        }
        #endregion
    }
}

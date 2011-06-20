using System.Web.Hosting;

namespace Radischevo.Wahha.Web.Templates {
    internal sealed class HostingEnvironmentWrapper : IHostingEnvironment {
        public string MapPath(string virtualPath) {
            return HostingEnvironment.MapPath(virtualPath);
        }
    }
}

using System.Configuration;

namespace Radischevo.Wahha.Web.Templates.Configurations
{
    public class RazorWebSectionGroup : ConfigurationSectionGroup {
        public static readonly string GroupName = "radischevo.wahha.web.templates";

        private bool _hostSet = false;
        private bool _pagesSet = false;
        
        private HostSection _host;
        private RazorPagesSection _pages;

        [ConfigurationProperty("host", IsRequired = false)]
        public HostSection Host {
            get { return _hostSet ? _host : (HostSection)Sections["host"]; }
            set { _host = value; _hostSet = true; }
        }

        [ConfigurationProperty("pages", IsRequired = false)]
        public RazorPagesSection Pages {
            get { return _pagesSet ? _pages : (RazorPagesSection)Sections["pages"]; }
            set { _pages = value; _pagesSet = true; }
        }
    }
}

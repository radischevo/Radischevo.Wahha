using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Web.Configuration;

namespace Radischevo.Wahha.Web.Templates.Configurations
{
    public class RazorPagesSection : ConfigurationSection {
        public static readonly string SectionName = RazorWebSectionGroup.GroupName + "/pages";

        private static readonly ConfigurationProperty _baseTypeProperty =
            new ConfigurationProperty("baseType",
                                      typeof(string),
                                      null,
                                      ConfigurationPropertyOptions.IsRequired);

        private static readonly ConfigurationProperty _namespacesProperty =
            new ConfigurationProperty("namespaces",
                                      typeof(NamespaceCollection),
                                      null,
                                      ConfigurationPropertyOptions.IsRequired);

        private bool _baseTypeSet = false;
        private bool _namespacesSet = false;

        private string _baseType;
        private NamespaceCollection _namespaces;

        [ConfigurationProperty("baseType", IsRequired = true)]
        public string BaseType {
            get { return _baseTypeSet ? _baseType : (string)this[_baseTypeProperty]; }
            set { _baseType = value; _baseTypeSet = true; }
        }

        [ConfigurationProperty("namespaces", IsRequired = true)]
        public NamespaceCollection Namespaces {
            get { return _namespacesSet ? _namespaces : (NamespaceCollection)this[_namespacesProperty]; }
            set { _namespaces = value; _namespacesSet = true; }
        }
    }
}

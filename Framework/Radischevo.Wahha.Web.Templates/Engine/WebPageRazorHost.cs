using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Web.Razor;
using System.Web;
using System.Collections.Concurrent;
using System;
using System.Web.Razor.Generator;
using System.Web.Razor.Parser;

namespace Radischevo.Wahha.Web.Templates {
    public class WebPageRazorHost : RazorEngineHost {
        // DevDiv Bug 941404 - Add a prefix and folder name to class names
		// TODO: Refactor these names
        internal const string PageClassNamePrefix = "_Page_";
        internal const string ApplicationInstancePropertyName = "ApplicationInstance";
        internal const string ContextPropertyName = "Context";
        internal const string DefineSectionMethodName = "DefineSection";
        internal const string WebDefaultNamespace = "ASP";
        internal const string WriteToMethodName = "WriteTo";
        internal const string WriteLiteralToMethodName = "WriteLiteralTo";

        private const string ApplicationStartFileName = "_AppStart";
        private const string PageStartFileName = "_PageStart";

        internal static readonly string FallbackApplicationTypeName = typeof(HttpApplication).FullName;
        internal static readonly string PageBaseClass = typeof(Template).FullName;
        internal static readonly string TemplateTypeName = typeof(TemplateResult).FullName;

        private static ConcurrentDictionary<string, object> _importedNamespaces = new ConcurrentDictionary<string, object>();

        private string _className;
        private RazorCodeLanguage _codeLanguage;
        private string _globalAsaxTypeName;
        private bool? _isSpecialPage; 
        private string _physicalPath = null;
        private readonly Dictionary<string, string> _specialFileBaseTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private string _specialFileBaseClass;

        public override RazorCodeLanguage CodeLanguage {
            get {
                if (_codeLanguage == null) {
                    _codeLanguage = GetCodeLanguage();
                }
                return _codeLanguage;
            }
            protected set { _codeLanguage = value; }
        }

        public override string DefaultBaseClass {
            get {
                if (base.DefaultBaseClass != null) {
                    return base.DefaultBaseClass;
                }
                if (IsSpecialPage) {
                    return SpecialPageBaseClass;
                }
                else {
                    return DefaultPageBaseClass;
                }
            }
            set {
                base.DefaultBaseClass = value;
            }
        }

        public override string DefaultClassName {
            get {
                if (_className == null) {
                    _className = GetClassName(VirtualPath);
                }
                return _className;
            }
            set { _className = value; }
        }

        public bool DefaultDebugCompilation { get; set; }

        public string DefaultPageBaseClass { get; set; }

        internal string GlobalAsaxTypeName {
            get {
                return _globalAsaxTypeName ?? (HostingEnvironment.IsHosted ? BuildManager.GetGlobalAsaxType().FullName : FallbackApplicationTypeName);
            }
            set { _globalAsaxTypeName = value; }
        }

        public bool IsSpecialPage {
            get {
                CheckForSpecialPage();
                return _isSpecialPage.Value;
            }
        }

        public string PhysicalPath {
            get {
                MapPhysicalPath();
                return _physicalPath;
            }
            set { _physicalPath = value; }
        }

        private string SpecialPageBaseClass {
            get {
                CheckForSpecialPage();
                return _specialFileBaseClass;
            }
        }

        public string VirtualPath { get; private set; }

        private WebPageRazorHost() {
            NamespaceImports.Add("System");
            NamespaceImports.Add("System.Collections.Generic");
            NamespaceImports.Add("System.IO");
            NamespaceImports.Add("System.Linq");
            NamespaceImports.Add("System.Net");
            NamespaceImports.Add("System.Web");
            NamespaceImports.Add("System.Web.Helpers");
            NamespaceImports.Add("System.Web.Security");
            NamespaceImports.Add("System.Web.UI");
            NamespaceImports.Add("System.Web.WebPages");
            NamespaceImports.Add("System.Web.WebPages.Html");

            DefaultNamespace = WebDefaultNamespace;
            GeneratedClassContext = new GeneratedClassContext(GeneratedClassContext.DefaultExecuteMethodName,
                                                              GeneratedClassContext.DefaultWriteMethodName,
                                                              GeneratedClassContext.DefaultWriteLiteralMethodName,
                                                              WriteToMethodName,
                                                              WriteLiteralToMethodName,
                                                              TemplateTypeName,
                                                              DefineSectionMethodName);
            DefaultPageBaseClass = PageBaseClass;
            DefaultDebugCompilation = true;
        }

        public WebPageRazorHost(string virtualPath)
            : this(virtualPath, null) {
        }

        public WebPageRazorHost(string virtualPath, string physicalPath)
            : this() {
            if (String.IsNullOrEmpty(virtualPath)) { throw new ArgumentException(String.Format("CommonResources.Argument_Cannot_Be_Null_Or_Empty", "virtualPath"), "virtualPath"); }

            VirtualPath = virtualPath;

            PhysicalPath = physicalPath;
            DefaultClassName = GetClassName(VirtualPath);
            CodeLanguage = GetCodeLanguage();
        }

        public static void AddGlobalImport(string ns) {
            if (String.IsNullOrEmpty(ns)) { throw new ArgumentException(String.Format("CommonResources.Argument_Cannot_Be_Null_Or_Empty", "ns"), "ns"); }

            _importedNamespaces.TryAdd(ns, null);
        }

        private void CheckForSpecialPage() {
            if (!_isSpecialPage.HasValue) {
                string fileName = Path.GetFileNameWithoutExtension(VirtualPath);
                string baseType;
                if (_specialFileBaseTypes.TryGetValue(fileName, out baseType)) {
                    _isSpecialPage = true;
                    _specialFileBaseClass = baseType;
                }
                else {
                    _isSpecialPage = false;
                }
            }
        }

        public override MarkupParser CreateMarkupParser() {
            return new HtmlMarkupParser();
        }

        private static RazorCodeLanguage DetermineCodeLanguage(string fileName) {
            string extension = Path.GetExtension(fileName);

            // Use an if rather than else-if just in case Path.GetExtension returns null for some reason
            if (String.IsNullOrEmpty(extension)) {
                return null;
            }
            if (extension[0] == '.') {
                extension = extension.Substring(1); // Trim off the dot
            }

            // Look up the language
            // At the moment this only deals with code languages: cs, vb, etc., but in theory we could have MarkupLanguageServices which allow for
            // interesting combinations like: vbcss, csxml, etc.
            RazorCodeLanguage language = GetLanguageByExtension(extension);
            return language;
        }

        protected virtual string GetClassName(string virtualPath) {
            // Remove "~/" and run through our santizer
            // For example, for ~/Foo/Bar/Baz.cshtml, the class name is _Page_Foo_Bar_Baz_cshtml
            return ParserHelpers.SanitizeClassName(PageClassNamePrefix + virtualPath.TrimStart('~', '/'));
        }

        protected virtual RazorCodeLanguage GetCodeLanguage() {
            RazorCodeLanguage language = DetermineCodeLanguage(VirtualPath);
            if (language == null && !String.IsNullOrEmpty(PhysicalPath)) {
                language = DetermineCodeLanguage(PhysicalPath);
            }

            if (language == null) {
                throw new InvalidOperationException(String.Format("RazorWebResources.BuildProvider_No_CodeLanguageService_For_Path {0]", VirtualPath));
            }

            return language;
        }

        public static IEnumerable<string> GetGlobalImports() {
            return _importedNamespaces.ToArray().Select(pair => pair.Key);
        }

        private static RazorCodeLanguage GetLanguageByExtension(string extension) {
            return RazorCodeLanguage.GetLanguageByExtension(extension);
        }

        private void MapPhysicalPath() {
            if (_physicalPath == null && HostingEnvironment.IsHosted) {
                string path = HostingEnvironment.MapPath(VirtualPath);
                if (!String.IsNullOrEmpty(path) && File.Exists(path)) {
                    _physicalPath = path;
                }
            }
        }

        public override void PostProcessGeneratedCode(CodeCompileUnit codeCompileUnit,
                                                      CodeNamespace generatedNamespace,
                                                      CodeTypeDeclaration generatedClass,
                                                      CodeMemberMethod executeMethod) {
            base.PostProcessGeneratedCode(codeCompileUnit, generatedNamespace, generatedClass, executeMethod);

            // Add additional global imports
            generatedNamespace.Imports.AddRange(GetGlobalImports().Select(s => new CodeNamespaceImport(s)).ToArray());

            // Create ApplicationInstance property
            CodeMemberProperty prop = new CodeMemberProperty() {
                Name = ApplicationInstancePropertyName,
                Type = new CodeTypeReference(GlobalAsaxTypeName),
                HasGet = true,
                HasSet = false,
                Attributes = MemberAttributes.Family | MemberAttributes.Final
            };
            prop.GetStatements.Add(
                new CodeMethodReturnStatement(
                    new CodeCastExpression(
                        new CodeTypeReference(GlobalAsaxTypeName),
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(
                                null,
                                ContextPropertyName),
                            ApplicationInstancePropertyName)
                        )
                    )
                );
            generatedClass.Members.Add(prop);
        }

        protected void RegisterSpecialFile(string fileName, Type baseType) {
            if (baseType == null) {
                throw new ArgumentNullException("baseType");
            }
            RegisterSpecialFile(fileName, baseType.FullName);
        }

        protected void RegisterSpecialFile(string fileName, string baseTypeName) {
            if (String.IsNullOrEmpty(fileName)) {
                throw new ArgumentException(String.Format("CommonResources.Argument_Cannot_Be_Null_Or_Empty {0}", "fileName"), "fileName");
            }
            if (String.IsNullOrEmpty(baseTypeName)) {
                throw new ArgumentException(String.Format("CommonResources.Argument_Cannot_Be_Null_Or_Empty {0}", "baseTypeName"), "baseTypeName");
            }

            _specialFileBaseTypes[fileName] = baseTypeName;
        }
    }
}

using System.CodeDom;
using System.Web.Compilation;
using System;

namespace Radischevo.Wahha.Web.Templates {
    internal sealed class AssemblyBuilderWrapper : IAssemblyBuilder {
        internal AssemblyBuilder InnerBuilder { get; set; }

        public AssemblyBuilderWrapper(AssemblyBuilder builder) {
            if (builder == null) {
                throw new ArgumentNullException("builder");
            }

            InnerBuilder = builder;
        }

        public void AddCodeCompileUnit(BuildProvider buildProvider, CodeCompileUnit compileUnit) {
            InnerBuilder.AddCodeCompileUnit(buildProvider, compileUnit);
        }

        public void GenerateTypeFactory(string typeName) {
            InnerBuilder.GenerateTypeFactory(typeName);
        }
    }
}

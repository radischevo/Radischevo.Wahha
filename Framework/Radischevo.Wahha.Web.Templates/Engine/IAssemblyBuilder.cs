using System.CodeDom;
using System.Web.Compilation;

namespace Radischevo.Wahha.Web.Templates {
    internal interface IAssemblyBuilder {
        void AddCodeCompileUnit(BuildProvider buildProvider, CodeCompileUnit compileUnit);
        void GenerateTypeFactory(string typeName);
    }
}

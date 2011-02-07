using System;
using System.CodeDom;
using System.Collections;
using System.Web.UI;

namespace Radischevo.Wahha.Web.Mvc.Html.Forms
{
    internal sealed class ViewTypeControlBuilder : ControlBuilder
    {
        #region Instance Fields
        private string _typeName;
        #endregion

        #region Constructors
        public ViewTypeControlBuilder()
        {   }
        #endregion

        #region Instance Methods
        public override void Init(TemplateParser parser, ControlBuilder builder,
            Type type, string tagName, string id, IDictionary attributes)
        {
            base.Init(parser, builder, type, tagName, id, attributes);
            _typeName = (string)attributes["typename"];
        }

        public override void ProcessGeneratedCode(CodeCompileUnit compileUnit, 
            CodeTypeDeclaration baseType, CodeTypeDeclaration derivedType, 
            CodeMemberMethod buildMethod, CodeMemberMethod dataBindingMethod)
        {
            derivedType.BaseTypes[0] = new CodeTypeReference(_typeName);
        }
        #endregion
    }
}

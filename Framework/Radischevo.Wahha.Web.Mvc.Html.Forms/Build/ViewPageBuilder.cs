using System;
using System.CodeDom;
using System.Web.UI;

namespace Radischevo.Wahha.Web.Mvc.Html.Forms
{
    internal sealed class ViewPageBuilder : FileLevelPageControlBuilder
    {
        #region Instance Fields
        private string _pageBaseType;
        #endregion

        #region Constructors
        public ViewPageBuilder()
        {
        }
        #endregion

        #region Instance Properties
        public string PageBaseType
        {
            get
            {
                return _pageBaseType;
            }
            set
            {
                _pageBaseType = value;
            }
        }
        #endregion

        #region Instance Methods
        public override void ProcessGeneratedCode(CodeCompileUnit compileUnit, CodeTypeDeclaration baseType, 
            CodeTypeDeclaration derivedType, CodeMemberMethod buildMethod, CodeMemberMethod dataBindingMethod)
        {
            if (PageBaseType != null)
                derivedType.BaseTypes[0] = new CodeTypeReference(PageBaseType);
        }
        #endregion
    }
}

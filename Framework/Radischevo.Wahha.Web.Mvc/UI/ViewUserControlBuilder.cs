using System;
using System.CodeDom;
using System.Web.UI;

namespace Radischevo.Wahha.Web.Mvc.UI
{
    internal sealed class ViewUserControlBuilder : FileLevelUserControlBuilder
    {
        #region Instance Fields
        private string _userControlBaseType;
        #endregion

        #region Constructors
        public ViewUserControlBuilder()
        {
        }
        #endregion

        #region Instance Properties
        public string UserControlBaseType
        {
            get
            {
                return _userControlBaseType;
            }
            set
            {
                _userControlBaseType = value;
            }
        }
        #endregion

        #region Instance Methods
        public override void ProcessGeneratedCode(CodeCompileUnit compileUnit, CodeTypeDeclaration baseType, 
            CodeTypeDeclaration derivedType, CodeMemberMethod buildMethod, CodeMemberMethod dataBindingMethod)
        {
            if (UserControlBaseType != null)
                derivedType.BaseTypes[0] = new CodeTypeReference(UserControlBaseType);
        }
        #endregion
    }
}

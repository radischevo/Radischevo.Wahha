using System;
using System.Collections;
using System.Web.UI;

namespace Radischevo.Wahha.Web.Mvc.Html.Forms
{
    internal class ViewTypeParserFilter : PageParserFilter
    {
        #region Nested Types
        private enum DirectiveType
        {
            Unknown,
            Page,
            UserControl,
            Master
        }
        #endregion

        #region Instance Fields
        private DirectiveType _directiveType;
        private string _viewBaseType;
        private bool _viewTypeControlAdded;
        #endregion

        #region Constructors
        public ViewTypeParserFilter() 
            : base()
        {   
        }
        #endregion

        #region Instance Properties
        public override bool AllowCode
        {
            get
            {
                return true;
            }
        }

        public override int NumberOfControlsAllowed
        {
            get
            {
                return -1;
            }
        }

        public override int NumberOfDirectDependenciesAllowed
        {
            get
            {
                return -1;
            }
        }

        public override int TotalNumberOfDependenciesAllowed
        {
            get
            {
                return -1;
            }
        }
        #endregion

        #region Static Methods
        private static bool IsGenericTypeString(string typeName)
        {
            return (typeName.IndexOfAny(new char[] { '<', '(' }) >= 0);
        }
        #endregion

        #region Instance Methods
        public override bool AllowBaseType(Type baseType)
        {
            return true;
        }

        public override bool AllowControl(Type controlType, ControlBuilder builder)
        {
            return true;
        }

        public override bool AllowServerSideInclude(string virtualPath)
        {
            return true;
        }

        public override bool AllowVirtualReference(string virtualPath, 
            VirtualReferenceType referenceType)
        {
            return true;
        }

        public override void ParseComplete(ControlBuilder builder)
        {
            base.ParseComplete(builder);
            ViewPageBuilder pageBuilder = (builder as ViewPageBuilder);
            if (pageBuilder != null)
                pageBuilder.PageBaseType = _viewBaseType;
            
            ViewUserControlBuilder controlBuilder = (builder as ViewUserControlBuilder);
            if (controlBuilder != null)
                controlBuilder.UserControlBaseType = _viewBaseType;
        }

        public override void PreprocessDirective(string directiveName, IDictionary attributes)
        {
            base.PreprocessDirective(directiveName, attributes);
            string fullName = null;
            
            if (directiveName != null)
            {
                if (directiveName != "page")
                {
                    if (directiveName == "control")
                    {
                        _directiveType = DirectiveType.UserControl;
                        fullName = typeof(ViewUserControl).FullName;
                    }
                    else if (directiveName == "master")
                    {
                        _directiveType = DirectiveType.Master;
                        fullName = typeof(ViewMasterPage).FullName;
                    }
                }
                else
                {
                    _directiveType = DirectiveType.Page;
                    fullName = typeof(ViewPage).FullName;
                }
            }

            if (_directiveType != DirectiveType.Unknown)
            {
                string inherits = (string)attributes["inherits"];
                if (!String.IsNullOrEmpty(inherits) && IsGenericTypeString(inherits))
                {
                    attributes["inherits"] = fullName;
                    _viewBaseType = inherits;
                }
            }
        }

        public override bool ProcessCodeConstruct(CodeConstructType codeType, string code)
        {
            if (codeType == CodeConstructType.ExpressionSnippet && !_viewTypeControlAdded 
                && _viewBaseType != null && _directiveType == DirectiveType.Master)
            {
                Hashtable attributes = new Hashtable();
                attributes["typename"] = _viewBaseType;
                base.AddControl(typeof(ViewType), attributes);
                this._viewTypeControlAdded = true;
            }
            return base.ProcessCodeConstruct(codeType, code);
        }
        #endregion
    }
}

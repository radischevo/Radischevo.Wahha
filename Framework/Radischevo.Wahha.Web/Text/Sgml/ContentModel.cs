using System;

namespace Radischevo.Wahha.Web.Text.Sgml
{
    public class ContentModel
    {
        #region Instance Fields
        private int _currentDepth;
        private DeclaredContent _declaredContent;
        private Group _model;
        #endregion

        #region Constructors
        public ContentModel()
        {
            _model = new Group(null);
        }
        #endregion

        #region Instance Properties
        public int CurrentDepth
        {
            get 
            { 
                return _currentDepth; 
            }
        }

        public DeclaredContent DeclaredContent
        {
            get 
            { 
                return _declaredContent; 
            }
            set 
            { 
                _declaredContent = value; 
            }
        }

        public Group Model
        {
            get 
            { 
                return _model; 
            }
        }
        #endregion

        #region Instance Methods
        public void AddConnector(char c)
        {
            _model.AddConnector(c);
        }

        public void AddOccurrence(char c)
        {
            _model.AddOccurrence(c);
        }

        public void AddSymbol(string sym)
        {
            _model.AddSymbol(sym);
        }

        public bool CanContain(string name, SgmlDtd dtd)
        {
            if (_declaredContent != DeclaredContent.Default)
                return false;
            
            return _model.CanContain(name, dtd);
        }

        public int PopGroup()
        {
            if (_currentDepth == 0)
                return -1;
            
            _currentDepth--;
            _model.Parent.AddGroup(_model);
            _model = _model.Parent;

            return _currentDepth;
        }

        public void PushGroup()
        {
            _model = new Group(_model);
            _currentDepth++;
        }

        public void SetDeclaredContent(string value)
        {
            switch (value)
            {
                case "EMPTY":
                    _declaredContent = DeclaredContent.Empty;
                    break;
                case "RCDATA":
                    _declaredContent = DeclaredContent.RCData;
                    break;
                case "CDATA":
                    _declaredContent = DeclaredContent.CData;
                    break;
                default:
                    throw Error.InvalidDeclaredContentTypeValue(value);
            }
        }
        #endregion
    }
}

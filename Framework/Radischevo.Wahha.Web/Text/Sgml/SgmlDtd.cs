using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Radischevo.Wahha.Web.Text.Sgml
{
    /// <summary>
    /// Provides DTD parsing and support for the SgmlParser framework.
    /// </summary>
    public class SgmlDtd
    {
        #region Constants
        private const string _whiteSpace = " \r\n\t";
        private const string _nameGroupTerm = " \r\n\t|,)";
        private const string _declaredContentTerm = " \r\n\t>";
        private const string _contentModelTerm = " \r\n\t,&|()?+*";
        private const string _parameterEntityTerm = " \t\r\n>";
        #endregion

        #region Instance Fields
        private string _name;
        private XmlNameTable _nameTable;
        private Entity _current;
        private StringBuilder _builder;
        private Dictionary<string, ElementDeclaration> _elements;
        private Dictionary<string, Entity> _paramEntities;
        private Dictionary<string, Entity> _entities;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SgmlDtd"/> class.
        /// </summary>
        /// <param name="name">The name of the DTD.</param>
        /// <param name="nt">The <see cref="XmlNameTable"/> is not used.</param>
        public SgmlDtd(string name, XmlNameTable nt)
        {
            _name = name;
            _nameTable = nt;
            _elements = new Dictionary<string, ElementDeclaration>();
            _paramEntities = new Dictionary<string, Entity>();
            _entities = new Dictionary<string, Entity>();
            _builder = new StringBuilder();
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// The name of the DTD.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Gets the XmlNameTable associated with this implementation.
        /// </summary>
        /// <value>The XmlNameTable enabling you to get the atomized 
        /// version of a string within the node.</value>
        public XmlNameTable NameTable
        {
            get
            {
                return _nameTable;
            }
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Parses a DTD and creates a <see cref="SgmlDtd"/> instance that encapsulates the DTD.
        /// </summary>
        /// <param name="baseUri">The base URI of the DTD.</param>
        /// <param name="name">The name of the DTD.</param>
        /// <param name="publicId"></param>
        /// <param name="url"></param>
        /// <param name="subset"></param>
        /// <param name="proxy"></param>
        /// <param name="nt">The <see cref="XmlNameTable"/> is not used.</param>
        /// <returns>A new <see cref="SgmlDtd"/> instance that encapsulates the DTD.</returns>
        public static SgmlDtd Parse(Uri baseUri, string name, string publicId, 
            string url, string subset, string proxy, XmlNameTable nt)
        {
            SgmlDtd dtd = new SgmlDtd(name, nt);
            if (!String.IsNullOrEmpty(url))
                dtd.PushEntity(baseUri, new Entity(dtd.Name, publicId, url, proxy));

            if (!String.IsNullOrEmpty(subset))
                dtd.PushEntity(baseUri, new Entity(name, subset));

            try
            {
                dtd.Parse();
            }
            catch (ApplicationException e)
            {
                throw new SgmlParseException(e.Message + dtd._current.Context());
            }
            return dtd;
        }

        /// <summary>
        /// Parses a DTD and creates a <see cref="SgmlDtd"/> instance that encapsulates the DTD.
        /// </summary>
        /// <param name="baseUri">The base URI of the DTD.</param>
        /// <param name="name">The name of the DTD.</param>
        /// <param name="input">The reader to load the DTD from.</param>
        /// <param name="subset"></param>
        /// <param name="proxy">The proxy server to use when loading resources.</param>
        /// <param name="nt">The <see cref="XmlNameTable"/> is NOT used.</param>
        /// <returns>A new <see cref="SgmlDtd"/> instance that encapsulates the DTD.</returns>
        public static SgmlDtd Parse(Uri baseUri, string name, TextReader input, 
            string subset, string proxy, XmlNameTable nt)
        {
            SgmlDtd dtd = new SgmlDtd(name, nt);
            dtd.PushEntity(baseUri, new Entity(dtd.Name, baseUri, input, proxy));
            
            if (!String.IsNullOrEmpty(subset))
                dtd.PushEntity(baseUri, new Entity(name, subset));

            try
            {
                dtd.Parse();
            }
            catch (ApplicationException e)
            {
                throw new SgmlParseException(e.Message + dtd._current.Context());
            }
            return dtd;
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Finds an entity in the DTD with the specified name.
        /// </summary>
        /// <param name="name">The name of the <see cref="Entity"/> to find.</param>
        /// <returns>The specified Entity from the DTD.</returns>
        public Entity FindEntity(string name)
        {
            Entity e;
            _entities.TryGetValue(name, out e);

            return e;
        }

        /// <summary>
        /// Finds an element declaration in the DTD with the specified name.
        /// </summary>
        /// <param name="name">The name of the <see cref="ElementDecl"/> to find and return.</param>
        /// <returns>The <see cref="ElementDecl"/> matching the specified name.</returns>
        public ElementDeclaration FindElement(string name)
        {
            ElementDeclaration el;
            _elements.TryGetValue(name.ToUpper(CultureInfo.InvariantCulture), out el);

            return el;
        }

        /// <summary>
        /// Returns a dictionary for looking up entities by their <see cref="Entity.Literal"/> value.
        /// </summary>
        /// <returns>A dictionary for looking up entities by their <see cref="Entity.Literal"/> value.</returns>
        public IDictionary<string, Entity> GetEntitiesLiteralNameLookup()
        {
            Dictionary<string, Entity> hashtable = new Dictionary<string, Entity>();
            foreach (Entity entity in _entities.Values)
                hashtable[entity.Literal] = entity;

            return hashtable;
        }

        private void PushEntity(Uri baseUri, Entity e)
        {
            e.Open(_current, baseUri);
            _current = e;
            _current.ReadChar();
        }

        private void PopEntity()
        {
            if (_current != null) 
                _current.Close();

            if (_current.Parent != null)
                _current = _current.Parent;
            else
                _current = null;
        }

        private void Parse()
        {
            char ch = _current.LastChar;
            while (true)
            {
                switch (ch)
                {
                    case Entity.EOF:
                        PopEntity();
                        if (_current == null)
                            return;

                        ch = _current.LastChar;
                        break;
                    case ' ':
                    case '\n':
                    case '\r':
                    case '\t':
                        ch = _current.ReadChar();
                        break;
                    case '<':
                        ParseMarkup();
                        ch = _current.ReadChar();
                        break;
                    case '%':
                        Entity e = ParseParameterEntity(SgmlDtd._whiteSpace);
                        try
                        {
                            PushEntity(_current.ResolvedUri, e);
                        }
                        catch (Exception ex)
                        {
                            throw new SgmlParseException(ex.Message + _current.Context(), ex);
                        }
                        ch = _current.LastChar;
                        break;
                    default:
                        throw Error.UnexpectedCharacter(ch);
                }
            }
        }

        private void ParseMarkup()
        {
            char ch = _current.ReadChar();
            if (ch != '!')
                throw Error.ExpectingDeclaration(ch);
            
            ch = _current.ReadChar();
            if (ch == '-')
            {
                ch = _current.ReadChar();
                if (ch != '-') 
                    throw Error.ExpectingComment(ch);

                _current.ScanToEnd(_builder, "Comment", "-->");
            }
            else if (ch == '[')
            {
                ParseMarkedSection();
            }
            else
            {
                string token = _current.ScanToken(_builder, _whiteSpace, true);
                switch (token)
                {
                    case "ENTITY":
                        ParseEntity();
                        break;
                    case "ELEMENT":
                        ParseElementDeclaration();
                        break;
                    case "ATTLIST":
                        ParseAttributeList();
                        break;
                    default:
                        throw Error.InvalidDeclaration(token);
                }
            }
        }

        private char ParseDeclarationComments()
        {
            char ch = _current.LastChar;
            while (ch == '-')
                ch = ParseDeclarationComment(true);
            
            return ch;
        }

        private char ParseDeclarationComment(bool full)
        {
            int start = _current.Line;
            // -^-...--
            // This method scans over a comment inside a markup declaration.
            char ch = _current.ReadChar();
            if (full && ch != '-') 
                throw Error.ExpectingCommentDelimiter(ch);

            _current.ScanToEnd(_builder, "Markup Comment", "--");
            return _current.SkipWhitespace();
        }

        private void ParseMarkedSection()
        {
            // <![^ name [ ... ]]>
            _current.ReadChar(); // move to next char.
            string name = ScanName("[");
            if (String.Equals(name, "INCLUDE", 
                StringComparison.InvariantCultureIgnoreCase))
                ParseIncludeSection();
            else if (String.Equals(name, "IGNORE", 
                StringComparison.InvariantCultureIgnoreCase))
                ParseIgnoreSection();
            else
                throw Error.InvalidMarkedSectionType(name);
        }

        private void ParseIncludeSection()
        {
            throw new NotImplementedException("Include Section");
        }

        private void ParseIgnoreSection()
        {
            int start = _current.Line;
            // <!-^-...-->
            char ch = _current.SkipWhitespace();
            if (ch != '[') 
                throw Error.CouldNotParseConditionalSection(ch);

            _current.ScanToEnd(_builder, "Conditional Section", "]]>");
        }

        private string ScanName(string term)
        {
            // skip whitespace, scan name (which may be parameter entity reference
            // which is then expanded to a name)
            char ch = _current.SkipWhitespace();
            if (ch == '%')
            {
                Entity e = ParseParameterEntity(term);
                ch = _current.LastChar;

                if (!e.IsInternal)
                    throw Error.UnsupportedExternalParameterEntityResolution();

                return e.Literal.Trim();
            }
            return _current.ScanToken(_builder, term, true);
        }

        private Entity ParseParameterEntity(string term)
        {
            // almost the same as this.current.ScanToken, except we also terminate on ';'
            char ch = _current.ReadChar();
            string name = _current.ScanToken(_builder, ";" + term, false);
            if (_current.LastChar == ';')
                _current.ReadChar();

            return GetParameterEntity(name);
        }

        private Entity GetParameterEntity(string name)
        {
            if(!_paramEntities.ContainsKey(name))
                throw Error.UndefinedParameterEntityReference(name);

            return _paramEntities[name];
        }

        private void ParseEntity()
        {
            char ch = _current.SkipWhitespace();
            bool pe = (ch == '%');
            if (pe)
            {
                // parameter entity.
                _current.ReadChar(); // move to next char
                ch = _current.SkipWhitespace();
            }
            string name = _current.ScanToken(_builder, _whiteSpace, true);
            ch = _current.SkipWhitespace();

            Entity e = null;
            if (ch == '"' || ch == '\'')
            {
                string literal = _current.ScanLiteral(_builder, ch);
                e = new Entity(name, literal);
            }
            else
            {
                string pubid = null;
                string extid = null;
                string tok = _current.ScanToken(_builder, _whiteSpace, true);
                if (Entity.IsLiteralType(tok))
                {
                    ch = _current.SkipWhitespace();
                    string literal = _current.ScanLiteral(_builder, ch);
                    e = new Entity(name, literal);
                    e.SetLiteralType(tok);
                }
                else
                {
                    extid = tok;
                    if (String.Equals(extid, "PUBLIC", StringComparison.InvariantCultureIgnoreCase))
                    {
                        ch = _current.SkipWhitespace();
                        if (ch == '"' || ch == '\'')
                            pubid = _current.ScanLiteral(_builder, ch);
                        else
                            throw Error.ExpectingPublicIdentifierLiteral(ch);
                    }
                    else if (!String.Equals(extid, "SYSTEM", StringComparison.InvariantCultureIgnoreCase))
                        throw Error.InvalidExternalIdentifierLiteral(extid);
                    
                    string uri = null;
                    ch = _current.SkipWhitespace();

                    if (ch == '"' || ch == '\'')
                        uri = _current.ScanLiteral(_builder, ch);
                    else if (ch != '>')
                        throw Error.ExpectingSystemIdentifierLiteral(ch);
                    
                    e = new Entity(name, pubid, uri, _current.Proxy);
                }
            }

            ch = _current.SkipWhitespace();
            if (ch == '-')
                ch = ParseDeclarationComments();

            if (ch != '>')
                throw Error.ExpectingEndOfEntityDeclaration(ch);
            
            if (pe)
                _paramEntities.Add(e.Name, e);
            else
                _entities.Add(e.Name, e);
        }

        private void ParseElementDeclaration()
        {
            char ch = _current.SkipWhitespace();
            string[] names = ParseNameGroup(ch, true);
            ch = Char.ToUpper(_current.SkipWhitespace(), CultureInfo.InvariantCulture);
            
            bool sto = false;
            bool eto = false;
            
            if (ch == 'O' || ch == '-')
            {
                sto = (ch == 'O'); // start tag optional?   
                _current.ReadChar();
                ch = Char.ToUpper(_current.SkipWhitespace(), CultureInfo.InvariantCulture);
                if (ch == 'O' || ch == '-')
                {
                    eto = (ch == 'O'); // end tag optional? 
                    ch = _current.ReadChar();
                }
            }
            ch = _current.SkipWhitespace();
            ContentModel cm = ParseContentModel(ch);
            ch = _current.SkipWhitespace();

            string[] exclusions = null;
            string[] inclusions = null;

            if (ch == '-')
            {
                ch = _current.ReadChar();
                if (ch == '(')
                {
                    exclusions = ParseNameGroup(ch, true);
                    ch = _current.SkipWhitespace();
                }
                else if (ch == '-')
                    ch = ParseDeclarationComment(false);
                else
                    throw Error.InvalidDeclarationSyntax(ch);
            }

            if (ch == '-')
                ch = ParseDeclarationComments();

            if (ch == '+')
            {
                ch = _current.ReadChar();
                if (ch != '(')
                    throw Error.ExpectingInclusionsNameGroup(ch);
                
                inclusions = ParseNameGroup(ch, true);
                ch = _current.SkipWhitespace();
            }

            if (ch == '-')
                ch = ParseDeclarationComments();

            if (ch != '>')
                throw Error.ExpectingEndOfElementDeclaration(ch);

            foreach (string name in names)
            {
                string atom = name.ToUpper(CultureInfo.InvariantCulture);
                _elements.Add(atom, new ElementDeclaration(atom, sto, eto, cm, inclusions, exclusions));
            }
        }

        private string[] ParseNameGroup(char ch, bool nameTokens)
        {
            List<string> names = new List<string>();
            if (ch == '(')
            {
                ch = _current.ReadChar();
                ch = _current.SkipWhitespace();

                while (ch != ')')
                {        
                    ch = _current.SkipWhitespace();
                    if (ch == '%')
                    {
                        Entity e = ParseParameterEntity(_nameGroupTerm);
                        PushEntity(_current.ResolvedUri, e);
                        ParseNameList(names, nameTokens);

                        PopEntity();
                        ch = _current.LastChar;
                    }
                    else
                    {
                        string token = _current.ScanToken(_builder, _nameGroupTerm, nameTokens);
                        token = token.ToUpper(CultureInfo.InvariantCulture);
                        names.Add(token);
                    }
                    ch = _current.SkipWhitespace();
                    if (ch == '|' || ch == ',') ch = _current.ReadChar();
                }
                _current.ReadChar();
            }
            else
            {
                string name = _current.ScanToken(_builder, _whiteSpace, nameTokens);
                name = name.ToUpper(CultureInfo.InvariantCulture);
                names.Add(name);
            }
            return names.ToArray();
        }

        private void ParseNameList(List<string> names, bool nameTokens)
        {
            char ch = _current.LastChar;
            ch = _current.SkipWhitespace();

            while (ch != Entity.EOF)
            {
                string name;
                if (ch == '%')
                {
                    Entity e = ParseParameterEntity(_nameGroupTerm);
                    PushEntity(_current.ResolvedUri, e);
                    ParseNameList(names, nameTokens);
                    PopEntity();
                    ch = _current.LastChar;
                }
                else
                {
                    name = _current.ScanToken(_builder, _nameGroupTerm, true);
                    name = name.ToUpper(CultureInfo.InvariantCulture);
                    names.Add(name);
                }

                ch = _current.SkipWhitespace();
                if (ch == '|')
                {
                    ch = _current.ReadChar();
                    ch = _current.SkipWhitespace();
                }
            }
        }

        private ContentModel ParseContentModel(char ch)
        {
            ContentModel cm = new ContentModel();
            if (ch == '(')
            {
                _current.ReadChar();
                ParseModel(')', cm);

                ch = _current.ReadChar();
                if (ch == '?' || ch == '+' || ch == '*')
                {
                    cm.AddOccurrence(ch);
                    _current.ReadChar();
                }
            }
            else if (ch == '%')
            {
                Entity e = ParseParameterEntity(_declaredContentTerm);
                
                PushEntity(_current.ResolvedUri, e);
                cm = ParseContentModel(_current.LastChar);
                PopEntity(); 
            }
            else
            {
                string dc = ScanName(_declaredContentTerm);
                cm.SetDeclaredContent(dc);
            }
            return cm;
        }

        private void ParseModel(char cmt, ContentModel cm)
        {
            int depth = cm.CurrentDepth;
            char ch = _current.LastChar;
            ch = _current.SkipWhitespace();

            while (ch != cmt || cm.CurrentDepth > depth) 
            {
                if (ch == Entity.EOF)
                    throw Error.ContentModelWasNotClosed();
                
                if (ch == '%')
                {
                    Entity e = ParseParameterEntity(_contentModelTerm);

                    PushEntity(_current.ResolvedUri, e);
                    ParseModel(Entity.EOF, cm);
                    PopEntity();
                    
                    ch = _current.SkipWhitespace();
                }
                else if (ch == '(')
                {
                    cm.PushGroup();

                    _current.ReadChar();
                    ch = _current.SkipWhitespace();
                }
                else if (ch == ')')
                {
                    ch = _current.ReadChar();
                    
                    if (ch == '*' || ch == '+' || ch == '?')
                    {
                        cm.AddOccurrence(ch);
                        ch = _current.ReadChar();
                    }

                    if (cm.PopGroup() < depth)
                        throw Error.ParameterEntityClosedOutsideTheScope();
                    
                    ch = _current.SkipWhitespace();
                }
                else if (ch == ',' || ch == '|' || ch == '&')
                {
                    cm.AddConnector(ch);
                    _current.ReadChar(); // skip connector
                    ch = _current.SkipWhitespace();
                }
                else
                {
                    string token;
                    if (ch == '#')
                    {
                        ch = _current.ReadChar();
                        token = "#" + _current.ScanToken(_builder, _contentModelTerm, true);
                    }
                    else
                    {
                        token = _current.ScanToken(_builder, SgmlDtd._contentModelTerm, true);
                    }

                    token = token.ToUpper(CultureInfo.InvariantCulture);
                    ch = _current.LastChar;
                    if (ch == '?' || ch == '+' || ch == '*')
                    {
                        cm.PushGroup();
                        cm.AddSymbol(token);
                        cm.AddOccurrence(ch);
                        cm.PopGroup();

                        _current.ReadChar(); // skip connector
                        ch = _current.SkipWhitespace();
                    }
                    else
                    {
                        cm.AddSymbol(token);
                        ch = _current.SkipWhitespace();
                    }
                }
            }
        }

        private void ParseAttributeList()
        {
            char ch = _current.SkipWhitespace();
            string[] names = ParseNameGroup(ch, true);
            
            AttributeList list = new AttributeList();
            ParseAttributeList(list, '>');

            foreach (string name in names)
            {
                ElementDeclaration e;
                if (!_elements.TryGetValue(name, out e))
                    throw Error.AttributeListReferencesToAnUndefinedElement(name);

                e.AddAttributeDefinitions(list);
            }
        }

        private void ParseAttributeList(AttributeList list, char term)
        {
            char ch = _current.SkipWhitespace();
            while (ch != term)
            {
                if (ch == '%')
                {
                    Entity e = ParseParameterEntity(_parameterEntityTerm);
                    PushEntity(_current.ResolvedUri, e);

                    ParseAttributeList(list, Entity.EOF);

                    PopEntity();
                    ch = _current.SkipWhitespace();
                }
                else if (ch == '-')
                {
                    ch = ParseDeclarationComments();
                }
                else
                {
                    AttributeDefinition a = ParseAttributeDefinition(ch);
                    list.Add(a);
                }
                ch = _current.SkipWhitespace();
            }
        }

        private AttributeDefinition ParseAttributeDefinition(char ch)
        {
            ch = _current.SkipWhitespace();
            string name = ScanName(_whiteSpace)
                .ToUpper(CultureInfo.InvariantCulture);

            AttributeDefinition attr = new AttributeDefinition(name);

            ch = _current.SkipWhitespace();
            if (ch == '-')
                ch = ParseDeclarationComments();

            ParseAttributeType(ch, attr);

            ch = _current.SkipWhitespace();
            if (ch == '-')
                ch = ParseDeclarationComments();

            ParseAttributeDefault(ch, attr);

            ch = _current.SkipWhitespace();
            if (ch == '-')
                ch = ParseDeclarationComments();

            return attr;
        }

        private void ParseAttributeType(char ch, AttributeDefinition attr)
        {
            if (ch == '%')
            {
                Entity e = ParseParameterEntity(_whiteSpace);
                
                PushEntity(_current.ResolvedUri, e);
                ParseAttributeType(_current.LastChar, attr);
                PopEntity();

                ch = _current.LastChar;
                return;
            }

            if (ch == '(')
            {
                attr.SetEnumeratedType(ParseNameGroup(ch, false), AttributeType.Enumeration);
            }
            else
            {
                string token = ScanName(_whiteSpace);
                if (String.Compare(token, "NOTATION", true, CultureInfo.InvariantCulture) == 0)
                {
                    ch = _current.SkipWhitespace();
                    if (ch != '(')
                        throw Error.ExpectingNameGroup(ch);
                    
                    attr.SetEnumeratedType(ParseNameGroup(ch, true), AttributeType.Notation);
                }
                else
                    attr.SetType(token);
            }
        }

        private void ParseAttributeDefault(char ch, AttributeDefinition attr)
        {
            if (ch == '%')
            {
                Entity e = ParseParameterEntity(SgmlDtd._whiteSpace);
                
                PushEntity(_current.ResolvedUri, e);
                ParseAttributeDefault(_current.LastChar, attr);
                PopEntity(); 

                ch = _current.LastChar;
                return;
            }

            bool hasDefault = true;
            if (ch == '#')
            {
                _current.ReadChar();
                string token = _current.ScanToken(_builder, _whiteSpace, true);
                hasDefault = attr.SetPresence(token);
                ch = _current.SkipWhitespace();
            }
            if (hasDefault)
            {
                if (ch == '\'' || ch == '"')
                {
                    string lit = _current.ScanLiteral(_builder, ch);
                    attr.Default = lit;
                    ch = _current.SkipWhitespace();
                }
                else
                {
                    string name = _current.ScanToken(_builder, _whiteSpace, false);
                    name = name.ToUpper(CultureInfo.InvariantCulture);
                    attr.Default = name;
                    ch = _current.SkipWhitespace();
                }
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Radischevo.Wahha.Web.Text.Sgml
{
    /// <summary>
    /// SgmlReader is an XmlReader API over any 
    /// SGML document (including built in 
    /// support for HTML).  
    /// </summary>
    public class SgmlReader : XmlReader
    {
        #region Constants
        /// <summary>
        /// The value returned when a namespace is queried and none has been specified.
        /// </summary>
        public const string UndefinedNamespace = "#unknown";
        private const string _declarationTerm = " \t\r\n><";
        private const string _tagTerm = " \t\r\n=/><";
        private const string _attrTerm = " \t\r\n='\"/>";
        private const string _attrValueTerm = " \t\r\n>";
        private const string _cDataTerm = "\t\r\n[]<>";
        private const string _docTypeTerm = " \t\r\n>";
        private const string _processingInstructionTerm = " \t\r\n?";
        #endregion

        #region Instance Fields
        private SgmlDtd _dtd;
        private XmlNameTable _nameTable;
        private Entity _current;
        private State _state;
        private char _partial;
        private string _endTag;
        private HWStack<SgmlNode> _stack;
        private SgmlNode _node;

        private Attribute _attr;
        private int _attrPosition;
        private Uri _baseUri;
        private StringBuilder _builder;
        private StringBuilder _nameBuilder;
        private TextWriter _log;
        private bool _foundRoot;

        private SgmlNode _newNode;
        private int _popToDepth;
        private int _rootCount;
        private bool _isHtml;
        private string _rootElementName;

        private string _href;
        private string _errorLogFile;
        private Entity _lastError;
        private string _proxy;
        private TextReader _inputStream;
        private string _systemLiteral;
        private string _publicId;
        private string _subset;
        private string _docType;
        private bool _stripDocType;

        private WhitespaceHandling _whitespaceHandling;
        private CaseFolding _folding;
        private Dictionary<string, string> _unknownNamespaces;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the SgmlReader class.
        /// </summary>
        public SgmlReader() 
            : this(new NameTable())
        {
        }

        /// <summary>
        /// Initializes a new instance of the SgmlReader class with an 
        /// existing <see cref="XmlNameTable"/>, which is not used.
        /// </summary>
        /// <param name="nt">The nametable to use.</param>
        public SgmlReader(XmlNameTable nt) 
        {
            _nameTable = nt;
            _stripDocType = true;
            _unknownNamespaces = new Dictionary<string, string>();

            Init();
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Specify the SgmlDtd object directly. 
        /// This allows you to cache the Dtd and share
        /// it across multipl SgmlReaders. To load a 
        /// DTD from a URL use the SystemLiteral property.
        /// </summary>
        public SgmlDtd Dtd
        {
            get
            {
                if (_dtd == null)
                    LazyLoadDtd(_baseUri);

                return _dtd;
            }
            set
            {
                _dtd = value;
            }
        }

        /// <summary>
        /// The name of root element specified 
        /// in the DOCTYPE tag.
        /// </summary>
        public string DocType
        {
            get
            {
                return _docType;
            }
            set
            {
                _docType = value;
            }
        }

        /// <summary>
        /// The root element of the document.
        /// </summary>
        public string RootElementName
        {
            get
            {
                return _rootElementName;
            }
        }

        /// <summary>
        /// The PUBLIC identifier in the DOCTYPE tag
        /// </summary>
        public string PublicIdentifier
        {
            get
            {
                return _publicId;
            }
            set
            {
                _publicId = value;
            }
        }

        /// <summary>
        /// The SYSTEM literal in the DOCTYPE tag 
        /// identifying the location of the DTD.
        /// </summary>
        public string SystemLiteral
        {
            get
            {
                return _systemLiteral;
            }
            set
            {
                _systemLiteral = value;
            }
        }

        /// <summary>
        /// The DTD internal subset in the DOCTYPE tag
        /// </summary>
        public string InternalSubset
        {
            get
            {
                return _subset;
            }
            set
            {
                _subset = value;
            }
        }

        /// <summary>
        /// The input stream containing SGML data to parse.
        /// You must specify this property or the 
        /// Href property before calling Read().
        /// </summary>
        public TextReader InputStream
        {
            get
            {
                return _inputStream;
            }
            set
            {
                _inputStream = value;
                Init();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the reader 
        /// is positioned at the end of the stream.
        /// </summary>
        /// <value>true if the reader is positioned at 
        /// the end of the stream; otherwise, false.</value>
        public override bool EOF
        {
            get
            {
                return (_state == State.Eof);
            }
        }

        /// <summary>
        /// Sometimes you need to specify a proxy server in order to load data via HTTP
        /// from outside the firewall.  For example: "itgproxy:80".
        /// </summary>
        public string WebProxy
        {
            get
            {
                return _proxy;
            }
            set
            {
                _proxy = value;
            }
        }

        /// <summary>
        /// Specify the location of the input 
        /// SGML document as a URL.
        /// </summary>
        public string Href
        {
            get
            {
                return _href;
            }
            set
            {
                _href = value;
                Init();

                if (_baseUri == null)
                {
                    if (_href.IndexOf(Uri.SchemeDelimiter) > 0)
                        _baseUri = new Uri(_href);
                    else
                        _baseUri = new Uri("file:///" + Directory.GetCurrentDirectory() + "//");
                }
            }
        }

        /// <summary>
        /// Whether to strip out the DOCTYPE tag from the output (default true)
        /// </summary>
        public bool StripDocType
        {
            get
            {
                return _stripDocType;
            }
            set
            {
                _stripDocType = value;
            }
        }

        /// <summary>
        /// The case conversion behaviour while processing tags.
        /// </summary>
        public CaseFolding CaseFolding
        {
            get
            {
                return _folding;
            }
            set
            {
                _folding = value;
            }
        }

        /// <summary>
        /// DTD validation errors are written to this stream.
        /// </summary>
        public TextWriter ErrorLog
        {
            get
            {
                return _log;
            }
            set
            {
                _log = value;
            }
        }

        /// <summary>
        /// DTD validation errors are written to this log file.
        /// </summary>
        public string ErrorLogFile
        {
            get
            {
                return _errorLogFile;
            }
            set
            {
                _errorLogFile = value;
                _log = new StreamWriter(value);
            }
        }

        /// <summary>
        /// The node type of the node currently being parsed.
        /// </summary>
        public override XmlNodeType NodeType
        {
            get
            {
                if (_state == State.Attr)
                    return XmlNodeType.Attribute;

                else if (_state == State.AttrValue)
                    return XmlNodeType.Text;

                else if (_state == State.EndTag ||
                    _state == State.AutoClose)
                    return XmlNodeType.EndElement;

                return _node.NodeType;
            }
        }

        /// <summary>
        /// The name of the current node, if currently 
        /// positioned on a node or attribute.
        /// </summary>
        public override string Name
        {
            get
            {
                if (_state == State.Attr)
                    return AddToNameTable(_attr.Name);
                
                else if (_state != State.AttrValue)
                    return AddToNameTable(_node.Name);
                
                return null;
            }
        }

        /// <summary>
        /// The local name of the current node, 
        /// if currently positioned on a node or attribute.
        /// </summary>
        public override string LocalName
        {
            get
            {
                string result = Name;
                if (result != null)
                {
                    int colon = result.IndexOf(':');
                    if (colon != -1)
                        return result.Substring(colon + 1);
                }

                return result;
            }
        }

        /// <summary>
        /// The namespace of the current node, if 
        /// currently positioned on a node or attribute.
        /// </summary>
        /// <remarks>
        /// If not positioned on a node or attribute, 
        /// <see cref="UndefinedNamespace"/> is returned.
        /// </remarks>
        public override string NamespaceURI
        {
            get
            {
                // SGML has no namespaces, unless this turned out to be an xmlns attribute.
                if (_state == State.Attr && String.Equals(_attr.Name, 
                    "xmlns", StringComparison.InvariantCultureIgnoreCase))
                    return "http://www.w3.org/2000/xmlns/";

                string prefix = Prefix;
                switch (Prefix)
                {
                    case "xmlns":
                        return "http://www.w3.org/2000/xmlns/";
                    case "xml":
                        return "http://www.w3.org/XML/1998/namespace";
                    case null: // Should never occur since Prefix never returns null
                    case "":
                        if (NodeType == XmlNodeType.Attribute)
                        {
                            return String.Empty;
                        }
                        else if (NodeType == XmlNodeType.Element)
                        {
                            for (int i = _stack.Count - 1; i > 0; --i)
                            {
                                SgmlNode node = _stack[i];
                                if (node != null && node.NodeType == XmlNodeType.Element)
                                {
                                    Attribute attr = node.GetAttribute("xmlns");
                                    if (attr != null && attr.Value != null)
                                        return attr.Value;
                                }
                            }
                        }
                        return String.Empty;
                    default:
                        string value;
                        if (NodeType == XmlNodeType.Attribute || 
                            NodeType == XmlNodeType.Element)
                        {
                            string key = "xmlns:" + prefix;
                            for (int i = _stack.Count - 1; i > 0; --i)
                            {
                                SgmlNode node = _stack[i];
                                if (node != null && node.NodeType == XmlNodeType.Element)
                                {
                                    Attribute attr = node.GetAttribute("xmlns");
                                    if (attr != null && attr.Value != null)
                                        return attr.Value;
                                }
                            }
                        }

                        if (!_unknownNamespaces.TryGetValue(prefix, out value))
                        {
                            if (_unknownNamespaces.Count > 0)
                                value = UndefinedNamespace + _unknownNamespaces.Count.ToString();
                            else
                                value = UndefinedNamespace;

                            _unknownNamespaces[prefix] = value;
                        }
                        return value;
                }
            }
        }

        /// <summary>
        /// The prefix of the current node's name.
        /// </summary>
        public override string Prefix
        {
            get
            {
                string result = Name;
                if (result != null)
                {
                    int colon = result.IndexOf(':');
                    if (colon != -1)
                        result = result.Substring(0, colon);
                    else
                        result = String.Empty;
                }
                return result ?? String.Empty;
            }
        }

        /// <summary>
        /// Whether the current node has a value or not.
        /// </summary>
        public override bool HasValue
        {
            get
            {
                if (_state == State.Attr || 
                    _state == State.AttrValue)
                    return true;

                return (_node.Value != null);
            }
        }

        /// <summary>
        /// The value of the current node.
        /// </summary>
        public override string Value
        {
            get
            {
                if (_state == State.Attr || 
                    _state == State.AttrValue)
                    return _attr.Value;

                return _node.Value;
            }
        }

        /// <summary>
        /// Gets the depth of the current node 
        /// in the XML document.
        /// </summary>
        /// <value>The depth of the current node 
        /// in the XML document.</value>
        public override int Depth
        {
            get
            {
                if (_state == State.Attr)
                    return _stack.Count;
                else if (_state == State.AttrValue)
                    return _stack.Count + 1;

                return _stack.Count - 1;
            }
        }

        /// <summary>
        /// Gets the base URI of the current node.
        /// </summary>
        /// <value>The base URI of the current node.</value>
        public override string BaseURI
        {
            get
            {
                return (_baseUri == null) ? String.Empty : _baseUri.AbsoluteUri;
            }
        }

        /// <summary>
        /// Gets the XmlNameTable associated with this implementation.
        /// </summary>
        /// <value>The XmlNameTable enabling you to get the 
        /// atomized version of a string within the node.</value>
        public override XmlNameTable NameTable
        {
            get
            {
                return _nameTable;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current node is an empty element (for example, &lt;MyElement/&gt;).
        /// </summary>
        public override bool IsEmptyElement
        {
            get
            {
                if (_state == State.Markup || 
                    _state == State.Attr || 
                    _state == State.AttrValue)
                    return _node.IsEmpty;

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current node 
        /// is an attribute that was generated from the 
        /// default value defined in the DTD or schema.
        /// </summary>
        /// <value>
        /// true if the current node is an attribute whose 
        /// value was generated from the default value 
        /// defined in the DTD or schema; false if the 
        /// attribute value was explicitly set.
        /// </value>
        public override bool IsDefault
        {
            get
            {
                if (_state == State.Attr || 
                    _state == State.AttrValue)
                    return _attr.IsDefault;

                return false;
            }
        }

        /// <summary>
        /// Gets the quotation mark character used to 
        /// enclose the value of an attribute node.
        /// </summary>
        /// <value>The quotation mark character (" or ') 
        /// used to enclose the value of an attribute node.</value>
        /// <remarks>
        /// This property applies only to an attribute node.
        /// </remarks>
        public override char QuoteChar
        {
            get
            {
                if (_attr != null)
                    return _attr.QuoteChar;

                return '\0';
            }
        }

        /// <summary>
        /// Gets the current xml:space scope.
        /// </summary>
        /// <value>One of the <see cref="XmlSpace"/> values. 
        /// If no xml:space scope exists, this property 
        /// defaults to XmlSpace.None.</value>
        public override XmlSpace XmlSpace
        {
            get
            {
                for (int i = _stack.Count - 1; i > 1; i--)
                {
                    XmlSpace xs = _stack[i].Space;
                    if (xs != XmlSpace.None)
                        return xs;
                }

                return XmlSpace.None;
            }
        }

        /// <summary>
        /// Gets the current xml:lang scope.
        /// </summary>
        /// <value>The current xml:lang scope.</value>
        public override string XmlLang
        {
            get
            {
                for (int i = _stack.Count - 1; i > 1; i--)
                {
                    string lang = _stack[i].XmlLang;
                    if (lang != null)
                        return lang;
                }

                return String.Empty;
            }
        }

        /// <summary>
        /// Specifies how white space is handled.
        /// </summary>
        public WhitespaceHandling WhitespaceHandling
        {
            get
            {
                return _whitespaceHandling;
            }
            set
            {
                _whitespaceHandling = value;
            }
        }

        /// <summary>
        /// Gets the number of attributes on the current node.
        /// </summary>
        /// <value>The number of attributes on the current node.</value>
        public override int AttributeCount
        {
            get
            {
                if (_state == State.Attr || _state == State.AttrValue)
                    return 0;
                else if (_node.NodeType == XmlNodeType.Element || 
                    _node.NodeType == XmlNodeType.DocumentType)
                    return _node.AttributeCount;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Gets whether the content is HTML or not.
        /// </summary>
        public bool IsHtml
        {
            get
            {
                return _isHtml;
            }
        }

        /// <summary>
        /// Gets the state of the reader.
        /// </summary>
        /// <value>One of the ReadState values.</value>
        public override ReadState ReadState
        {
            get
            {
                if (_state == State.Initial)
                    return ReadState.Initial;
                else if (_state == State.Eof)
                    return ReadState.EndOfFile;
                else
                    return ReadState.Interactive;
            }
        }

        /// <summary>
        /// Gets the value of the attribute with the specified index.
        /// </summary>
        /// <param name="i">The index of the attribute.</param>
        /// <returns>The value of the specified attribute. 
        /// This method does not move the reader.</returns>
        public override string this[int i]
        {
            get
            {
                return GetAttribute(i);
            }
        }

        /// <summary>
        /// Gets the value of an attribute with the specified <see cref="Name"/>.
        /// </summary>
        /// <param name="name">The name of the attribute to retrieve.</param>
        /// <returns>The value of the specified attribute. If the attribute 
        /// is not found, a null reference (Nothing in Visual Basic) is returned. </returns>
        public override string this[string name]
        {
            get
            {
                return GetAttribute(name);
            }
        }

        /// <summary>
        /// Gets the value of the attribute with the specified <see cref="LocalName"/> 
        /// and <see cref="NamespaceURI"/>.
        /// </summary>
        /// <param name="name">The local name of the attribute.</param>
        /// <param name="namespaceURI">The namespace URI of the attribute.</param>
        /// <returns>The value of the specified attribute. If the attribute 
        /// is not found, a null reference (Nothing in Visual Basic) is returned. 
        /// This method does not move the reader.</returns>
        public override string this[string name, string namespaceURI]
        {
            get
            {
                return GetAttribute(name, namespaceURI);
            }
        }
        #endregion

        #region Static Methods
        private static void ValidateAttribute(SgmlNode node, Attribute a)
        {
            ElementDeclaration e = node.DtdType;
            if (e != null)
            {
                AttributeDefinition ad = e.FindAttribute(a.Name);
                if (ad != null)
                    a.DtdType = ad;
            }
        }

        private static bool ValidAttributeName(string name)
        {
            try
            {
                XmlConvert.VerifyNMTOKEN(name);
                int index = name.IndexOf(':');
                if (index >= 0)
                    XmlConvert.VerifyNCName(name.Substring(index + 1));

                return true;
            }
            catch (XmlException)
            {
                return false;
            }
            catch (ArgumentNullException)
            {
                return false;
            }
        }

        private static bool VerifyName(string name)
        {
            try
            {
                XmlConvert.VerifyName(name);
                return true;
            }
            catch (XmlException)
            {
                return false;
            }
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// The base Uri is used to resolve relative Uri's like the SystemLiteral and
        /// Href properties.  This is a method because BaseURI is a read-only
        /// property on the base XmlReader class.
        /// </summary>
        public void SetBaseUri(string uri)
        {
            _baseUri = new Uri(uri);
        }

        /// <summary>
        /// Gets the value of an attribute with the specified <see cref="Name"/>.
        /// </summary>
        /// <param name="name">The name of the attribute to retrieve.</param>
        /// <returns>The value of the specified attribute. If the attribute is not 
        /// found, a null reference (Nothing in Visual Basic) is returned. </returns>
        public override string GetAttribute(string name)
        {
            if (_state != State.Attr && _state != State.AttrValue)
            {
                Attribute attr = _node.GetAttribute(name);
                return (attr == null) ? null : attr.Value;
            }
            return null;
        }

        /// <summary>
        /// Gets the value of the attribute with the specified <see cref="LocalName"/> 
        /// and <see cref="NamespaceURI"/>.
        /// </summary>
        /// <param name="name">The local name of the attribute.</param>
        /// <param name="namespaceURI">The namespace URI of the attribute.</param>
        /// <returns>The value of the specified attribute. If the attribute is not 
        /// found, a null reference (Nothing in Visual Basic) is returned. 
        /// This method does not move the reader.</returns>
        public override string GetAttribute(string name, string namespaceURI)
        {
            return GetAttribute(name); // SGML has no namespaces.
        }

        /// <summary>
        /// Gets the value of the attribute with the specified index.
        /// </summary>
        /// <param name="i">The index of the attribute.</param>
        /// <returns>The value of the specified attribute. 
        /// This method does not move the reader.</returns>
        public override string GetAttribute(int i)
        {
            if (_state != State.Attr && _state != State.AttrValue)
            {
                Attribute a = _node.GetAttribute(i);
                if (a != null)
                    return a.Value;
            }

            throw new ArgumentOutOfRangeException("i");
        }

        /// <summary>
        /// Moves to the atttribute with the specified <see cref="Name"/>.
        /// </summary>
        /// <param name="name">The qualified name of the attribute.</param>
        /// <returns>true if the attribute is found; otherwise, false. 
        /// If false, the reader's position does not change.</returns>
        public override bool MoveToAttribute(string name)
        {
            int i = _node.GetAttributeIndex(name);
            if (i >= 0)
            {
                MoveToAttribute(i);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Moves to the attribute with the specified <see cref="LocalName"/> and <see cref="NamespaceURI"/>.
        /// </summary>
        /// <param name="name">The local name of the attribute.</param>
        /// <param name="ns">The namespace URI of the attribute.</param>
        /// <returns>true if the attribute is found; otherwise, false. 
        /// If false, the reader's position does not change.</returns>
        public override bool MoveToAttribute(string name, string ns)
        {
            return MoveToAttribute(name);
        }

        /// <summary>
        /// Moves to the attribute with the specified index.
        /// </summary>
        /// <param name="i">The index of the attribute to move to.</param>
        public override void MoveToAttribute(int i)
        {
            Attribute a = _node.GetAttribute(i);
            if (a != null)
            {
                _attrPosition = i;
                _attr = a;
                if (_state != State.Attr)
                    _node.State = _state; //save current state.

                _state = State.Attr;
                return;
            }
            throw new ArgumentOutOfRangeException("i");
        }

        /// <summary>
        /// Moves to the first attribute.
        /// </summary>
        /// <returns></returns>
        public override bool MoveToFirstAttribute()
        {
            if (_node.AttributeCount > 0)
            {
                MoveToAttribute(0);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Moves to the next attribute.
        /// </summary>
        /// <returns>true if there is a next attribute; false 
        /// if there are no more attributes.</returns>
        /// <remarks>
        /// If the current node is an element node, this method is equivalent to 
        /// <see cref="MoveToFirstAttribute"/>. If <see cref="MoveToNextAttribute"/> 
        /// returns true, the reader moves to the next attribute; 
        /// otherwise, the position of the reader does not change.
        /// </remarks>
        public override bool MoveToNextAttribute()
        {
            if (_state != State.Attr && _state != State.AttrValue)
            {
                return MoveToFirstAttribute();
            }
            else if (_attrPosition < _node.AttributeCount - 1)
            {
                MoveToAttribute(_attrPosition + 1);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Moves to the element that contains the current attribute node.
        /// </summary>
        /// <returns>
        /// true if the reader is positioned on an attribute 
        /// (the reader moves to the element that owns the attribute); 
        /// false if the reader is not positioned
        /// on an attribute (the position of the reader does not change).
        /// </returns>
        public override bool MoveToElement()
        {
            if (_state == State.Attr || _state == State.AttrValue)
            {
                _state = _node.State;
                _attr = null;

                return true;
            }
            else
                return (_node.NodeType == XmlNodeType.Element);
        }

        /// <summary>
        /// Returns the encoding of the current entity.
        /// </summary>
        /// <returns>The encoding of the current entity.</returns>
        public Encoding GetEncoding()
        {
            if (_current == null)
                OpenInput();

            return _current.Encoding;
        }

        /// <summary>
        /// Reads the next node from the stream.
        /// </summary>
        /// <returns>true if the next node was read successfully; 
        /// false if there are no more nodes to read.</returns>
        public override bool Read()
        {
            if (_current == null)
                OpenInput();

            State start = _state;
            if (_node.Simulated)
            {
                _node.Simulated = false;
                _node = Top();
                _state = _node.State;

                return true;
            }

            bool foundNode = false;
            while (!foundNode)
            {
                switch (_state)
                {
                    case State.Initial:
                        _state = State.Markup;
                        _current.ReadChar();
                        goto case State.Markup;
                    case State.Eof:
                        if (_current.Parent != null)
                        {
                            _current.Close();
                            _current = _current.Parent;
                        }
                        else
                            return false;
                        break;
                    case State.EndTag:
                        if (String.Equals(_endTag, _node.Name,
                            StringComparison.InvariantCultureIgnoreCase))
                        {
                            Pop();
                            _state = State.Markup;
                            goto case State.Markup;
                        }
                        Pop();
                        foundNode = true;

                        break;
                    case State.Markup:
                        if (_node.IsEmpty)
                            Pop();

                        foundNode = ParseMarkup();
                        break;
                    case State.PartialTag:
                        Pop();
                        _state = State.Markup;
                        foundNode = ParseTag(_partial);
                        break;
                    case State.PseudoStartTag:
                        foundNode = ParseStartTag('<');
                        break;
                    case State.AutoClose:
                        Pop();
                        if (_stack.Count <= _popToDepth)
                        {
                            _state = State.Markup;
                            if (_newNode != null)
                            {
                                Push(_newNode);

                                _newNode = null;
                                _state = State.Markup;
                            }
                            else if (_node.NodeType == XmlNodeType.Document)
                            {
                                _state = State.Eof;
                                goto case State.Eof;
                            }
                        }
                        foundNode = true;
                        break;
                    case State.CData:
                        foundNode = ParseCData();
                        break;
                    case State.Attr:
                        goto case State.AttrValue;
                    case State.AttrValue:
                        _state = State.Markup;
                        goto case State.Markup;
                    case State.Text:
                        Pop();
                        goto case State.Markup;
                    case State.PartialText:
                        if (ParseText(_current.LastChar, false))
                            _node.NodeType = XmlNodeType.Whitespace;

                        foundNode = true;
                        break;
                }

                if (foundNode && _node.NodeType == XmlNodeType.Whitespace &&
                    _whitespaceHandling == WhitespaceHandling.None)
                    foundNode = false;

                if (!foundNode && _state == State.Eof && _stack.Count > 1)
                {
                    _popToDepth = 1;
                    _state = State.AutoClose;
                    _node = Top();

                    return true;
                }
            }

            if (!_foundRoot && (NodeType == XmlNodeType.Element ||
                NodeType == XmlNodeType.Text ||
                NodeType == XmlNodeType.CDATA))
            {
                _foundRoot = true;
                if (IsHtml && (NodeType != XmlNodeType.Element ||
                    !String.Equals(LocalName, "html",
                    StringComparison.InvariantCultureIgnoreCase)))
                {
                    _node.State = _state;
                    SgmlNode root = Push("html", XmlNodeType.Element, null);

                    SwapTopNodes();
                    _node = root;
                    root.Simulated = true;
                    root.IsEmpty = false;

                    _state = State.Markup;
                }
                return true;
            }
            return true;
        }

        /// <summary>
        /// Changes the <see cref="ReadState"/> to Closed.
        /// </summary>
        public override void Close()
        {
            if (_current != null)
            {
                _current.Close();
                _current = null;
            }

            if (_log != null)
            {
                _log.Close();
                _log = null;
            }
        }

        /// <summary>
        /// Reads the contents of an element or text node as a string.
        /// </summary>
        /// <returns>The contents of the element or an empty string.</returns>
        public override string ReadString()
        {
            if (_node.NodeType == XmlNodeType.Element)
            {
                _builder.Length = 0;
                while (Read())
                {
                    switch (NodeType)
                    {
                        case XmlNodeType.CDATA:
                        case XmlNodeType.SignificantWhitespace:
                        case XmlNodeType.Whitespace:
                        case XmlNodeType.Text:
                            _builder.Append(_node.Value);
                            break;
                        default:
                            return _builder.ToString();
                    }
                }
                return _builder.ToString();
            }
            return _node.Value;
        }

        /// <summary>
        /// Reads all the content, including markup, as a string.
        /// </summary>
        /// <returns>
        /// All the XML content, including markup, in 
        /// the current node. If the current node has no children,
        /// an empty string is returned. If the current node is 
        /// neither an element nor attribute, an empty
        /// string is returned.
        /// </returns>
        public override string ReadInnerXml()
        {
            StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);
            XmlTextWriter xw = new XmlTextWriter(sw);
            xw.Formatting = Formatting.Indented;

            switch (NodeType)
            {
                case XmlNodeType.Element:
                    Read();
                    while (!EOF && NodeType != XmlNodeType.EndElement)
                        xw.WriteNode(this, true);

                    Read();
                    break;
                case XmlNodeType.Attribute:
                    sw.Write(Value);
                    break;
                default:
                    // return empty string according to XmlReader spec.
                    break;
            }

            xw.Close();
            return sw.ToString();
        }

        /// <summary>
        /// Reads the content, including markup, 
        /// representing this node and all its children.
        /// </summary>
        /// <returns>
        /// If the reader is positioned on an element or an 
        /// attribute node, this method returns all the XML content, 
        /// including markup, of the current node and all its children; 
        /// otherwise, it returns an empty string.
        /// </returns>
        public override string ReadOuterXml()
        {
            StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);
            XmlTextWriter xw = new XmlTextWriter(sw);

            xw.Formatting = Formatting.Indented;
            xw.WriteNode(this, true);
            xw.Close();

            return sw.ToString();
        }

        /// <summary>
        /// Resolves a namespace prefix in the 
        /// current element's scope.
        /// </summary>
        /// <param name="prefix">The prefix whose namespace 
        /// URI you want to resolve. To match the default 
        /// namespace, pass an empty string.</param>
        /// <returns>The namespace URI to which the prefix 
        /// maps or a null reference (Nothing in Visual Basic) 
        /// if no matching prefix is found.</returns>
        public override string LookupNamespace(string prefix)
        {
            return null;
        }

        /// <summary>
        /// Resolves the entity reference for EntityReference nodes.
        /// </summary>
        /// <exception cref="InvalidOperationException">SgmlReader 
        /// does not resolve or return entities.</exception>
        public override void ResolveEntity()
        {
            throw Error.NotOnAnEntityReference();
        }

        /// <summary>
        /// Parses the attribute value into one or more Text, 
        /// EntityReference, or EndEntity nodes.
        /// </summary>
        /// <returns>
        /// true if there are nodes to return. false if the reader 
        /// is not positioned on an attribute node when the initial 
        /// call is made or if all the
        /// attribute values have been read. An empty attribute, 
        /// such as, misc="", returns true with a single node 
        /// with a value of String.Empty.
        /// </returns>
        public override bool ReadAttributeValue()
        {
            if (_state == State.Attr)
            {
                _state = State.AttrValue;
                return true;
            }
            else if (_state == State.AttrValue)
                return false;
            else
                throw Error.NotOnAnAttribute();
        }

        private void LazyLoadDtd(Uri baseUri)
        {
            if (_dtd == null)
            {
                if (String.IsNullOrEmpty(_systemLiteral))
                {
                    if (_docType != null && String.Equals(_docType, "html", 
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        Assembly a = typeof(SgmlReader).Assembly;
                        string name = a.FullName.Split(',')[0] + ".Resources.html.dtd";
                        Stream stream = a.GetManifestResourceStream(name);

                        if (stream != null)
                        {
                            StreamReader sr = new StreamReader(stream);
                            _dtd = SgmlDtd.Parse(baseUri, "HTML", sr, null, _proxy, null);
                        }
                    }
                }
                else
                {
                    if (baseUri != null)
                        baseUri = new Uri(baseUri, _systemLiteral);
                    else
                        baseUri = new Uri(new Uri(Directory.GetCurrentDirectory() + "\\"), _systemLiteral);

                    _dtd = SgmlDtd.Parse(baseUri, _docType, _publicId, baseUri.AbsoluteUri, _subset, _proxy, null);
                }
            }

            if (_dtd != null && _dtd.Name != null)
            {
                switch (CaseFolding)
                {
                    case CaseFolding.ToUpper:
                        _rootElementName = _dtd.Name.ToUpper(CultureInfo.InvariantCulture);
                        break;
                    case CaseFolding.ToLower:
                        _rootElementName = _dtd.Name.ToLower(CultureInfo.InvariantCulture);
                        break;
                    default:
                        _rootElementName = _dtd.Name;
                        break;
                }

                _isHtml = String.Equals(_dtd.Name, "html", 
                    StringComparison.InvariantCultureIgnoreCase);
            }
        }

        protected virtual void Init()
        {
            _state = State.Initial;
            _stack = new HWStack<SgmlNode>(10);
            _node = Push(null, XmlNodeType.Document, null);
            _node.IsEmpty = false;
            _builder = new StringBuilder();
            _nameBuilder = new StringBuilder();
            _popToDepth = 0;
            _current = null;
            _partial = '\0';
            _endTag = null;
            _attr = null;
            _attrPosition = 0;
            _newNode = null;
            _rootCount = 0;
            _foundRoot = false;
            _unknownNamespaces.Clear();
        }

        private string AddToNameTable(string name)
        {
            if (String.IsNullOrEmpty(name))
                return name;

            string atom = _nameTable.Get(name);
            if (atom == null)
                atom = _nameTable.Add(name);

            return name;
        }

        private void Log(string message, char ch)
        {
            Log(message, ch.ToString());
        }

        private void Log(string message, params string[] args)
        {
            if (ErrorLog != null)
            {
                string err = String.Format(CultureInfo.CurrentUICulture, message, args);
                if (_lastError != _current)
                {
                    _lastError = _current;
                    err = String.Concat("### Error: ", err, "\t", _current.Context());

                    ErrorLog.WriteLine(err);
                }
                else
                {
                    string path = String.Empty;
                    if (_current.ResolvedUri != null)
                        path = _current.ResolvedUri.AbsolutePath;

                    ErrorLog.WriteLine("### Error in {0}#{1}, line {2}, position {3}: {4}", 
                        path, _current.Name, _current.Line, _current.LinePosition, err);
                }
            }
        }

        private SgmlNode Push(string name, XmlNodeType nodeType, string value)
        {
            SgmlNode result = _stack.Push();
            if (result == null)
            {
                result = new SgmlNode();
                _stack[_stack.Count - 1] = result;
            }

            result.Reset(name, nodeType, value);
            _node = result;

            return result;
        }

        private void SwapTopNodes()
        {
            int top = _stack.Count - 1;
            if (top > 0)
            {
                SgmlNode n = _stack[top - 1];
                _stack[top - 1] = _stack[top];
                _stack[top] = n;
            }
        }

        private SgmlNode Push(SgmlNode n)
        {
            SgmlNode n2 = Push(n.Name, n.NodeType, n.Value);
            
            n2.DtdType = n.DtdType;
            n2.IsEmpty = n.IsEmpty;
            n2.Space = n.Space;
            n2.XmlLang = n.XmlLang;
            n2.State = n.State;
            n2.CopyAttributes(n);
            _node = n2;

            return n2;
        }

        private void Pop()
        {
            if (_stack.Count > 1)
                _node = _stack.Pop();
        }

        private SgmlNode Top()
        {
            int top = _stack.Count - 1;
            if (top > 0)
                return _stack[top];

            return null;
        }

        private void OpenInput()
        {
            LazyLoadDtd(_baseUri);

            if (Href != null)
                _current = new Entity("#document", null, _href, _proxy);
            else if (_inputStream != null)
                _current = new Entity("#document", null, _inputStream, _proxy);
            else
                throw Error.InputStreamIsUndefined();

            _current.IsHtml = IsHtml;
            _current.Open(null, _baseUri);

            if (_current.ResolvedUri != null)
                _baseUri = _current.ResolvedUri;

            if (_current.IsHtml && _dtd == null)
            {
                _docType = "HTML";
                LazyLoadDtd(_baseUri);
            }
        }

        private bool ParseMarkup()
        {
            char ch = _current.LastChar;
            if (ch == '<')
            {
                ch = _current.ReadChar();
                return ParseTag(ch);
            }
            else if (ch != Entity.EOF)
            {
                if (_node.DtdType != null && 
                    _node.DtdType.ContentModel.DeclaredContent == DeclaredContent.CData)
                {
                    _partial = '\0';
                    _state = State.CData;

                    return false;
                }
                else if (ParseText(ch, true))
                    _node.NodeType = XmlNodeType.Whitespace;

                return true;
            }

            _state = State.Eof;
            return false;
        }

        private bool ParseTag(char ch)
        {
            if (ch == '%')
            {
                return ParseAspNet();
            }
            else if (ch == '!')
            {
                ch = _current.ReadChar();
                if (ch == '-')
                {
                    return ParseComment();
                }
                else if (ch == '[')
                {
                    return ParseConditionalBlock();
                }
                else if (ch != '_' && !Char.IsLetter(ch))
                {
                    // perhaps it's one of those nasty office document hacks like '<![if ! ie ]>'
                    string value = _current.ScanToEnd(_builder, "Recovering", ">");
                    Log("Ignoring invalid markup '<!" + value + ">");

                    return false;
                }
                else
                {
                    string name = _current.ScanToken(_builder, _declarationTerm, false);
                    
                    if (String.Equals(name, "DOCTYPE", 
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        ParseDocType();

                        if (GetAttribute("SYSTEM") == null && GetAttribute("PUBLIC") != null)
                            _node.AddAttribute("SYSTEM", "", '"', _folding == CaseFolding.None);

                        if (_stripDocType)
                        {
                            return false;
                        }
                        else
                        {
                            _node.NodeType = XmlNodeType.DocumentType;
                            return true;
                        }
                    }
                    else
                    {
                        Log("Invalid declaration '<!{0}...'.  Expecting '<!DOCTYPE' only.", name);
                        _current.ScanToEnd(_builder, "Recovering", ">");

                        return false;
                    }
                }
            }
            else if (ch == '?')
            {
                _current.ReadChar();
                return ParseProcessingInstruction();
            }
            else if (ch == '/')
                return ParseEndTag();
            else
                return ParseStartTag(ch);
        }

        private string ScanName(string terminators)
        {
            string name = _current.ScanToken(_builder, terminators, false);
            switch (_folding)
            {
                case CaseFolding.ToUpper:
                    name = name.ToUpper(CultureInfo.InvariantCulture);
                    break;
                case CaseFolding.ToLower:
                    name = name.ToLower(CultureInfo.InvariantCulture);
                    break;
            }
            return name;
        }

        private bool ParseStartTag(char ch)
        {
            string name = null;
            if (_state != State.PseudoStartTag)
            {
                if (_tagTerm.IndexOf(ch) >= 0)
                {
                    _builder.Length = 0;
                    _builder.Append('<');
                    _state = State.PartialText;
                    return false;
                }
                name = ScanName(_tagTerm);
            }
            else
                _state = State.Markup;

            SgmlNode n = Push(name, XmlNodeType.Element, null);
            n.IsEmpty = false;

            Validate(n);
            ch = _current.SkipWhitespace();

            while (ch != Entity.EOF && ch != '>')
            {
                if (ch == '/')
                {
                    n.IsEmpty = true;
                    ch = _current.ReadChar();
                    if (ch != '>')
                    {
                        Log("Expected empty start tag '/>' sequence instead of '{0}'", ch);
                        _current.ScanToEnd(_builder, "Recovering", ">");

                        return false;
                    }
                    break;
                }
                else if (ch == '<')
                {
                    Log("Start tag '{0}' is missing '>'", name);
                    break;
                }

                string attrName = ScanName(_attrTerm);
                ch = _current.SkipWhitespace();

                if (String.Equals(attrName, ",", StringComparison.InvariantCultureIgnoreCase) ||
                    String.Equals(attrName, "=", StringComparison.InvariantCultureIgnoreCase) ||
                    String.Equals(attrName, ":", StringComparison.InvariantCultureIgnoreCase) ||
                    String.Equals(attrName, ";", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                string value = null;
                char quote = '\0';

                if (ch == '=' || ch == '"' || ch == '\'')
                {
                    if (ch == '=')
                    {
                        _current.ReadChar();
                        ch = _current.SkipWhitespace();
                    }

                    if (ch == '\'' || ch == '\"')
                    {
                        quote = ch;
                        value = ScanLiteral(_builder, ch);
                    }
                    else if (ch != '>')
                        value = _current.ScanToken(_builder, _attrValueTerm, false);
                }

                if (ValidAttributeName(attrName))
                {
                    Attribute a = n.AddAttribute(attrName, value, quote, 
                        _folding == CaseFolding.None);
                    
                    if (a == null)
                        Log("Duplicate attribute '{0}' ignored", attrName);
                    else
                        ValidateAttribute(n, a);
                }

                ch = _current.SkipWhitespace();
            }

            if (ch == Entity.EOF)
                throw Error.UnexpectedEndOfFileParsingStartTag(name);
            else if (ch == '>')
                _current.ReadChar();

            if (Depth == 1)
            {
                if (_rootCount == 1)
                {
                    _state = State.Eof;
                    return false;
                }
                _rootCount++;
            }

            ValidateContent(n);
            return true;
        }

        private bool ParseEndTag()
        {
            _state = State.EndTag;
            _current.ReadChar();
            string name = ScanName(_tagTerm);
            char ch = _current.SkipWhitespace();

            if (ch != '>')
            {
                Log("Expected empty start tag '/>' sequence instead of '{0}'", ch);
                _current.ScanToEnd(_builder, "Recovering", ">");
            }

            _current.ReadChar();
            _endTag = name;

            bool caseInsensitive = (_folding == CaseFolding.None);
            _node = _stack[_stack.Count - 1];

            for (int i = _stack.Count - 1; i > 0; i--)
            {
                SgmlNode n = _stack[i];
                if (String.Compare(n.Name, name, caseInsensitive, 
                    CultureInfo.InvariantCulture) == 0)
                {
                    _endTag = n.Name;
                    return true;
                }
            }

            Log("No matching start tag for '</{0}>'", name);
            _state = State.Markup;

            return false;
        }

        private bool ParseAspNet()
        {
            string value = "<%" + _current.ScanToEnd(_builder, "AspNet", "%>") + "%>";
            Push(null, XmlNodeType.CDATA, value);

            return true;
        }

        private bool ParseComment()
        {
            char ch = _current.ReadChar();
            if (ch != '-')
            {
                Log("Expecting comment '<!--' but found {0}", ch);
                _current.ScanToEnd(_builder, "Comment", ">");

                return false;
            }

            string value = _current.ScanToEnd(_builder, "Comment", "-->");

            int i = value.IndexOf("--");
            while (i >= 0)
            {
                int j = i + 2;
                while (j < value.Length && value[j] == '-')
                    j++;

                if (i > 0)
                    value = value.Substring(0, i - 1) + "-" + value.Substring(j);
                else
                    value = "-" + value.Substring(j);

                i = value.IndexOf("--");
            }

            if (value.Length > 0 && value[value.Length - 1] == '-')
                value += " ";

            Push(null, XmlNodeType.Comment, value);
            return true;
        }

        private bool ParseConditionalBlock()
        {
            char ch = _current.ReadChar();
            ch = _current.SkipWhitespace();
            string name = _current.ScanToken(_builder, _cDataTerm, false);

            if (name.StartsWith("if "))
            {
                _current.ScanToEnd(_builder, "CDATA", ">");
                return false;
            }
            else if (!String.Equals(name, "CDATA", 
                StringComparison.InvariantCultureIgnoreCase))
            {
                Log("Expecting CDATA but found '{0}'", name);
                _current.ScanToEnd(_builder, "CDATA", ">");

                return false;
            }
            else
            {
                ch = _current.SkipWhitespace();
                if (ch != '[')
                {
                    Log("Expecting '[' but found '{0}'", ch);
                    _current.ScanToEnd(_builder, "CDATA", ">");

                    return false;
                }

                string value = _current.ScanToEnd(_builder, "CDATA", "]]>");
                Push(null, XmlNodeType.CDATA, value);
                return true;
            }
        }

        private void ParseDocType()
        {
            char ch = _current.SkipWhitespace();
            string name = ScanName(_docTypeTerm);
            
            Push(name, XmlNodeType.DocumentType, null);
            ch = _current.SkipWhitespace();

            if (ch != '>')
            {
                string subset = "";
                string pubid = "";
                string syslit = "";

                if (ch != '[')
                {
                    string token = _current.ScanToken(_builder, _docTypeTerm, false);
                    if (String.Equals(token, "PUBLIC", 
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        ch = _current.SkipWhitespace();
                        if (ch == '\"' || ch == '\'')
                        {
                            pubid = _current.ScanLiteral(_builder, ch);
                            _node.AddAttribute(token, pubid, ch, _folding == CaseFolding.None);
                        }
                    }
                    else if (!String.Equals(token, "SYSTEM", 
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        Log("Unexpected token in DOCTYPE '{0}'", token);
                        _current.ScanToEnd(_builder, "DOCTYPE", ">");
                    }
                    
                    ch = _current.SkipWhitespace();
                    if (ch == '\"' || ch == '\'')
                    {
                        token = "SYSTEM";
                        syslit = _current.ScanLiteral(_builder, ch);
                        _node.AddAttribute(token, syslit, ch, _folding == CaseFolding.None);
                    }
                    ch = _current.SkipWhitespace();
                }

                if (ch == '[')
                {
                    subset = _current.ScanToEnd(_builder, "Internal Subset", "]");
                    _node.Value = subset;
                }

                ch = _current.SkipWhitespace();
                if (ch != '>')
                {
                    Log("Expecting end of DOCTYPE tag, but found '{0}'", ch);
                    _current.ScanToEnd(_builder, "DOCTYPE", ">");
                }

                if (_dtd != null && !String.Equals(_dtd.Name, name, 
                    StringComparison.InvariantCultureIgnoreCase))
                    throw Error.DtdDoesNotMatchDocumentType();

                _docType = name;
                _publicId = pubid;
                _systemLiteral = syslit;
                _subset = subset;

                LazyLoadDtd(_current.ResolvedUri);
            }
            _current.ReadChar();
        }

        private bool ParseProcessingInstruction()
        {
            string name = _current.ScanToken(_builder, _processingInstructionTerm, false);
            string value = null;

            if (_current.LastChar != '?')
                value = _current.ScanToEnd(_builder, "Processing Instruction", ">");
            else
                value = _current.ScanToEnd(_builder, "Processing Instruction", ">");
            
            if (!String.Equals(name, "xml", StringComparison.InvariantCultureIgnoreCase))
            {
                Push(name, XmlNodeType.ProcessingInstruction, value);
                return true;
            }
            return false;
        }

        private bool ParseText(char ch, bool newText)
        {
            bool ws = (!newText || _current.IsWhitespace);
            if (newText)
                _builder.Length = 0;

            _state = State.Text;

            while (ch != Entity.EOF)
            {
                if (ch == '<')
                {
                    ch = _current.ReadChar();
                    if (ch == '/' || ch == '!' || ch == '?' || Char.IsLetter(ch))
                    {
                        _state = State.PartialTag;
                        _partial = ch;
                        break;
                    }
                    else
                    {
                        _builder.Append('<');
                        _builder.Append(ch);
                        ws = false;
                        ch = _current.ReadChar();
                    }
                }
                else if (ch == '&')
                {
                    ExpandEntity(_builder, '<');
                    ws = false;
                    ch = _current.LastChar;
                }
                else
                {
                    if (!_current.IsWhitespace)
                        ws = false;
                    _builder.Append(ch);
                    ch = _current.ReadChar();
                }
            }

            string value = _builder.ToString();
            Push(null, XmlNodeType.Text, value);

            return ws;
        }

        private string ScanLiteral(StringBuilder sb, char quote)
        {
            sb.Length = 0;
            char ch = _current.ReadChar();

            while (ch != Entity.EOF && ch != quote && ch != '>')
            {
                if (ch == '&')
                {
                    ExpandEntity(sb, quote);
                    ch = _current.LastChar;
                }
                else
                {
                    sb.Append(ch);
                    ch = _current.ReadChar();
                }
            }
            if (ch == quote)
                _current.ReadChar();
            
            return sb.ToString();
        }

        private bool ParseCData()
        {
            bool ws = _current.IsWhitespace;
            char ch = _current.LastChar;
            _builder.Length = 0;

            if (_partial != '\0')
            {
                Pop();
                switch (_partial)
                {
                    case '!':
                        _partial = ' ';
                        return ParseComment();
                    case '?':
                        _partial = ' ';
                        return ParseProcessingInstruction();
                    case '/':
                        _state = State.EndTag;
                        return true;
                    case ' ':
                        break;
                }
            }

            while (ch != Entity.EOF)
            {
                if (ch == '<')
                {
                    ch = _current.ReadChar();
                    if (ch == '!')
                    {
                        ch = _current.ReadChar();
                        if (ch == '-')
                        {
                            if (ws)
                            {
                                _partial = ' ';
                                return ParseComment();
                            }
                            else
                            {
                                _partial = '!';
                                break;
                            }

                        } 
                        else if (ch == '[')
                        {
                            if (ParseConditionalBlock())
                            {
                                _partial = ' ';
                                return true;
                            }
                        }
                        else
                        {
                            _builder.Append('<');
                            _builder.Append('!');
                            _builder.Append(ch);

                            ws = false;
                        }
                    }
                    else if (ch == '?')
                    {
                        _current.ReadChar();
                        if (ws)
                        {
                            _partial = ' ';
                            return ParseProcessingInstruction();
                        }
                        else
                        {
                            _partial = '?';
                            break;
                        }
                    }
                    else if (ch == '/')
                    {
                        string temp = _builder.ToString();
                        if (ParseEndTag() && String.Equals(_endTag, _node.Name, 
                            StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (ws || String.IsNullOrEmpty(temp))
                            {
                                return true;
                            }
                            else
                            {
                                _partial = '/';
                                _builder.Length = 0;
                                _builder.Append(temp);
                                _state = State.CData;

                                break;
                            }
                        }
                        else
                        {
                            _builder.Length = 0;
                            _builder.Append(temp);
                            _builder.Append("</" + _endTag + ">");

                            ws = false;
                        }
                    }
                    else
                    {
                        _builder.Append('<');
                        _builder.Append(ch);

                        ws = false;
                    }
                }
                else
                {
                    if (!_current.IsWhitespace && ws)
                        ws = false;

                    _builder.Append(ch);
                }
                ch = _current.ReadChar();
            }

            if (ch == Entity.EOF)
            {
                _state = State.Eof;
                return false;
            }

            string value = _builder.ToString();

            value = value.Replace("<![CDATA[", string.Empty);
            value = value.Replace("]]>", string.Empty);
            value = value.Replace("/**/", string.Empty);

            Push(null, XmlNodeType.CDATA, value);

            if (_partial == '\0')
                _partial = ' ';

            return true;
        }

        private void ExpandEntity(StringBuilder sb, char terminator)
        {
            char ch = _current.ReadChar();
            if (ch == '#')
            {
                string charent = _current.ExpandCharEntity();
                sb.Append(charent);
                ch = _current.LastChar;
            }
            else
            {
                _nameBuilder.Length = 0;
                
                while (ch != Entity.EOF &&
                    (Char.IsLetter(ch) || ch == '_' || ch == '-') || 
                    (_nameBuilder.Length > 0 && Char.IsDigit(ch)))
                {
                    _nameBuilder.Append(ch);
                    ch = _current.ReadChar();
                }

                string name = _nameBuilder.ToString();
                if (_dtd != null && !String.IsNullOrEmpty(name))
                {
                    Entity e = _dtd.FindEntity(name);
                    if (e != null)
                    {
                        if (e.IsInternal)
                        {
                            sb.Append(e.Literal);
                            if (ch != terminator)
                                ch = _current.ReadChar();

                            return;
                        }
                        else
                        {
                            Entity ex = new Entity(name, e.PublicId, e.Uri, _current.Proxy);
                            e.Open(_current, new Uri(e.Uri));
                            
                            _current = ex;
                            _current.ReadChar();

                            return;
                        }
                    }
                    else
                        Log("Undefined entity '{0}'", name);
                }
                
                sb.Append("&");
                sb.Append(name);
                
                if (ch != terminator)
                {
                    sb.Append(ch);
                    ch = _current.ReadChar();
                }
            }
        }

        private void Validate(SgmlNode node)
        {
            if (_dtd != null)
            {
                ElementDeclaration e = _dtd.FindElement(node.Name);
                if (e != null)
                {
                    node.DtdType = e;
                    if (e.ContentModel.DeclaredContent == DeclaredContent.Empty)
                        node.IsEmpty = true;
                }
            }
        }

        private void ValidateContent(SgmlNode node)
        {
            if (node.NodeType == XmlNodeType.Element)
            {
                if (!VerifyName(node.Name))
                {
                    Pop();
                    Push(null, XmlNodeType.Text, "<" + node.Name + ">");

                    return;
                }
            }

            if (_dtd != null)
            {
                string name = node.Name.ToUpper(CultureInfo.InvariantCulture);
                int i = 0;
                int top = _stack.Count - 2;

                if (node.DtdType != null)
                {
                    for (i = top; i > 0; i--)
                    {
                        SgmlNode n = _stack[i];
                        if (n.IsEmpty)
                            continue;

                        ElementDeclaration f = n.DtdType;
                        if (f != null)
                        {
                            if (i == 2 && String.Equals(f.Name, "BODY",
                                StringComparison.InvariantCultureIgnoreCase))
                                break;

                            else if (String.Equals(f.Name, _dtd.Name,
                                StringComparison.InvariantCultureIgnoreCase))
                                break;

                            else if (f.CanContain(name, _dtd))
                                break;

                            else if (!f.EndTagOptional)
                                break;
                        }
                        else
                            break;
                    }
                }

                if (i == 0)
                {
                    return;
                }
                else if (i < top)
                {
                    SgmlNode n = _stack[top];
                    if (i == top - 1 && String.Equals(name, n.Name, 
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        // e.g. p not allowed inside p, not an interesting error.
                    }
                    else
                    {
#if DEBUG
                        string closing = "";
                        for (int k = top; k >= i+1; k--) 
                        {
                            
                            if (closing != "") 
                                closing += ",";

                            SgmlNode n2 = _stack[k];
                            closing += "<" + n2.Name + ">";
                        }
                        Log("Element '{0}' not allowed inside '{1}', closing {2}.", name, n.Name, closing);
#endif
                    }

                    _state = State.AutoClose;
                    _newNode = node;
                    Pop(); 

                    _popToDepth = i + 1;
                }
            }
        }
        #endregion
    }
}

using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Text.Sgml
{
    /// <summary>
    /// An Entity declared in a DTD.
    /// </summary>
    public class Entity : IDisposable
    {
        #region Constants
        public const char EOF = '\uffff';
        private const string _userAgent = "Mozilla/4.0 (compatible;);";
        #endregion

        #region Static Fields
        private static int[] _ctrlMap = new int[] {
             // This is the windows-1252 mapping of the code points 0x80 through 0x9f.
             8364, 129, 8218, 402, 8222, 8230, 8224, 8225, 710, 8240, 352, 8249, 338, 141,
             381, 143, 144, 8216, 8217, 8220, 8221, 8226, 8211, 8212, 732, 8482, 353, 8250, 
             339, 157, 382, 376
        };
        #endregion

        #region Instance Fields
        private string _proxy;
        private string _name;
        private bool _isInternal;
        private string _publicId;
        private string _uri;
        private string _literal;
        private LiteralType _literalType;
        private Entity _parent;
        private bool _isHtml;
        private int _line;
        private char _lastChar;
        private bool _isWhitespace;
        private Encoding _encoding;
        private Uri _resolvedUri;
        private TextReader _reader;
        private bool _ownedStream;
        private int _lineStart;
        private int _absolutePosition;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of an Entity declared in a DTD.
        /// </summary>
        /// <param name="name">The name of the entity.</param>
        /// <param name="literal">The literal value of the entity.</param>
        public Entity(string name, string literal)
        {
            _name = name;
            _literal = literal;
            _isInternal = true;
        }

        /// <summary>
        /// Initializes a new instance of an Entity declared in a DTD.
        /// </summary>
        /// <param name="name">The name of the entity.</param>
        /// <param name="publicId">The public id of the entity.</param>
        /// <param name="uri">The uri of the entity.</param>
        /// <param name="proxy">The proxy server to use when retrieving any web content.</param>
        public Entity(string name, string publicId, 
            string uri, string proxy)
        {
            _name = name;
            _publicId = publicId;
            _uri = uri;
            _proxy = proxy;
            _isHtml = (name != null && String.Equals(name, "html", 
                StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Initializes a new instance of an Entity declared in a DTD.
        /// </summary>
        /// <param name="name">The name of the entity.</param>
        /// <param name="baseUri">The baseUri for the entity to read from the TextReader.</param>
        /// <param name="reader">The TextReader to read the entity from.</param>
        /// <param name="proxy">The proxy server to use when retrieving any web content.</param>
        public Entity(string name, Uri baseUri, 
            TextReader reader, string proxy)
        {
            _reader = reader;
            _resolvedUri = baseUri;
            _name = name;
            _isInternal = true;
            _proxy = proxy;
            _isHtml = (String.Equals(name, "html",
                StringComparison.InvariantCultureIgnoreCase));
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// The name of the entity.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// True if the entity is the html element entity.
        /// </summary>
        public bool IsHtml
        {
            get
            {
                return _isHtml;
            }
            set
            {
                _isHtml = value;
            }
        }

        /// <summary>
        /// The public identifier of this entity.
        /// </summary>
        public string PublicId
        {
            get
            {
                return _publicId;
            }
        }

        /// <summary>
        /// The Uri that is the source for this entity.
        /// </summary>
        public string Uri
        {
            get
            {
                return _uri;
            }
        }

        /// <summary>
        /// The resolved location of the 
        /// DTD this entity is from.
        /// </summary>
        public Uri ResolvedUri
        {
            get
            {
                if (_resolvedUri != null)
                    return _resolvedUri;
                else if (_parent != null)
                    return _parent.ResolvedUri;
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets the parent Entity of this Entity.
        /// </summary>
        public Entity Parent
        {
            get
            {
                return _parent;
            }
        }

        /// <summary>
        /// The last character read from the 
        /// input stream for this entity.
        /// </summary>
        public char LastChar
        {
            get
            {
                return _lastChar;
            }
        }

        /// <summary>
        /// The line on which this entity was defined.
        /// </summary>
        public int Line
        {
            get
            {
                return _line;
            }
        }

        /// <summary>
        /// The index into the line 
        /// where this entity is defined.
        /// </summary>
        public int LinePosition
        {
            get
            {
                return (_absolutePosition - _lineStart + 1);
            }
        }

        /// <summary>
        /// Whether this entity is an internal entity or not.
        /// </summary>
        /// <value>true if this entity is 
        /// internal, otherwise false.</value>
        public bool IsInternal
        {
            get
            {
                return _isInternal;
            }
        }

        /// <summary>
        /// The literal value of this entity.
        /// </summary>
        public string Literal
        {
            get
            {
                return _literal;
            }
        }

        /// <summary>
        /// The <see cref="LiteralType"/> of this entity.
        /// </summary>
        public LiteralType LiteralType
        {
            get
            {
                return _literalType;
            }
        }

        /// <summary>
        /// Whether the last char read for 
        /// this entity is a whitespace character.
        /// </summary>
        public bool IsWhitespace
        {
            get
            {
                return _isWhitespace;
            }
        }

        /// <summary>
        /// The proxy server to use when making 
        /// web requests to resolve entities.
        /// </summary>
        public string Proxy
        {
            get
            {
                return _proxy;
            }
        }

        /// <summary>
        /// Gets the character encoding for this entity.
        /// </summary>
        public Encoding Encoding
        {
            get
            {
                return _encoding;
            }
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Checks whether a token denotes a literal entity or not.
        /// </summary>
        /// <param name="token">The token to check.</param>
        /// <returns>true if the token is "CDATA", "SDATA" or "PI", otherwise false.</returns>
        public static bool IsLiteralType(string token)
        {
            return (String.Equals(token, "CDATA", StringComparison.InvariantCultureIgnoreCase) ||
                   String.Equals(token, "SDATA", StringComparison.InvariantCultureIgnoreCase) ||
                   String.Equals(token, "PI", StringComparison.InvariantCultureIgnoreCase));
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Begins processing an entity.
        /// </summary>
        /// <param name="parent">The parent of this entity.</param>
        /// <param name="baseUri">The base Uri for processing this entity within.</param>
        public void Open(Entity parent, Uri baseUri)
        {
            _parent = parent;
            if (parent != null)
                _isHtml = parent.IsHtml;

            _line = 1;
            if (_isInternal)
            {
                if (_literal != null)
                    _reader = new StringReader(_literal);
            }
            else if (_uri == null)
            {
                throw Error.UnresolvableSgmlEntity(_name);
            }
            else
            {
                if (baseUri != null)
                    _resolvedUri = new Uri(baseUri, _uri);
                else
                    _resolvedUri = new Uri(_uri);                

                Stream stream = null;
                _encoding = Encoding.Default;

                switch (_resolvedUri.Scheme)
                {
                    case "file":
                        stream = new FileStream(_resolvedUri.LocalPath, FileMode.Open, FileAccess.Read);
                        break;
                    default:
                        HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(ResolvedUri);
                        wr.UserAgent = _userAgent;
                        wr.Timeout = 10000;
                        if (_proxy != null)
                            wr.Proxy = new WebProxy(_proxy);

                        wr.PreAuthenticate = false;
                        // Pass the credentials of the process. 
                        wr.Credentials = CredentialCache.DefaultCredentials;

                        WebResponse resp = wr.GetResponse();
                        Uri actual = resp.ResponseUri;
                        if (!String.Equals(actual.AbsoluteUri, _resolvedUri.AbsoluteUri, 
                            StringComparison.InvariantCultureIgnoreCase))
                            _resolvedUri = actual;
                        
                        string contentType = resp.ContentType.ToLower(CultureInfo.InvariantCulture);
                        string mimeType = contentType;
                        int i = contentType.IndexOf(';');
                        if (i >= 0)
                            mimeType = contentType.Substring(0, i);

                        if (String.Equals(mimeType, "text/html", StringComparison.InvariantCultureIgnoreCase))
                            _isHtml = true;

                        i = contentType.IndexOf("charset");
                        if (i >= 0)
                        {
                            int j = contentType.IndexOf("=", i);
                            int k = contentType.IndexOf(";", j);
                            if (k < 0)
                                k = contentType.Length;

                            if (j > 0)
                            {
                                j++;
                                string charset = contentType.Substring(j, k - j).Trim();
                                try
                                {
                                    _encoding = Encoding.GetEncoding(charset);
                                }
                                catch (ArgumentException) { }
                            }
                        }

                        stream = resp.GetResponseStream();
                        break;
                }

                _ownedStream = true;
                _reader = new HtmlStream(stream, _encoding);
            }
        }

        /// <summary>
        /// Closes the reader from which the entity is being read.
        /// </summary>
        public void Close()
        {
            if (_ownedStream)
                _reader.Close();
        }

        /// <summary>
        /// Reads the next character from the DTD stream.
        /// </summary>
        /// <returns>The next character from the DTD stream.</returns>
        public char ReadChar()
        {
            char ch = (char)_reader.Read();
            if (ch == 0)
                ch = ' ';
            
            _absolutePosition++;
            if (ch == 0xa)
            {
                _isWhitespace = true;
                _lineStart = _absolutePosition + 1;
                _line++;
            }
            else if (ch == ' ' || ch == '\t')
            {
                _isWhitespace = true;
                if (_lastChar == 0xd)
                {
                    _lineStart = _absolutePosition;
                    _line++;
                }
            }
            else if (ch == 0xd)
            {
                _isWhitespace = true;
            }
            else
            {
                _isWhitespace = false;
                if (_lastChar == 0xd)
                {
                    _line++;
                    _lineStart = _absolutePosition;
                }
            }
            _lastChar = ch;
            return ch;
        }

        public string Context()
        {
            Entity parent = this;
            StringBuilder builder = new StringBuilder();

            while (parent != null)
            {
                if (parent._isInternal)
                    builder.AppendFormat("\nReferenced on line {0}, position {1} of internal entity '{2}'", 
                        parent.Line, parent.LinePosition, parent.Name);
                else
                    builder.AppendFormat("\nReferenced on line {0}, position {1} of '{2}' entity at [{3}]", 
                        parent.Line, parent.LinePosition, parent.Name, parent.ResolvedUri.AbsolutePath);
                
                parent = parent.Parent;
            }
            return builder.ToString();
        }

        /// <summary>
        /// Returns the next character after any whitespace.
        /// </summary>
        /// <returns>The next character that is not whitespace.</returns>
        public char SkipWhitespace()
        {
            char ch = _lastChar;
            while (ch != Entity.EOF && (ch == ' ' || ch == '\r' || ch == '\n' || ch == '\t'))
                ch = ReadChar();
            
            return ch;
        }

        /// <summary>
        /// Scans a token from the input stream and returns the result.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use to process the token.</param>
        /// <param name="term">A set of characters to look for as terminators for the token.</param>
        /// <param name="nameToken">true if the token should be a NameToken, otherwise false.</param>
        /// <returns>The scanned token.</returns>
        public string ScanToken(StringBuilder sb, string term, bool nameToken)
        {
            Precondition.Require(sb, () => Error.ArgumentNull("sb"));
            Precondition.Require(term, () => Error.ArgumentNull("term"));
            
            sb.Length = 0;
            char ch = _lastChar;

            if (nameToken && ch != '_' && !Char.IsLetter(ch))
                throw Error.InvalidNameStartCharacter(ch);

            while (ch != Entity.EOF && term.IndexOf(ch) < 0)
            {
                if (!nameToken || ch == '_' || ch == '.' || ch == '-' || ch == ':' || Char.IsLetterOrDigit(ch))
                    sb.Append(ch);
                else
                    throw Error.InvalidNameCharacter(ch);
                
                ch = ReadChar();
            }
            return sb.ToString();
        }

        /// <summary>
        /// Read a literal from the input stream.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use to build the literal.</param>
        /// <param name="quote">The delimiter for the literal.</param>
        /// <returns>The literal scanned from the input stream.</returns>
        public string ScanLiteral(StringBuilder sb, char quote)
        {
            Precondition.Require(sb, () => Error.ArgumentNull("sb"));
            
            sb.Length = 0;
            char ch = ReadChar();

            while (ch != Entity.EOF && ch != quote)
            {
                if (ch == '&')
                {
                    ch = ReadChar();
                    if (ch == '#')
                    {
                        string charent = ExpandCharEntity();
                        sb.Append(charent);
                        ch = _lastChar;
                    }
                    else
                    {
                        sb.Append('&');
                        sb.Append(ch);
                        ch = ReadChar();
                    }
                }
                else
                {
                    sb.Append(ch);
                    ch = ReadChar();
                }
            }

            ReadChar(); // consume end quote.
            return sb.ToString();
        }

        /// <summary>
        /// Reads input until the end of the input stream or until a string of terminator characters is found.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use to build the string.</param>
        /// <param name="type">The type of the element being read (only used in reporting errors).</param>
        /// <param name="terminators">The string of terminator characters to look for.</param>
        /// <returns>The string read from the input stream.</returns>
        public string ScanToEnd(StringBuilder sb, string type, string terminators)
        {
            Precondition.Require(sb, () => Error.ArgumentNull("sb"));
            Precondition.Require(terminators, () => Error.ArgumentNull("terminators"));

            if (sb != null)
                sb.Length = 0;

            // This method scans over a chunk of text looking for the
            // termination sequence specified by the 'terminators' parameter.
            int start = _line;
            char ch = ReadChar();
            int state = 0;
            char next = terminators[state];
            while (ch != Entity.EOF)
            {
                if (ch == next)
                {
                    state++;
                    if (state >= terminators.Length)
                        break;

                    next = terminators[state];
                }
                else if (state > 0)
                {
                    // char didn't match, so go back and see how much does still match.
                    int i = state - 1;
                    int newstate = 0;
                    while (i >= 0 && newstate == 0)
                    {
                        if (terminators[i] == ch)
                        {
                            // character is part of the terminators pattern, ok, so see if we can
                            // match all the way back to the beginning of the pattern.
                            int j = 1;
                            while (i - j >= 0)
                            {
                                if (terminators[i - j] != terminators[state - j])
                                    break;

                                j++;
                            }

                            if (j > i)
                            {
                                newstate = i + 1;
                            }
                        }
                        else
                        {
                            i--;
                        }
                    }

                    if (sb != null)
                    {
                        i = (i < 0) ? 1 : 0;
                        for (int k = 0; k <= state - newstate - i; k++)
                        {
                            sb.Append(terminators[k]);
                        }

                        if (i > 0) // see if we've matched this char or not
                            sb.Append(ch); // if not then append it to buffer.
                    }

                    state = newstate;
                    next = terminators[newstate];
                }
                else
                {
                    if (sb != null)
                        sb.Append(ch);
                }

                ch = ReadChar();
            }

            if (ch == 0)
                throw Error.UnclosedElement(type, start);

            ReadChar(); // consume last char in termination sequence.
            if (sb != null)
                return sb.ToString();
            
            return String.Empty;
        }

        /// <summary>
        /// Expands a character entity to be read from the input stream.
        /// </summary>
        /// <returns>The string for the character entity.</returns>
        public string ExpandCharEntity()
        {
            char ch = ReadChar();
            int v = 0;
            if (ch == 'x')
            {
                ch = ReadChar();
                for (; ch != Entity.EOF && ch != ';'; ch = ReadChar())
                {
                    int p = 0;
                    if (ch >= '0' && ch <= '9')
                        p = (int)(ch - '0');
                    else if (ch >= 'a' && ch <= 'f')
                        p = (int)(ch - 'a') + 10;
                    else if (ch >= 'A' && ch <= 'F')
                        p = (int)(ch - 'A') + 10;
                    else
                        break;

                    v = (v * 16) + p;
                }
            }
            else
            {
                for (; ch != Entity.EOF && ch != ';'; ch = ReadChar())
                {
                    if (ch >= '0' && ch <= '9')
                        v = (v * 10) + (int)(ch - '0');
                    else
                        break;
                }
            }

            if (ch == 0)
                throw Error.CouldNotParseEntityReference(ch);
            else if (ch == ';')
                ReadChar();

            // HACK ALERT: IE and Netscape map the unicode characters 
            if (_isHtml && v >= 0x80 & v <= 0x9F)
            {
                // This range of control characters is mapped to Windows-1252!
				#pragma warning disable 0219
                int size = _ctrlMap.Length;
				#pragma warning restore 0219
                int i = v - 0x80;
                int unicode = _ctrlMap[i];

                return Convert.ToChar(unicode).ToString();
            }
            return Convert.ToChar(v).ToString();
        }

        /// <summary>
        /// Sets the entity to be a literal of the type specified.
        /// </summary>
        /// <param name="token">One of "CData", "SData" or "PI".</param>
        public void SetLiteralType(string token)
        {
            switch (token)
            {
                case "CDATA":
                    _literalType = LiteralType.CData;
                    break;
                case "SDATA":
                    _literalType = LiteralType.SData;
                    break;
                case "PI":
                    _literalType = LiteralType.ProcessingInstruction;
                    break;
                default:
                    throw Error.InvalidLiteralTypeValue(token);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. 
        /// </summary>
        /// <param name="isDisposing">true if this method has been called by user code, false if it has been called through a finalizer.</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                if (_reader != null)
                    _reader.Dispose();
            }
			_reader = null;
        }
        #endregion
    }
}

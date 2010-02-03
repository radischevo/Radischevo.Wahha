using System;
using System.IO;
using System.Text;
using System.Globalization;

namespace Radischevo.Wahha.Web.Text.Sgml
{
    internal class HtmlStream : TextReader
    {
        #region Constants
        private const int BUFSIZE = 16384;
        private const int EOF = -1;
        #endregion

        #region Instance Fields
        private char[] _buffer;
        private Decoder _decoder;
        private Encoding _encoding;        
        private int _position;
        private byte[] _rawBuffer;
        private int _rawPosition;
        private int _rawUsed;
        private Stream _stream;
        private int _used;
        #endregion

        #region Constructors
        public HtmlStream(Stream stream, Encoding defaultEncoding)
        {
            if (defaultEncoding == null) 
                defaultEncoding = Encoding.UTF8; // default is UTF8

            if (!stream.CanSeek)                
                stream = CopyToMemoryStream(stream);
            
            _stream = stream;
            _rawBuffer = new Byte[BUFSIZE];
            _rawUsed = stream.Read(_rawBuffer, 0, 4); // maximum byte order mark
            _buffer = new char[BUFSIZE];

            // Check byte order marks
            _decoder = AutoDetectEncoding(_rawBuffer, ref _rawPosition, _rawUsed);
            int bom = _rawPosition;
            
            if (_decoder == null)
            {
                _decoder = defaultEncoding.GetDecoder();
                _rawUsed += stream.Read(_rawBuffer, 4, BUFSIZE - 4);
                DecodeBlock();

                // Now sniff to see if there is an XML declaration or HTML <META> tag.
                Decoder sd = SniffEncoding();
                if (sd != null)
                    _decoder = sd;
            }

            // Reset to get ready for Read()
            _stream.Seek(0, SeekOrigin.Begin);
            _position = _used = 0;
            
            // skip bom
            if (bom > 0)
                _stream.Read(_rawBuffer, 0, bom);

            _rawPosition = _rawUsed = 0;
        }
        #endregion

        #region Instance Properties
        public Encoding Encoding
        {
            get
            {
                return _encoding;
            }
        }
        #endregion

        #region Static Methods
        private static Stream CopyToMemoryStream(Stream s)
        {
            int size = 100000; // large heap is more efficient
            byte[] copyBuff = new byte[size];
            int count;

            MemoryStream ms = new MemoryStream();
            while ((count = s.Read(copyBuff, 0, size)) > 0)
                ms.Write(copyBuff, 0, count);

            ms.Seek(0, SeekOrigin.Begin);
            s.Close();

            return ms;
        }

        private static Decoder AutoDetectEncoding(byte[] buffer, 
            ref int index, int length)
        {
            if (4 <= (length - index))
            {
                uint w = (uint)buffer[index + 0] << 24 | (uint)buffer[index + 1] << 16 | 
                    (uint)buffer[index + 2] << 8 | (uint)buffer[index + 3];
                // see if it's a 4-byte encoding
                switch (w)
                {
                    case 0xfefffeff:
                        index += 4;
                        return new Ucs4DecoderBigEndian();

                    case 0xfffefffe:
                        index += 4;
                        return new Ucs4DecoderLittleEndian();

                    case 0x3c000000:
                        goto case 0xfefffeff;

                    case 0x0000003c:
                        goto case 0xfffefffe;
                }
                w >>= 8;
                if (w == 0xefbbbf)
                {
                    index += 3;
                    return Encoding.UTF8.GetDecoder();
                }

                w >>= 8;
                switch (w)
                {
                    case 0xfeff:
                        index += 2;
                        return UnicodeEncoding.BigEndianUnicode.GetDecoder();
                    case 0xfffe:
                        index += 2;
                        return new UnicodeEncoding(false, false).GetDecoder();
                    case 0x3c00:
                        goto case 0xfeff;

                    case 0x003c:
                        goto case 0xfffe;
                }
            }
            return null;
        }
        #endregion

        #region Instance Methods
        public override void Close()
        {
            _stream.Close();
        }

        private int ReadChar()
        {
            if (_position < _used) 
                return _buffer[_position++];

            return EOF;
        }

        internal void DecodeBlock()
        {
            // shift current chars to beginning.
            if (_position > 0)
            {
                if (_position < _used)
                    Array.Copy(_buffer, _position, _buffer, 0, _used - _position);
                
                _used -= _position;
                _position = 0;
            }

            int len = _decoder.GetCharCount(_rawBuffer, _rawPosition, _rawUsed - _rawPosition);
            int available = _buffer.Length - _used;

            if (available < len)
            {
                char[] newbuf = new char[_buffer.Length + len];
                Array.Copy(_buffer, _position, newbuf, 0, _used - _position);
                _buffer = newbuf;
            }

            _used = _position + _decoder.GetChars(_rawBuffer, _rawPosition, 
                _rawUsed - _rawPosition, _buffer, _position);
            _rawPosition = _rawUsed; // consumed the whole buffer!
        }

        private int PeekChar()
        {
            int ch = ReadChar();
            if (ch != EOF)
                _position--;
            
            return ch;
        }

        private bool SniffPattern(string pattern)
        {
            int ch = PeekChar();
            if (ch != pattern[0]) 
                return false;

            for (int i = 0, n = pattern.Length; 
                ch != EOF && i < n; i++)
            {
                ch = ReadChar();
                char m = pattern[i];
                if (ch != m)
                    return false;
            }
            return true;
        }

        private void SniffWhitespace()
        {
            char ch = (char)PeekChar();
            while (ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n')
            {
                int i = _position;
                ch = (char)ReadChar();

                if (ch != ' ' && ch != '\t' && ch != '\r' && ch != '\n')
                    _position = i;
            }
        }

        private string SniffLiteral()
        {
            int quoteChar = PeekChar();
            if (quoteChar == '\'' || quoteChar == '"')
            {
                ReadChar();// consume quote char
                int i = _position;
                int ch = ReadChar();

                while (ch != EOF && ch != quoteChar)
                    ch = ReadChar();
                
                return (_position > i) ? new String(_buffer, i, _position - i - 1) : String.Empty;
            }
            return null;
        }

        private string SniffAttribute(string name)
        {
            SniffWhitespace();
            string id = SniffName();

            if (String.Equals(name, id, 
                StringComparison.InvariantCultureIgnoreCase))
            {
                SniffWhitespace();
                if (SniffPattern("="))
                {
                    SniffWhitespace();
                    return SniffLiteral();
                }
            }
            return null;
        }

        private string SniffAttribute(out string name)
        {
            SniffWhitespace();
            name = SniffName();

            if (name != null)
            {
                SniffWhitespace();
                if (SniffPattern("="))
                {
                    SniffWhitespace();
                    return SniffLiteral();
                }
            }
            return null;
        }

        private void SniffTerminator(string term)
        {
            int ch = ReadChar();
            int i = 0;
            int n = term.Length;

            while (i < n && ch != EOF)
            {
                if (term[i] == ch)
                {
                    i++;
                    if (i == n) break;
                }
                else
                {
                    i = 0; // reset.
                }
                ch = ReadChar();
            }
        }

        internal Decoder SniffEncoding()
        {
            Decoder decoder = null;
            if (SniffPattern("<?xml"))
            {
                string version = SniffAttribute("version");
                if (version != null)
                {
                    string encoding = SniffAttribute("encoding");
                    if (encoding != null)
                    {
                        try
                        {
                            Encoding enc = Encoding.GetEncoding(encoding);
                            if (enc != null)
                            {
                                _encoding = enc;
                                return enc.GetDecoder();
                            }
                        }
                        catch (ArgumentException) { }
                    }
                    SniffTerminator(">");
                }
            }
            
            if (decoder == null)
                return SniffMeta();
            
            return null;
        }

        internal Decoder SniffMeta()
        {
            int i = ReadChar();
            while (i != EOF)
            {
                char ch = (char)i;
                if (ch == '<')
                {
                    string name = SniffName();
                    if (name != null && String.Equals(name, "meta", 
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        string httpequiv = null;
                        string content = null;
                        while (true)
                        {
                            string value = SniffAttribute(out name);
                            if (name == null)
                                break;

                            if (String.Equals(name, "http-equiv", 
                                StringComparison.InvariantCultureIgnoreCase))
                                httpequiv = value;
                            else if (String.Equals(name, "content", 
                                StringComparison.InvariantCultureIgnoreCase))
                                content = value;
                        }

                        if (httpequiv != null && String.Equals(httpequiv, "content-type", 
                            StringComparison.InvariantCultureIgnoreCase) && content != null)
                        {
                            int j = content.IndexOf("charset");
                            if (j >= 0)
                            {
                                //charset=utf-8
                                j = content.IndexOf("=", j);
                                if (j >= 0)
                                {
                                    j++;
                                    int k = content.IndexOf(";", j);
                                    if (k < 0) k = content.Length;
                                    string charset = content.Substring(j, k - j).Trim();
                                    try
                                    {
                                        _encoding = Encoding.GetEncoding(charset);
                                        return _encoding.GetDecoder();
                                    }
                                    catch (ArgumentException) { }
                                }
                            }
                        }
                    }
                }
                i = ReadChar();
            }
            return null;
        }

        internal string SniffName()
        {
            int c = PeekChar();
            if (c == EOF)
                return null;

            char ch = (char)c;
            int start = _position;
            while (_position < _used - 1 && (Char.IsLetterOrDigit(ch) || 
                ch == '-' || ch == '_' || ch == ':'))
                ch = _buffer[++_position];

            if (start == _position)
                return null;

            return new String(_buffer, start, _position - start);
        }

        internal void SkipWhitespace()
        {
            char ch = (char)PeekChar();
            while (_position < _used - 1 && (ch == ' ' || 
                ch == '\r' || ch == '\n'))
                ch = _buffer[++_position];
        }

        internal void SkipTo(char what)
        {
            char ch = (char)PeekChar();
            while (_position < _used - 1 && (ch != what))
                ch = _buffer[++_position];
        }

        internal string ParseAttribute()
        {
            SkipTo('=');
            if (_position < _used)
            {
                _position++;
                SkipWhitespace();

                if (_position < _used)
                {
                    char quote = _buffer[_position];
                    _position++;

                    int start = _position;
                    SkipTo(quote);
                    if (_position < _used)
                    {
                        string result = new String(_buffer, start, _position - start);
                        _position++;

                        return result;
                    }
                }
            }
            return null;
        }

        public override int Peek()
        {
            int result = Read();
            if (result != EOF)
                _position--;
            
            return result;
        }

        public override int Read()
        {
            if (_position == _used)
            {
                _rawUsed = _stream.Read(_rawBuffer, 0, _rawBuffer.Length);
                _rawPosition = 0;
                
                if (_rawUsed == 0) 
                    return EOF;

                DecodeBlock();
            }
            if (_position < _used) 
                return _buffer[_position++];

            return -1;
        }

        public override int Read(char[] buffer, int start, int length)
        {
            if (_position == _used)
            {
                _rawUsed = _stream.Read(_rawBuffer, 0, _rawBuffer.Length);
                _rawPosition = 0;

                if (_rawUsed == 0) 
                    return -1;

                DecodeBlock();
            }
            if (_position < _used)
            {
                length = Math.Min(_used - _position, length);
                Array.Copy(_buffer, _position, buffer, start, length);
                _position += length;

                return length;
            }
            return 0;
        }

        public override int ReadBlock(char[] data, int index, int count)
        {
            return Read(data, index, count);
        }

        public int ReadLine(char[] buffer, int start, int length)
        {
            int i = 0;
            int ch = ReadChar();
            while (ch != EOF)
            {
                buffer[i + start] = (char)ch;
                i++;
                if (i + start == length)
                    break; // buffer is full

                if (ch == '\r')
                {
                    if (PeekChar() == '\n')
                    {
                        ch = ReadChar();
                        buffer[i + start] = (char)ch;
                        i++;
                    }
                    break;
                }
                else if (ch == '\n')
                {
                    break;
                }
                ch = ReadChar();
            }
            return i;
        }

        public override string ReadToEnd()
        {
            char[] buffer = new char[100000]; // large block heap is more efficient
            int len = 0;
            StringBuilder sb = new StringBuilder();

            while ((len = Read(buffer, 0, buffer.Length)) > 0)
                sb.Append(buffer, 0, len);
            
            return sb.ToString();
        }
        #endregion
    }
}

using System;
using System.IO;
using System.Text;
using System.Web;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    internal class CaptureFilter : Stream
    {
        #region Instance Fields
        private long _position;
        private Stream _stream;
        private MemoryStream _buffer;
        #endregion

        #region Constructors
        public CaptureFilter(Stream stream)
        {
            Precondition.Require(stream, Error.ArgumentNull("stream"));

            _stream = stream;
            _buffer = new MemoryStream();
        }
        #endregion

        #region Instance Properties
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override long Length
        {
            get
            {
                return 0;
            }
        }

        public override long Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }
        #endregion

        #region Instance Methods
        public override long Seek(long offset, SeekOrigin direction)
        {
            return 0;
        }

        public override void SetLength(long length)
        {
            _stream.SetLength(length);
        }

        public override void Close()
        {
            _stream.Close();
            _buffer.Close();
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _buffer.Write(buffer, 0, count);
        }

        public string GetContents(Encoding encoding)
        {
            byte[] buffer = new byte[_buffer.Length];
            _buffer.Position = 0;
            _buffer.Read(buffer, 0, buffer.Length);

            return encoding.GetString(buffer, 0, buffer.Length);
        }
        #endregion
    }
}

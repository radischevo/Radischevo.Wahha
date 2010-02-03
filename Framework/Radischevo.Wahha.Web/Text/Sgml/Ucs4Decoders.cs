using System;
using System.Text;

namespace Radischevo.Wahha.Web.Text.Sgml
{
    internal class Ucs4DecoderBigEndian : Ucs4Decoder
    {
        #region Constructors
        public Ucs4DecoderBigEndian() : base()
        {   }
        #endregion

        #region Instance Methods
        internal override int GetFullChars(byte[] bytes, int byteIndex, 
            int byteCount, char[] chars, int charIndex)
        {
            byteCount += byteIndex;
            int index = byteIndex;
            int count = charIndex;

            while ((index + 3) < byteCount)
            {
                uint code = (uint)((((bytes[index + 3] << 0x18) | (bytes[index + 2] << 0x10)) | (bytes[index + 1] << 8)) | bytes[index]);
                if (code > 0x10ffff)
                    throw Error.InvalidCharacterInEncoding(code);
                
                if (code > 0xffff)
                {
                    chars[count] = base.UnicodeToUTF16(code);
                    count++;
                }
                else
                {
                    if ((code >= 0xd800) && (code <= 0xdfff))
                        throw Error.InvalidCharacterInEncoding(code);
                    
                    chars[count] = (char)code;
                }
                count++;
                index += 4;
            }
            return (count - charIndex);
        }
        #endregion
    }

    internal class Ucs4DecoderLittleEndian : Ucs4Decoder
    {
        #region Constructors
        public Ucs4DecoderLittleEndian() : base()
        {   }
        #endregion

        #region Instance Methods
        internal override int GetFullChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            byteCount += byteIndex;
            int index = byteIndex;
            int count = charIndex;

            while ((index + 3) < byteCount)
            {
                uint code = (uint)((((bytes[index] << 0x18) | (bytes[index + 1] << 0x10)) | (bytes[index + 2] << 8)) | bytes[index + 3]);
                if (code > 0x10ffff)
                    throw Error.InvalidCharacterInEncoding(code);
                
                if (code > 0xffff)
                {
                    chars[count] = base.UnicodeToUTF16(code);
                    count++;
                }
                else
                {
                    if ((code >= 0xd800) && (code <= 0xdfff))
                        throw Error.InvalidCharacterInEncoding(code);
                    
                    chars[count] = (char)code;
                }
                count++;
                index += 4;
            }
            return (count - charIndex);
        }
        #endregion
    }
}

using System;
using System.Text;

namespace Radischevo.Wahha.Web.Text.Sgml
{
    internal abstract class Ucs4Decoder : Decoder
    {
        #region Instance Fields
        internal byte[] _temp;
        internal int _tempBytes;
        #endregion

        #region Constructors
        protected Ucs4Decoder() 
            : base()
        {
            _temp = new byte[4];
        }
        #endregion

        #region Instance Methods
        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return ((count + _tempBytes) / 4);
        }

        internal abstract int GetFullChars(byte[] bytes, int byteIndex, 
            int byteCount, char[] chars, int charIndex);

        public override int GetChars(byte[] bytes, int byteIndex, 
            int byteCount, char[] chars, int charIndex)
        {
            int tempBytes = _tempBytes;
            if (_tempBytes > 0)
            {
                while (tempBytes < 4)
                {
                    _temp[tempBytes] = bytes[byteIndex];
                    byteIndex++;
                    byteCount--;
                    tempBytes++;
                }
                tempBytes = 1;
                GetFullChars(_temp, 0, 4, chars, charIndex);
                charIndex++;
            }
            else
            {
                tempBytes = 0;
            }

            tempBytes = GetFullChars(bytes, byteIndex, byteCount, chars, charIndex) + tempBytes;
            int index = (_tempBytes + byteCount) % 4;
            byteCount += byteIndex;
            byteIndex = byteCount - index;
            _tempBytes = 0;

            if (byteIndex >= 0)
            {
                while (byteIndex < byteCount)
                {
                    _temp[_tempBytes] = bytes[byteIndex];
                    _tempBytes++;
                    byteIndex++;
                }
            }
            return tempBytes;
        }

        internal char UnicodeToUTF16(uint code)
        {
            byte num = (byte)(0xd7c0 + (code >> 10));
            byte num2 = (byte)(0xdc00 | (code & 0x3ff));
            return (char)((num2 << 8) | num);
        }
        #endregion
    }
}

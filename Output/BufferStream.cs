using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Output
{
    class BufferStream : OutputStream
    {

        public readonly byte[] Buffer;
        public int Index { get; private set; } = 0;
       
        public BufferStream(int size = 1024)
        {
            Buffer = new byte[size];
        }

        public override void WriteByte(byte b)
            => Buffer[Index++] = b;

        public override void WriteString(string s = "")
        {
            foreach (char c in s)
            {
                WriteByte((byte)(c & 0xFF));
                WriteByte((byte)(c >> 8));
            }
        }

        public void WriteToStream(OutputStream stream)
            => stream.WriteBytes(Buffer, 0, Index);


    }
}

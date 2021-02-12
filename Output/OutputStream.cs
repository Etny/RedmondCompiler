using System;
using System.IO;

namespace Redmond.Output
{
    abstract class OutputStream 
    {
        protected int currentIndent = 0;


        public virtual void WriteByte(byte b)
             => throw new NotImplementedException();

        public virtual void WriteBytes(params byte[] bytes)
        { 
            foreach (byte b in bytes) 
                WriteByte(b);
        }

        public virtual void WriteBytes(byte[] bytes, int start = 0, int length = 0)
        {
            for (int i = start; start < start + length; i++)
                WriteByte(bytes[i]);
        }

        public virtual void WriteString(string s = "") 
            => throw new NotImplementedException();
        public virtual void WriteLine(string s = "")
            => WriteString(s + '\n');

        public virtual void AddIndentation(int indent = 1)
            => currentIndent += indent;
        public virtual void ReduceIndentation(int indent = 1)
            => currentIndent -= indent;

    }
}
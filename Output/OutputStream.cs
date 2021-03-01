using System;
using System.IO;

namespace Redmond.Output
{
    abstract class OutputStream 
    {
        protected int currentIndent = 0;



        public virtual void WriteString(string s = "") 
            => throw new NotImplementedException();
        public virtual void WriteLine(string s = "")
            => WriteString(s + '\n');

        public virtual void WriteStringAtLocation(int location, string s = "")
            => throw new NotImplementedException();

        public virtual int ReserveLocation()
            => throw new NotImplementedException();

        public virtual void AddIndentation(int indent = 1)
            => currentIndent += indent;
        public virtual void ReduceIndentation(int indent = 1)
            => currentIndent -= indent;

    }
}
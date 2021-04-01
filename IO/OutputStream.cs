using System;
using System.IO;

namespace Redmond.IO
{
    abstract class OutputStream 
    {
        protected int currentIndent = 0;
        protected int nextIndent = 0;



        public virtual void OpenStream() { }
        public virtual void CloseStream() { }


        protected virtual string GetIndentation()
            => new string('\t', currentIndent < 0 ? 0 : currentIndent);

        public virtual void WriteString(string s = "") 
            => throw new NotImplementedException();
        public virtual void WriteLine(string s = "") { WriteString(s + '\n'); currentIndent = nextIndent; }

        public virtual void WriteStringAtLocation(int location, string s = "")
            => throw new NotImplementedException();

        public virtual int ReserveLocation()
            => throw new NotImplementedException();

        public virtual void AddIndentation(int indent = 1) { currentIndent += indent; nextIndent = currentIndent; }
        public virtual void ReduceIndentation(int indent = 1) { currentIndent -= indent; nextIndent = currentIndent; }

        public virtual void ReduceIndentationForLine(int indent = 1)
            => currentIndent -= indent;

        public virtual void AddIndentationForLine(int indent = 1)
            => currentIndent += indent;

    }
}
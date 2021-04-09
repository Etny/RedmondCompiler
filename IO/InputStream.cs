using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.IO
{
    abstract class InputStream
    {

        public bool AtEnd { get; protected set; } = false;
        protected int Position = 0;

        public virtual void OpenStream() { }
        public virtual void CloseStream() { }

        public abstract char CurrentChar(int offset = 0);
        public abstract string GetSubString(int length);

        public virtual string GetInfo() { return ""; }

        public virtual void Advance(int advance)
            => Position += advance;
    }
}

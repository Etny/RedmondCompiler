using System;

namespace Redmond.Output
{
    class ConsoleStream : OutputStream
    {

        private bool wroteIndent = false;

        private string GetIndentation()
            => new string('\t', currentIndent < 0 ? 0 : currentIndent);

        public override void WriteString(string s = "")
        {
            if (!wroteIndent) Console.Write(GetIndentation());
            wroteIndent = true;
            Console.Write(s);
        }

        public override void WriteLine(string s = "")
        {
            if(!wroteIndent) Console.Write(GetIndentation());
            Console.WriteLine(s);
            wroteIndent = false;
        }

        public override void WriteByte(byte b)
            => Console.Write(GetIndentation() + b);
    }
}

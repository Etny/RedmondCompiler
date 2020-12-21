using System;

namespace Redmond.Output
{
    class ConsoleStream : IStringStream
    {
        private int _indent = -1;

        public void AddIndentation(int indent = 1)
            => _indent += indent;


        public void ReduceIndentation(int indent = 1)
            => _indent -= indent;

        private string GetIndentation()
            => new string('\t', _indent);

        public void Write(string s = "")
            => Console.Write(GetIndentation() + s);

        public void WriteLine(string s = "")
            => Console.WriteLine(GetIndentation() + s);
    }
}

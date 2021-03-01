using System;

namespace Redmond.Output
{
    class ConsoleStream : OutputStream
    {

        private bool wroteIndent = false;
        private int startLoc = 0;
        private int currentLoc = 0;

        public ConsoleStream() { startLoc = Console.CursorTop; }

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
            currentLoc++;
        }

        public override int ReserveLocation()
        {
            WriteLine();
            return currentLoc-1;
        }

        public override void WriteStringAtLocation(int location, string s = "")
        {
            if(location > currentLoc)
            {
                for (int i = location; i < currentLoc; i++)
                    WriteLine();

                WriteLine(s);
            }
            else
            {
                int temp = Console.CursorTop;
                Console.CursorTop = startLoc + location;
                Console.WriteLine(GetIndentation() + s);
                Console.CursorTop = temp;
            }
        }
    }
}

using System;

namespace Redmond.IO
{
    class ConsoleStream : OutputStream
    {

        private bool wroteIndent = false;
        private int startLoc = 0;
        private int currentLoc = 0;

        public ConsoleStream() { startLoc = Console.CursorTop; }

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
            currentIndent = nextIndent;
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

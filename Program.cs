
using Redmond.Lex;
using Redmond.Output;
using Redmond.Parsing;
using System;

namespace Redmond
{
   class Program
    {

        private static string InputString = "9+2*3/(5-2+6)";
        private static TokenStream Input = new TokenStream(InputString);
        private static IStringStream Output = new ConsoleStream();

        static void Main(string[] args)
        {
            new Compiler(Input, Output).StartCompilation();
        }
    }
}

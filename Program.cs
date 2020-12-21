
using Redmond.Lex;
using Redmond.Output;
using Redmond.Parsing;

namespace Redmond
{
    class Program
    {

        private static string InputString = "86 + 3 / (24 + 3 / 2)";
        private static TokenStream Input = new TokenStream(InputString);
        private static IStringStream Output = new ConsoleStream();

        static void Main(string[] args)
        {
            new CompilationContext(Input, Output).Start();
        }
    }
}


using Redmond.Lex;
using Redmond.Lex.LexCompiler;
using Redmond.Lex.LexCompiler.RegexTree;
using Redmond.Output;
using Redmond.Parsing;
using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;

namespace Redmond
{
    class Program
    {

        private static string InputString = @"ifab b 123-123";
        private static TokenStream Input = new TokenStream(InputString, @"C:\Users\yveem\source\repos\Redmond\TestDec.lex", @"123-abcif() ");
        private static IStringStream Output = new ConsoleStream();
        static void Main(string[] args)
        {
            //new CompilationContext(Input, Output).Start();

            Grammar g = new Grammar();
        }
    }
}

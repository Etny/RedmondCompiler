
using Redmond.Common;
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

        private static string InputString = @"-1*1$";
        private static DecFile Dec = new DecFile(@"C:\Users\yveem\source\repos\Redmond\TestDec.lex");
        private static IStringStream Output = new ConsoleStream();
        static void Main(string[] args)
        {
            //new CompilationContext(Input, Output).Start();

            TokenType.AddTypes(Dec.TokenLines);
            TokenStream Input = new TokenStream(InputString, Dec.LexLines, @"*+-1()$");
            Console.WriteLine(new Grammar(Dec.GrammarLines).Parse(Input));
        }
    }
}

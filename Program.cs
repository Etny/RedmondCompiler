﻿
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

        private static readonly string InputString = "12+6*(1+(1*2))\n23+9$";
        private static readonly DecFile Dec = new DecFile(@"C:\Users\yveem\source\repos\Redmond\TestDec.dec");
        //private static IStringStream Output = new ConsoleStream();
        static void Main()
        {
            //new CompilationContext(Input, Output).Start();

            TokenType.AddTypes(Dec.TokenLines);
            TokenStream Input = new TokenStream(InputString, Dec.LexLines, "abc+-/*0123456789()$\n");
            //Console.WriteLine(new Grammar(Dec.GrammarLines).Parse(Input).ToTreeString());
            new Grammar(Dec.GrammarLines).Parse(Input);
        }
    }
}

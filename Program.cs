
using Redmond.Common;
using Redmond.Lex;
using Redmond.Lex.LexCompiler;
using Redmond.Lex.LexCompiler.RegexTree;
using Redmond.Output;
using Redmond.Parsing;
using Redmond.Parsing.CodeGeneration;
using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.IO;

namespace Redmond
{
    class Program
    {

        private static readonly string InputString = File.ReadAllText(@"C:\Users\yveem\source\repos\Redmond\TestInput.txt")+'$';
        private static readonly DecFile Dec = new DecFile(@"C:\Users\yveem\source\repos\Redmond\TestDec.dec");
        //private static IStringStream Output = new ConsoleStream();
        static void Main()
        {
            CompileSettings.InitSettings(Dec.SettingsLines);
            TokenType.AddTypes(Dec.TokenLines);
            TokenStream Input = new TokenStream(InputString, Dec.LexLines, "\"\'[]{}12345678910abcdefghijklmnopqrstuvwxyz_+-=*/:;() $\n\r\t");
            //Console.WriteLine(new Grammar(Dec.GrammarLines).Parse(Input).ToTreeString());
            var tree = new Grammar(Dec.GrammarLines).Parse(Input);
            Console.WriteLine(tree.ToTreeString());
            Console.WriteLine("============\n");
            new CodeGenerator(tree);
        }
    }
}

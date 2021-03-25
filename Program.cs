
using Microsoft.CodeAnalysis;
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
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Redmond
{
    class Program
    {

        private static readonly string InputString = File.ReadAllText(@"C:\Users\yveem\source\repos\Redmond\TestInput.txt")+GrammarConstants.EndChar;
        private static readonly DecFile Dec = new DecFile(@"C:\Users\yveem\source\repos\Redmond\TestDec.dec");
        //private static IStringStream Output = new ConsoleStream();
        static void Main()
        {
            CompileSettings.InitSettings(Dec.SettingsLines);
            TokenType.AddTypes(Dec.TokenLines);
            Console.WriteLine("Generating parsing table...");
            var parser = new Grammar(Dec.GrammarLines).GetParser();
            Console.Write("Finished parsing table, enter file: ");

            string s = Console.ReadLine();

            do
            {
                string inputString = InputString;

                if (s.Trim().Length > 0 && File.Exists(s))
                    inputString = File.ReadAllText(s);

                TokenStream Input = new TokenStream(inputString, Dec.LexLines, "\"\'.,&|?<>[]{}!12345678910abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_+-=*/:;() $\n\r\t"); ;

                parser.Parse(Input);
                var tree = SyntaxTreeNode.CurrentNode;
                Console.WriteLine(tree.ToTreeString());
                Console.WriteLine("============\n");
                new IntermediateGenerator(tree);
                Console.WriteLine("============\n");
                s = Console.ReadLine();
            } while (s != "q");

        }
    }
}

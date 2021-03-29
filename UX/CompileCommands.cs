using Redmond.Common;
using Redmond.Lex;
using Redmond.Parsing.CodeGeneration;
using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Redmond.UX
{
    class CompileCommands
    {

        public static void Compile(ParsedCommandOptions opts)
        {
            var Dec = new DecFile(@"C:\Users\yveem\source\repos\Redmond\TestDec.dec");
            CompileSettings.InitSettings(Dec.SettingsLines);
            TokenType.AddTypes(Dec.TokenLines);
            Console.WriteLine("Generating parsing table...");
            var parser = new Grammar(Dec.GrammarLines).GetParser();

            string inputFile = opts.FindOption("input", "i")?.Argument ??
                @"C:\Users\yveem\source\repos\Redmond\TestInput.txt";

            string inputString = File.ReadAllText(inputFile) + GrammarConstants.EndChar;

            TokenStream Input = new TokenStream(inputString, Dec.LexLines, new string(Enumerable.Range('\x1', 127).Select(i => (char)i).ToArray()));

            parser.Parse(Input);
            var tree = SyntaxTreeNode.CurrentNode;
            Console.WriteLine(tree.ToTreeString());
            Console.WriteLine("============\n");
            new IntermediateGenerator(tree);
            Console.WriteLine("============\n");
        }
    }
}

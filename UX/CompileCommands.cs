using Redmond.Common;
using Redmond.Lex;
using Redmond.Parsing;
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
            Console.WriteLine("Reading parse file..");
            
            ParseFile parseFile = new ParseFile(@"C:\Users\yveem\source\repos\Redmond\TestParse.parse").Read();

            string inputFile = opts.FindOption("input", "i")?.Argument ??
                @"C:\Users\yveem\source\repos\Redmond\TestInput.txt";

            var context = new CompilationContext(parseFile, inputFile);
            context.Compile();
        }

        public static void CompileDec(ParsedCommandOptions opts)
        {
            var Dec = new DecFile(@"C:\Users\yveem\source\repos\Redmond\TestDec.dec");
            CompileSettings.InitSettings(Dec.SettingsLines);
            var gram = new DecGrammar(Dec);
            Console.WriteLine("Generating parsing table...");

            ParseFile parseFile = new ParseFile(@"C:\Users\yveem\source\repos\Redmond\TestParse.parse");
            parseFile.SetLexLines(Dec.LexLines);
            parseFile.SetParseTableLines(gram.SerializeParsingTable());
            parseFile.SetTokenIdLines(ProductionEntry.SerializeTags());
            parseFile.Save();

            //var parser = gram.GetParser();

            //string inputFile = opts.FindOption("input", "i")?.Argument ??
            //    @"C:\Users\yveem\source\repos\Redmond\TestInput.txt";

            //string inputString = File.ReadAllText(inputFile) + GrammarConstants.EndChar;

            //TokenStream Input = new TokenStream(inputString, Dec.LexLines, new string(Enumerable.Range('\x1', 127).Select(i => (char)i).ToArray()));

            //parser.Parse(Input);
            //var tree = SyntaxTreeNode.CurrentNode;
            //Console.WriteLine(tree.ToTreeString());
            //Console.WriteLine("============\n");
            //new IntermediateGenerator().Start(tree);
            //Console.WriteLine("============\n");
        }


    }
}

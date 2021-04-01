using Redmond.Common;
using Redmond.IO;
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

            var output = GetSelectedOutput(opts);
            var input = new MultiFileInputStream(new List<string>(new string[] { @"C:\Users\yveem\Documents\C#_Compiler_tests\Multi File test\File1.txt",
                                                                                 @"C:\Users\yveem\Documents\C#_Compiler_tests\Multi File test\File2.txt"}));

            ParseFile parseFile = new ParseFile(@"C:\Users\yveem\source\repos\Redmond\TestParse.parse").Read();

            string inputFile = opts.FindOption("input", "i")?.Argument ??
                @"C:\Users\yveem\source\repos\Redmond\TestInput.txt";

            var context = new CompilationContext(parseFile, input, output);
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
            parseFile.SetTokenIdLines(ProductionEntry.Register.Serialize());
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

        private static OutputStream GetSelectedOutput(ParsedCommandOptions options)
        {
            var o = options.FindOption("output", "out", "o")?.Argument;

            if (o == null || o.ToLower() == "console") return new ConsoleStream();
            else return new FileOutputStream(o);
        }


    }
}

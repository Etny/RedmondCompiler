using Redmond.Common;
using Redmond.IO;
using Redmond.IO.Error;
using Redmond.IO.Error.Exceptions;
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
            var output = GetSelectedOutputStream(opts);
            var input = GetAllCSFiles(GetParameterValue(opts, null, "input", "in", "i"));
            ParseFile parseFile = new ParseFile(GetParameterValue(opts, null, "parse", "p")).Read();

            Console.WriteLine("Reading parse file..");

            var context = new CompilationContext(parseFile, input, output, new CompilationOptions(opts));
            context.Compile();

            Console.WriteLine("Done!");
        }

        public static void CompileDec(ParsedCommandOptions opts)
        {
            var Dec = new DecFile(GetParameterValue(opts, null, "input", "in", "i"));
            ParseFile parseFile = new ParseFile(GetParameterValue(opts, null, "output", "out", "o"));

            CompileSettings.InitSettings(Dec.SettingsLines);
            var gram = new DecGrammar(Dec);
            Console.WriteLine("Generating parsing table... (This will take a while)");

            parseFile.SetLexLines(Dec.LexLines);
            parseFile.SetParseTableLines(gram.SerializeParsingTable());
            parseFile.SetTokenIdLines(ProductionEntry.Register.Serialize());
            parseFile.Save();

            Console.WriteLine("Done!");

            //var parser = gram.GetParser();

            //var input = new MultiFileInputStream(new List<string>(new string[] { @"C:\Users\yveem\source\repos\Redmond\TestInput.txt" }));
            ////var input = GetAllCSFiles(@"C:\Users\yveem\source\repos\CompileTestProject");


            //TokenStream Input = new TokenStream(input, Dec.LexLines, new string(Enumerable.Range('\x1', 127).Select(i => (char)i).ToArray()));

            //parser.Parse(Input);
            //var tree = SyntaxTreeNode.CurrentNode;
            //Console.WriteLine(tree.ToTreeString());
            //Console.WriteLine("============\n");
            //new IntermediateGenerator(new ConsoleStream()).Start(tree);
            //Console.WriteLine("============\n");
        }

        private static OutputStream GetSelectedOutputStream(ParsedCommandOptions options)
        {
            var o = GetParameterValue(options, "console", "output", "out", "o");

            if (o == "console") return new ConsoleStream();
            else return new FileOutputStream(o);
        }

        private static string GetParameterValue(ParsedCommandOptions options, string defaultValue, params string[] names)
        { 
            var o = options.FindOption(names)?.Argument;
            if (o == null)
            {
                if (defaultValue != null)
                    return defaultValue;
                else
                    ErrorManager.ExitWithError(new MissingParameterException(names[0]));
            }
            return o;
        }
        

        private static InputStream GetAllCSFiles(string path)
        {
            List<string> FindAllInFolder(string path)
            {
                List<string> files = new List<string>();

                if (!Directory.Exists(path))
                {
                    if (Path.GetExtension(path) == ".cs")
                        files.Add(path);
                }
                else
                {
                    foreach (var v in Directory.GetFiles(path))
                        if (Path.GetExtension(v) == ".cs")
                            files.Add(v);

                    foreach (var d in Directory.GetDirectories(path))
                        if (Path.GetFileName(d) != "obj") files.AddRange(FindAllInFolder(d));
                }

                return files;
            }

            return new MultiFileInputStream(FindAllInFolder(path));
        }


    }
}

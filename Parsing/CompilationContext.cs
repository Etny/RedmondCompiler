using Redmond.Common;
using Redmond.Lex;
using Redmond.IO;
using Redmond.Parsing.CodeGeneration;
using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Redmond.Parsing
{
    class CompilationContext
    {

        public readonly TokenStream Input;
        public readonly Grammar Grammar;
        public readonly IntermediateGenerator Generator;
        public readonly CompilationOptions Options;

        public CompilationContext(ParseFile file, InputStream input, OutputStream output, CompilationOptions options)
        {
            Options = options;
            Input = new TokenStream(input, file.LexLines.ToBuilder().ToArray(), new string(Enumerable.Range('\x1', 127).Select(i => (char)i).ToArray()));

            ProductionEntry.Register = new ProductionEntryRegister();
            ProductionEntry.Register.ParseSerializedTags(file.TokenIdLines);

            Grammar = new ParseGrammar(file);
            Generator = new IntermediateGenerator(output, options);
        }

        public void Compile()
        {
            Parser parser = Grammar.GetParser();
            parser.Parse(Input);
            if(Options.ParseTree) Console.WriteLine(SyntaxTreeNode.CurrentNode.ToTreeString());
            Generator.Start(SyntaxTreeNode.CurrentNode);
        }

    }
}

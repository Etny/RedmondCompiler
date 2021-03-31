using Redmond.Common;
using Redmond.Lex;
using Redmond.Output;
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
        public readonly IGrammar Grammar;
        public readonly IntermediateGenerator Generator;
        
        public CompilationContext(ParseFile file, string inputPath)
        {
            Input = new TokenStream(File.ReadAllText(inputPath) + GrammarConstants.EndChar, file.LexLines.ToBuilder().ToArray(), new string(Enumerable.Range('\x1', 127).Select(i => (char)i).ToArray()));

            ProductionEntry.ParseTags(file.TokenIdLines);

            Grammar = new ParseGrammar(file);
            Generator = new IntermediateGenerator();
        }

        public void Compile()
        {
            Parser parser = Grammar.GetParser();
            parser.Parse(Input);
            Generator.Start(SyntaxTreeNode.CurrentNode);
        }

    }
}

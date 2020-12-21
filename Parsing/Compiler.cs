using Redmond.Lex;
using Redmond.Output;
using Redmond.Parsing.Nodes;
using System;

namespace Redmond.Parsing
{
    internal class Compiler
    {
        private readonly TokenStream _input;
        private readonly IStringStream _output;

        private readonly CompilationContext _context;

        public Compiler(TokenStream input, IStringStream output, CompilationContext context)
        {
            _input = input;
            _output = output;
            _context = context;
        }

        public void StartCompilation()
        {
            new ExpressionNode(_input.EatToken(), _context).Parse();
        }

        private void Error(String msg)
            => throw new Exception(msg);
    }
}

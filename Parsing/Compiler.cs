using Redmond.Lex;
using Redmond.Output;
using System;

namespace Redmond.Parsing
{
    internal class Compiler
    {
        private readonly TokenStream _input;
        private readonly OutputStream _output;

        private readonly CompilationContext _context;

        public Compiler(TokenStream input, OutputStream output, CompilationContext context)
        {
            _input = input;
            _output = output;
            _context = context;
        }

        public void StartCompilation()
        {
            //new ExpressionNode(_input.EatToken(), _context).Parse();

            Token t = _input.EatToken();

            while (t.Type.Name != "EndOfFile")
            {
                _output.WriteLine(t.Text + " ||| " + t.Type);
                t = _input.EatToken();
            }
        }

        private void Error(String msg)
            => throw new Exception(msg);
    }
}

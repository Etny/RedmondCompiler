using Redmond.Lex;
using Redmond.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing
{
    internal partial class Compiler
    {
        private TokenStream Input;
        private IStringStream Output;

        private Token NextToken { get => Input.NextToken; }

        public Compiler(TokenStream input, IStringStream output)
        {
            Input = input;
            Output = output;
        }

        public void StartCompilation()
        {
            ParseExpression();
        }

        private void Error(String msg)
            => throw new Exception(msg);
    }
}

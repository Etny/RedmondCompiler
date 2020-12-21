using Redmond.Lex;
using Redmond.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.Nodes
{
    abstract class SyntaxNode
    {
        public readonly Token Token;

        public SyntaxNode(Token t)
        {
            Token = t;
        }

        public abstract void Parse(TokenStream input, IStringStream output);

        protected void Error(String msg)
            => throw new Exception(msg);
    }
}

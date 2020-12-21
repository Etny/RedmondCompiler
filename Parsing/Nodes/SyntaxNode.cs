using Redmond.Lex;
using Redmond.Output;
using System;

namespace Redmond.Parsing.Nodes
{
    abstract class SyntaxNode
    {
        public readonly Token Token;

        protected readonly CompilationContext Context;
        protected TokenStream Input { get => Context.Input; }
        protected IStringStream Output { get => Context.Output; set { } }

        public SyntaxNode(Token t, CompilationContext context)
        {
            Token = t;
            Context = context;
        }

        public void Parse()
            => Parse(Context.Output);

        public abstract void Parse(IStringStream Output);

        protected void Error(String msg)
            => throw new Exception(msg);
    }
}

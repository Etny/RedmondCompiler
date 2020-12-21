using Redmond.Lex;
using Redmond.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.Nodes
{
    class FactorNode : SyntaxNode
    {

        public FactorNode(Token t) : base(t) { }

        public override void Parse(TokenStream input, IStringStream output)
        {
            switch (Token.Type)
            {
                case TokenType.NumLiteral:
                    output *= "push " + Token.NumValue;
                    return;

                case TokenType.Punctuation:
                    if(Token.Text == "(")
                    {
                        new ExpressionNode(input.EatToken()).Parse(input, output);
                        input.Match(")");
                    }

                    return;

            }
        }

    }
}

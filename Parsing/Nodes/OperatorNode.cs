using Redmond.Lex;
using Redmond.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.Nodes
{
    class OperatorNode : SyntaxNode
    {

        public OperatorNode(Token t) : base(t) { }

        public override void Parse(TokenStream input, IStringStream output)
        {
            if (Token.Type != TokenType.Operator)
                Error($"\'{Token.Text}\' is not an operator");

            switch (Token.Text)
            {
                case "+":
                    output *= "add";
                    return;

                case "-":
                    output *= "sub";
                    return;

                case "*":
                    output *= "mul";
                    return;

                case "/":
                    output *= "div";
                    return;
            }

            Error($"Unknow operator \'{Token.Text}\'");
        }
    }
}

using Redmond.Lex;
using Redmond.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.Nodes
{
    class ExpressionNode : SyntaxNode
    {
        public ExpressionNode(Token t) : base(t) { }

        public override void Parse(TokenStream input, IStringStream output)
        {
            output.AddIndentation();

            new TermNode(Token).Parse(input, output);

            while (input.NextToken.Type == TokenType.Operator)
            {
                var op = new OperatorNode(input.EatToken());
                new TermNode(input.EatToken()).Parse(input, output);
                op.Parse(input, output);
            }

            output.ReduceIndentation();
        }

    }
}

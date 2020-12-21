using Redmond.Lex;
using Redmond.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.Nodes
{
    class TermNode : SyntaxNode
    {
        public TermNode(Token t) : base(t) { }

        public override void Parse(TokenStream input, IStringStream output)
        {
            new FactorNode(Token).Parse(input, output);

            while(input.NextToken.Type == TokenType.Operator && "*/".Contains(input.NextToken.Text))
            {
                var op = new OperatorNode(input.EatToken());
                new FactorNode(input.EatToken()).Parse(input, output);
                op.Parse(input, output);
            }

            //Error($"\'{Token.Text}\' is not a Term");
        }
    }
}

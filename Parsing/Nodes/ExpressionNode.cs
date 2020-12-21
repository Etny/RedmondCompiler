﻿using Redmond.Lex;
using Redmond.Output;

namespace Redmond.Parsing.Nodes
{
    class ExpressionNode : SyntaxNode
    {
        public ExpressionNode(Token t, CompilationContext context) : base(t, context) { }

        public override void Parse(IStringStream Output)
        {
            Output.AddIndentation();

            new TermNode(Token, Context).Parse();

            while (Input.NextToken.Type == TokenType.Operator)
            {
                var op = new OperatorNode(Input.EatToken(), Context);
                new TermNode(Input.EatToken(), Context).Parse();
                op.Parse();
            }

            Output.ReduceIndentation();
        }

    }
}
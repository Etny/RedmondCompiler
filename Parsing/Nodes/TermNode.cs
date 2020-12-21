using Redmond.Lex;
using Redmond.Output;

namespace Redmond.Parsing.Nodes
{
    class TermNode : SyntaxNode
    {
        public TermNode(Token t, CompilationContext context) : base(t, context) { }

        public override void Parse(IStringStream Output)
        {
            new FactorNode(Token, Context).Parse();

            while (Input.NextToken.Type == TokenType.Operator && "*/".Contains(Input.NextToken.Text))
            {
                var op = new OperatorNode(Input.EatToken(), Context);
                new FactorNode(Input.EatToken(), Context).Parse();
                op.Parse();
            }

            //Error($"\'{Token.Text}\' is not a Term");
        }
    }
}

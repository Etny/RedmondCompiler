using Redmond.Lex;
using Redmond.Output;

namespace Redmond.Parsing.Nodes
{
    class FactorNode : SyntaxNode
    {

        public FactorNode(Token t, CompilationContext context) : base(t, context) { }

        public override void Parse(IStringStream Output)
        {
            switch (Token.Type)
            {
                case TokenType.NumLiteral:
                    Output *= "push " + Token.NumValue;
                    return;

                case TokenType.Punctuation:
                    if (Token.Text == "(")
                    {
                        new ExpressionNode(Input.EatToken(), Context).Parse();
                        Input.Match(")");
                    }

                    return;

            }
        }

    }
}

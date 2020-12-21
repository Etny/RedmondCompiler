using Redmond.Lex;
using Redmond.Output;

namespace Redmond.Parsing.Nodes
{
    class OperatorNode : SyntaxNode
    {

        public OperatorNode(Token t, CompilationContext context) : base(t, context) { }

        public override void Parse(IStringStream Output)
        {
            if (Token.Type != TokenType.Operator)
                Error($"\'{Token.Text}\' is not an operator");

            switch (Token.Text)
            {
                case "+":
                    Output *= "add";
                    return;

                case "-":
                    Output *= "sub";
                    return;

                case "*":
                    Output *= "mul";
                    return;

                case "/":
                    Output *= "div";
                    return;
            }

            Error($"Unknow operator \'{Token.Text}\'");
        }
    }
}

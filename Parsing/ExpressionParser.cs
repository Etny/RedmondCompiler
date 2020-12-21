using Redmond.Lex;
using Redmond.Parsing.Nodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing
{
    internal partial class Compiler
    {
        private void ParseExpression()
        {
            new ExpressionNode(Input.EatToken()).Parse(Input, Output);
        }
    }
}

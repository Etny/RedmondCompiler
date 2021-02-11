using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration
{
    internal partial class CodeGenerator
    {

        [CodeGenFunction("binop")]
        public void CompileBinaryOperator(SyntaxTreeNode node)
        {
            builder.EmitString("Binop: " + node.Val);
        }
    }
}

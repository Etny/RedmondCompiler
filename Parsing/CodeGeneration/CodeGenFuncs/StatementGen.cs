using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration
{
    internal partial class CodeGenerator
    {

        [CodeGenFunction("statement")]
        public void CompileStatement(SyntaxTreeNode node)
        {
            switch (node.ValueString)
            {
                case "assign":
                    CompileAssign(node);
                    break;

                default:
                    throw new Exception("Unknown statement type: " + node.ValueString);
            }
        }

        public void CompileAssign(SyntaxTreeNode node)
        {
            CompileExpression(node.Children[1]);
            builder.EmitString("Assigning to ID " + node.Children[0].Val);
        }
    }
}

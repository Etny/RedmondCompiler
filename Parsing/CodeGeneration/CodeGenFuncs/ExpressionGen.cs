using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration
{
    internal partial class CodeGenerator
    {

        [CodeGenFunction("expr")]
        public void CompileExpression(SyntaxTreeNode node)
        {
            switch (node.ValueString)
            {
                case "const":
                    CompileConstExpression(node.Children[0]);
                    break;

                case "binop":
                    CompileBinaryExpression(node);
                    break;

                default:
                    throw new Exception("Unknown expression type: " + node.ValueString);
            }
        }

        private void CompileConstExpression(SyntaxTreeNode node)
        {
            switch (node.Op)
            {
                case "num":
                    builder.EmitString("Num Const: " + node.Val);
                    break;

                case "string":
                    builder.EmitString("String Const: " + node.Val);
                    break;

                case "id":
                    builder.EmitString("Id Const: " + node.Val);
                    break;

                default:
                    throw new Exception("Unknown const type: " + node.ValueString);
            }
        }

        private void CompileBinaryExpression(SyntaxTreeNode node)
        {
            CompileExpression(node.Children[0]);
            CompileExpression(node.Children[1]);
            CompileBinaryOperator(node.Children[2]);
        }
    }
}

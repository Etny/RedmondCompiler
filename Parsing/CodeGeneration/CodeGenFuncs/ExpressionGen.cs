using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration
{
    internal partial class CodeGenerator
    {
        [CodeGenFunction("StringLiteral")]

        [CodeGenFunction("NumericalLiteral")]
        public void CompileNumLiteralExpression(SyntaxTreeNode node)
        {
            builder.EmitLine("Push " + node.Val);
        }

        [CodeGenFunction("IdentifierName")]
        public void CompileIdNameExpression(SyntaxTreeNode node)
        {
            builder.EmitLine("Push ID " + node.Val);
        }

        [CodeGenFunction("BinaryExpression")]
        public void CompileBinaryExpression(SyntaxTreeNode node)
        {
            CompileNodes(node.Children[0], node.Children[1]);
            CompileBinaryOperator(node.Children[2]);
        }
    }
}

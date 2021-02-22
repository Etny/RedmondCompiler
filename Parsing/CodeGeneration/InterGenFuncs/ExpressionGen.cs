using Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions;
using Redmond.Parsing.CodeGeneration.SymbolManagement;
using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration
{
    internal partial class IntermediateGenerator
    {

        [CodeGenFunction("StringLiteral")]
        [CodeGenFunction("NumericalLiteral")]
        [CodeGenFunction("IdentiferExpression")]
            public void CompileValueExpression(SyntaxTreeNode node) { }


        private bool IsValue(SyntaxTreeNode node)
            => ToValue(node) != null;

        private CodeValue ToValue(SyntaxTreeNode node)
        {
            switch (node.Op)
            {
                case "StringLiteral":
                case "NumericalLiteral":
                    return node.Val as CodeValue;

                case "IdentifierExpression":
                case "IdentifierName":
                    return GetFirst(node.ValueString);

                default:
                    return null;
            }
        }

        private InterInstOperand ToIntermediateExpression(SyntaxTreeNode node)
        {
            if (IsValue(node))
                return new InterInstOperand(ToValue(node));
            else
            {
                object o = CompileNode(node);
                return new InterInstOperand(o as IInterOp);
            }
        }

        [CodeGenFunction("BinaryExpression")]
        public IInterOp CompileBinaryExpression(SyntaxTreeNode node)
        {
            return (IInterOp)builder.AddInstruction(new InterBinOp(node[2].ValueString, ToIntermediateExpression(node.Children[0]), ToIntermediateExpression(node.Children[1])));
        }
    }
}
